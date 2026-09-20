using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

#if UNITY_EDITOR
using System.Collections.Generic;
using System.Text;
using UnityEngine.XR.Interaction.Toolkit.Transformers;
#endif

namespace ECDA.VRTutorialKit
{
    /// <summary>
    /// Grab interactable that resolves its attach transform per interactor: a left or right hand
    /// gets the matching handed attach point, everything else (sockets above all) keeps the attach
    /// transform assigned in the Inspector.
    /// In the Editor it additionally supports live tuning of the attach pose while held.
    /// </summary>
    public class HandedGrabInteractable : XRGrabInteractable
    {
        public Transform leftAttach;
        public Transform rightAttach;

#if UNITY_EDITOR
        [Header("Editor Attach Tuning")]
        [Tooltip("While held in Play Mode, re-captures the grab offset whenever the active attach " +
                 "transform is moved, so Scene view / Inspector edits apply immediately.")]
        [SerializeField] bool liveAttachTuning = true;

        readonly List<IXRGrabTransformer> _transformerBuffer = new List<IXRGrabTransformer>();
        Transform _activeAttach;
#endif

        /// <summary>
        /// The attach transform used by any interactor that is not a hand. This is the transform
        /// assigned on the component, i.e. the parent attach point the handed ones live under.
        /// </summary>
        Transform DefaultAttach => attachTransform != null ? attachTransform : transform;

        /// <inheritdoc />
        public override Transform GetAttachTransform(IXRInteractor interactor)
        {
            var resolved = base.GetAttachTransform(interactor);

            // Only the primary attach transform is swapped for a handed one. Dynamic, secondary and
            // predicted-visual attach transforms are handed back untouched so their own behaviour
            // is preserved.
            if (resolved != DefaultAttach)
                return resolved;

            var handedAttach = ResolveHandedAttach(interactor);
            return handedAttach != null ? handedAttach : resolved;
        }

        /// <summary>
        /// Returns the handed attach point for a left/right interactor, or null for anything else -
        /// sockets, gaze, and any interactor without a handed attach assigned.
        /// </summary>
        Transform ResolveHandedAttach(IXRInteractor interactor)
        {
            if (interactor == null)
                return null;

            switch (interactor.handedness)
            {
                case InteractorHandedness.Left:
                    return leftAttach;
                case InteractorHandedness.Right:
                    return rightAttach;
            }

            // Fallback for interactors that leave handedness unset but are tagged on the rig.
            var interactorTransform = interactor.transform;
            if (interactorTransform == null)
                return null;

            if (interactorTransform.CompareTag("LeftHand"))
                return leftAttach;

            if (interactorTransform.CompareTag("RightHand"))
                return rightAttach;

            return null;
        }

#if UNITY_EDITOR
        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            base.OnSelectEntering(args);

            // Resolved after the base call so any dynamic attach transform created for this
            // interactor is the one that gets tuned.
            _activeAttach = GetAttachTransform(args.interactorObject);
            if (_activeAttach != null)
                _activeAttach.hasChanged = false;
        }

        void Update()
        {
            if (!liveAttachTuning || !isSelected || _activeAttach == null)
                return;

            if (!_activeAttach.hasChanged)
                return;

            _activeAttach.hasChanged = false;
            RefreshGrabOffset();
        }

        /// <summary>
        /// Re-links the active grab transformers so they re-capture the interactor/attach offset
        /// from the attach transform's current pose. Deliberately avoids a select exit/enter cycle
        /// so that listeners (HeldInteractableGate, story beats, audio) are not pulsed.
        /// </summary>
        void RefreshGrabOffset()
        {
            if (interactorsSelecting.Count == 0)
                return;

            var useMultiple = interactorsSelecting.Count > 1 && multipleGrabTransformersCount > 0;

            _transformerBuffer.Clear();
            if (useMultiple)
                GetMultipleGrabTransformers(_transformerBuffer);
            else
                GetSingleGrabTransformers(_transformerBuffer);

            if (_transformerBuffer.Count == 0)
            {
                ForceRegrab();
                return;
            }

            for (var i = 0; i < _transformerBuffer.Count; i++)
            {
                var transformer = _transformerBuffer[i];
                if (useMultiple)
                {
                    RemoveMultipleGrabTransformer(transformer);
                    AddMultipleGrabTransformer(transformer);
                }
                else
                {
                    RemoveSingleGrabTransformer(transformer);
                    AddSingleGrabTransformer(transformer);
                }
            }
        }

        /// <summary>
        /// Fallback for setups with no registered transformer. This does fire select events.
        /// </summary>
        void ForceRegrab()
        {
            var interactor = interactorsSelecting[0];
            interactionManager.SelectExit(interactor, this);
            interactionManager.SelectEnter(interactor, this);
        }

        [ContextMenu("Copy Attach Poses To Clipboard")]
        void CopyAttachPosesToClipboard()
        {
            var sb = new StringBuilder();
            AppendPose(sb, nameof(leftAttach), leftAttach);
            AppendPose(sb, nameof(rightAttach), rightAttach);

            var text = sb.ToString();
            GUIUtility.systemCopyBuffer = text;
            Debug.Log($"{name} attach poses:\n{text}", this);
        }

        static void AppendPose(StringBuilder sb, string label, Transform t)
        {
            if (t == null)
            {
                sb.AppendLine($"{label}: <none>");
                return;
            }

            var p = t.localPosition;
            var e = t.localEulerAngles;
            sb.AppendLine($"{label}:");
            sb.AppendLine($"  m_LocalPosition: {{x: {p.x:F4}, y: {p.y:F4}, z: {p.z:F4}}}");
            sb.AppendLine($"  m_LocalEuler:    {{x: {e.x:F3}, y: {e.y:F3}, z: {e.z:F3}}}");
        }
#endif
    }
}
