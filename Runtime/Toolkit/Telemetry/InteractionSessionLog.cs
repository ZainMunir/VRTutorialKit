using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

namespace ECDA.VRTutorialKit
{
    // Records what the participant does in the scene to the session log: grabs, sockets, activations, respawns, conditions, button presses, triggers, locomotion and recenters. Works in any scene with a SessionLogger.
    public class InteractionSessionLog : MonoBehaviour
    {
        [Tooltip("Grabs, releases, socket placements and removals, activations.")]
        [SerializeField] private bool logInteractions = true;

        [Tooltip("Teleports, snap and continuous turns, continuous moves, other locomotion providers, recenters.")]
        [SerializeField] private bool logLocomotion = true;

        [Tooltip("Conditions met or lost, button presses, trigger entries.")]
        [SerializeField] private bool logConditions = true;

        [Tooltip("Respawns, with what caused them.")]
        [SerializeField] private bool logRespawns = true;

        private readonly List<XRInteractionManager> managers = new List<XRInteractionManager>();
        private readonly HashSet<IXRSelectInteractor> hookedInteractors = new HashSet<IXRSelectInteractor>();
        private readonly HashSet<IXRActivateInteractable> hookedActivatables = new HashSet<IXRActivateInteractable>();
        private readonly Dictionary<(IXRSelectInteractor, IXRSelectInteractable), (double time, string via)> selections =
            new Dictionary<(IXRSelectInteractor, IXRSelectInteractable), (double, string)>();
        private readonly List<LocomotionProvider> providers = new List<LocomotionProvider>();
        private readonly Dictionary<LocomotionProvider, (Vector3 position, float yaw, double time)> locomotionStarts =
            new Dictionary<LocomotionProvider, (Vector3, float, double)>();
        private XROrigin xrOrigin;

        private static double Now => Time.realtimeSinceStartupAsDouble;

        private void OnEnable()
        {
            RespawnableObject.AnyRespawned += OnRespawned;
            ConditionSourceBase.AnyConditionStateChanged += OnConditionStateChanged;
            ActionOnButtonPress.AnyButtonPressed += OnButtonPressed;
            OnTriggerEnterEventBase.AnyTriggered += OnTriggered;
            RecenterDetector.AnyRecenterDetected += OnRecenterDetected;
        }

        private void OnDisable()
        {
            RespawnableObject.AnyRespawned -= OnRespawned;
            ConditionSourceBase.AnyConditionStateChanged -= OnConditionStateChanged;
            ActionOnButtonPress.AnyButtonPressed -= OnButtonPressed;
            OnTriggerEnterEventBase.AnyTriggered -= OnTriggered;
            RecenterDetector.AnyRecenterDetected -= OnRecenterDetected;
        }

        // Scene discovery waits for Start so every manager, interactor and provider has woken up.
        private void Start()
        {
            var interactors = new List<IXRInteractor>();
            var interactables = new List<IXRInteractable>();
            foreach (var manager in FindObjectsByType<XRInteractionManager>(FindObjectsSortMode.None))
            {
                managers.Add(manager);
                manager.interactorRegistered += OnInteractorRegistered;
                manager.interactorUnregistered += OnInteractorUnregistered;
                manager.interactableRegistered += OnInteractableRegistered;
                manager.interactableUnregistered += OnInteractableUnregistered;

                manager.GetRegisteredInteractors(interactors);
                foreach (var interactor in interactors) Hook(interactor);
                manager.GetRegisteredInteractables(interactables);
                foreach (var interactable in interactables) Hook(interactable);
            }

            xrOrigin = FindFirstObjectByType<XROrigin>();
            providers.AddRange(FindObjectsByType<LocomotionProvider>(FindObjectsInactive.Include, FindObjectsSortMode.None));
            foreach (var provider in providers)
            {
                provider.locomotionStarted += OnLocomotionStarted;
                provider.locomotionEnded += OnLocomotionEnded;
            }
        }

        private void OnDestroy()
        {
            foreach (var manager in managers)
            {
                if (manager == null) continue;
                manager.interactorRegistered -= OnInteractorRegistered;
                manager.interactorUnregistered -= OnInteractorUnregistered;
                manager.interactableRegistered -= OnInteractableRegistered;
                manager.interactableUnregistered -= OnInteractableUnregistered;
            }

            foreach (var interactor in hookedInteractors)
            {
                interactor.selectEntered.RemoveListener(OnSelectEntered);
                interactor.selectExited.RemoveListener(OnSelectExited);
            }

            foreach (var activatable in hookedActivatables)
            {
                activatable.activated.RemoveListener(OnActivated);
                activatable.deactivated.RemoveListener(OnDeactivated);
            }

            foreach (var provider in providers)
            {
                if (provider == null) continue;
                provider.locomotionStarted -= OnLocomotionStarted;
                provider.locomotionEnded -= OnLocomotionEnded;
            }
        }

        private void OnInteractorRegistered(InteractorRegisteredEventArgs args) => Hook(args.interactorObject);

        private void OnInteractableRegistered(InteractableRegisteredEventArgs args) => Hook(args.interactableObject);

        private void OnInteractorUnregistered(InteractorUnregisteredEventArgs args)
        {
            if (args.interactorObject is IXRSelectInteractor interactor && hookedInteractors.Remove(interactor))
            {
                interactor.selectEntered.RemoveListener(OnSelectEntered);
                interactor.selectExited.RemoveListener(OnSelectExited);
            }
        }

        private void OnInteractableUnregistered(InteractableUnregisteredEventArgs args)
        {
            if (args.interactableObject is IXRActivateInteractable activatable && hookedActivatables.Remove(activatable))
            {
                activatable.activated.RemoveListener(OnActivated);
                activatable.deactivated.RemoveListener(OnDeactivated);
            }
        }

        private void Hook(IXRInteractor interactor)
        {
            if (interactor is IXRSelectInteractor selector && hookedInteractors.Add(selector))
            {
                selector.selectEntered.AddListener(OnSelectEntered);
                selector.selectExited.AddListener(OnSelectExited);
            }
        }

        private void Hook(IXRInteractable interactable)
        {
            if (interactable is BaseTeleportationInteractable) return;
            if (interactable is IXRActivateInteractable activatable && hookedActivatables.Add(activatable))
            {
                activatable.activated.AddListener(OnActivated);
                activatable.deactivated.AddListener(OnDeactivated);
            }
        }

        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (!logInteractions) return;
            // Teleporting selects the floor or anchor; the teleport itself is logged as locomotion.
            if (args.interactableObject is BaseTeleportationInteractable) return;

            var via = Via(args.interactorObject);
            selections[(args.interactorObject, args.interactableObject)] = (Now, via);

            if (args.interactorObject is XRSocketInteractor)
            {
                Log("socket_placed", new SocketInfo { target = NameOf(args.interactableObject), socket = NameOf(args.interactorObject) });
            }
            else
            {
                Log("grab", new GrabInfo { target = NameOf(args.interactableObject), hand = Hand(args.interactorObject), via = via });
            }
        }

        private void OnSelectExited(SelectExitEventArgs args)
        {
            if (!logInteractions) return;
            if (args.interactableObject is BaseTeleportationInteractable) return;

            var key = (args.interactorObject, args.interactableObject);
            float seconds = -1f;
            var via = Via(args.interactorObject);
            if (selections.TryGetValue(key, out var selection))
            {
                seconds = (float)(Now - selection.time);
                via = selection.via;
                selections.Remove(key);
            }

            if (args.interactorObject is XRSocketInteractor)
            {
                Log("socket_removed", new SocketExitInfo
                {
                    target = NameOf(args.interactableObject),
                    socket = NameOf(args.interactorObject),
                    seconds = seconds,
                    canceled = args.isCanceled,
                });
            }
            else
            {
                Log("release", new ReleaseInfo
                {
                    target = NameOf(args.interactableObject),
                    hand = Hand(args.interactorObject),
                    via = via,
                    seconds = seconds,
                    canceled = args.isCanceled,
                });
            }
        }

        private void OnActivated(ActivateEventArgs args)
        {
            if (!logInteractions) return;
            Log("activate", new HandInfo { target = NameOf(args.interactableObject), hand = Hand(args.interactorObject) });
        }

        private void OnDeactivated(DeactivateEventArgs args)
        {
            if (!logInteractions) return;
            Log("deactivate", new HandInfo { target = NameOf(args.interactableObject), hand = Hand(args.interactorObject) });
        }

        private void OnRespawned(RespawnableObject respawnable, RespawnableObject.Cause cause)
        {
            if (!logRespawns) return;
            Log("respawn", new RespawnInfo { target = respawnable.name, cause = CauseName(cause) });
        }

        private void OnConditionStateChanged(ConditionSourceBase source, bool met)
        {
            if (!logConditions) return;
            Log(met ? "condition_met" : "condition_lost", new SourceInfo { source = source.name, type = source.GetType().Name });
        }

        private void OnButtonPressed(ActionOnButtonPress source, string action)
        {
            if (!logConditions) return;
            Log("button_pressed", new ButtonInfo { source = source.name, action = action });
        }

        private void OnTriggered(OnTriggerEnterEventBase trigger, GameObject matched)
        {
            if (!logConditions) return;
            Log("trigger_entered", new TriggerInfo { trigger = trigger.name, matched = matched != null ? matched.name : string.Empty });
        }

        private void OnRecenterDetected(RecenterDetector detector, float distance)
        {
            if (!logLocomotion) return;
            Log("recenter", new RecenterInfo { distance = distance });
        }

        private void OnLocomotionStarted(LocomotionProvider provider)
        {
            if (!logLocomotion) return;
            locomotionStarts[provider] = (OriginPosition, OriginYaw, Now);

            if (provider is ContinuousMoveProvider || provider is ContinuousTurnProvider)
            {
                Log(provider is ContinuousMoveProvider ? "move_started" : "turn_started",
                    new LocomotionStartInfo { provider = provider.GetType().Name, position = OriginPosition });
            }
        }

        // XRI raises locomotionEnded after the queued body transformations are applied, so the origin is already at its destination.
        private void OnLocomotionEnded(LocomotionProvider provider)
        {
            if (!logLocomotion) return;
            var info = new LocomotionInfo { provider = provider.GetType().Name, position = OriginPosition };
            if (locomotionStarts.TryGetValue(provider, out var start))
            {
                info.distance = Vector3.Distance(start.position, info.position);
                info.angle = Mathf.DeltaAngle(start.yaw, OriginYaw);
                info.seconds = (float)(Now - start.time);
                locomotionStarts.Remove(provider);
            }

            Log(LocomotionEventName(provider), info);
        }

        private Vector3 OriginPosition => xrOrigin != null ? xrOrigin.Origin.transform.position : Vector3.zero;

        private float OriginYaw => xrOrigin != null ? xrOrigin.Origin.transform.eulerAngles.y : 0f;

        private static string LocomotionEventName(LocomotionProvider provider)
        {
            switch (provider)
            {
                case TeleportationProvider _: return "teleport";
                case SnapTurnProvider _: return "snap_turn";
                case ContinuousMoveProvider _: return "move_ended";
                case ContinuousTurnProvider _: return "turn_ended";
                default: return "locomotion";
            }
        }

        private static string Via(IXRInteractor interactor)
        {
            switch (interactor)
            {
                case NearFarInteractor nearFar: return nearFar.selectionRegion.Value == NearFarInteractor.Region.Far ? "far" : "near";
                case XRSocketInteractor _: return "socket";
                case XRPokeInteractor _: return "poke";
                case XRGazeInteractor _: return "gaze";
                case XRRayInteractor _: return "far";
                case XRDirectInteractor _: return "near";
                default: return interactor.GetType().Name;
            }
        }

        private static string Hand(IXRInteractor interactor)
        {
            switch (interactor.handedness)
            {
                case InteractorHandedness.Left: return "left";
                case InteractorHandedness.Right: return "right";
                default: return "none";
            }
        }

        private static string CauseName(RespawnableObject.Cause cause)
        {
            switch (cause)
            {
                case RespawnableObject.Cause.LeftVolume: return "left_volume";
                case RespawnableObject.Cause.Idle: return "idle";
                default: return "direct";
            }
        }

        private static string NameOf(IXRInteractor interactor) => interactor.transform != null ? interactor.transform.name : string.Empty;

        private static string NameOf(IXRInteractable interactable) => interactable.transform != null ? interactable.transform.name : string.Empty;

        // The XRI and locomotion hooks live from Start to OnDestroy, so disabling the component is checked here.
        private void Log(string eventName, object data)
        {
            if (!isActiveAndEnabled) return;
            var logger = SessionLogger.Instance;
            if (logger != null) logger.Log(eventName, data);
        }

        [Serializable]
        private struct GrabInfo
        {
            public string target, hand, via;
        }

        [Serializable]
        private struct ReleaseInfo
        {
            public string target, hand, via;
            public float seconds;
            public bool canceled;
        }

        [Serializable]
        private struct SocketInfo
        {
            public string target, socket;
        }

        [Serializable]
        private struct SocketExitInfo
        {
            public string target, socket;
            public float seconds;
            public bool canceled;
        }

        [Serializable]
        private struct HandInfo
        {
            public string target, hand;
        }

        [Serializable]
        private struct RespawnInfo
        {
            public string target, cause;
        }

        [Serializable]
        private struct SourceInfo
        {
            public string source, type;
        }

        [Serializable]
        private struct ButtonInfo
        {
            public string source, action;
        }

        [Serializable]
        private struct TriggerInfo
        {
            public string trigger, matched;
        }

        [Serializable]
        private struct RecenterInfo
        {
            public float distance;
        }

        [Serializable]
        private struct LocomotionStartInfo
        {
            public string provider;
            public Vector3 position;
        }

        [Serializable]
        private struct LocomotionInfo
        {
            public string provider;
            public Vector3 position;
            public float distance, angle, seconds;
        }
    }
}
