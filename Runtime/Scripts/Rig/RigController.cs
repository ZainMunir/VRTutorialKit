using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using System.Collections.Generic;

namespace ECDA.VRTutorialKit
{
    public class RigController : MonoBehaviour
    {
        [SerializeField] private ControllerInputActionManager leftControllerInputActionManager;
        [SerializeField] private ControllerInputActionManager rightControllerInputActionManager;

        [SerializeField] private XRRayInteractor leftTeleportRayInteractor;
        [SerializeField] private XRRayInteractor rightTeleportRayInteractor;

        [SerializeField] private SnapTurnProvider snapTurnProvider;
        [SerializeField] private DynamicMoveProvider dynamicMoveProvider;

        public enum LocomotionMode
        {
            None,
            Smooth,
            Teleport
        }

        [SerializeField] private LocomotionMode leftHandLocomotion;
        [SerializeField] private LocomotionMode rightHandLocomotion;
        [SerializeField] private bool enableSnapTurn;

        [SerializeField] private bool useHands = false;

        [SerializeField] private List<GameObject> controllers = new List<GameObject>();
        [SerializeField] private List<GameObject> hands = new List<GameObject>();
        public void SetLeftHandLocomotion(LocomotionMode mode)
        {
            leftHandLocomotion = mode;
        }
        public void SetRightHandLocomotion(LocomotionMode mode)
        {
            rightHandLocomotion = mode;
        }

        public void SetEnableSnapTurn(bool enabled)
        {
            enableSnapTurn = enabled;
        }


        public void SetUseHands(bool enabled)
        {
            useHands = enabled;
            foreach (var controller in controllers)
            {
                controller.SetActive(!enabled);
            }
            foreach (var hand in hands)
            {
                hand.SetActive(enabled);
            }
        }

        [ContextMenu("Toggle Use Hands")]
        public void ToggleUseHands()
        {
            SetUseHands(!useHands);
        }


        void Start()
        {
            UpdateLocomotionControls();
            SetUseHands(useHands);
        }

        void OnValidate()
        {
            UpdateLocomotionControls();
            SetUseHands(useHands);
        }

        private void AllowMovement(bool enabled)
        {
            dynamicMoveProvider.enabled = enabled;
        }

        private void SetSmoothMotion(bool enable, ControllerInputActionManager manager)
        {
            manager.smoothMotionEnabled = enable;
        }

        private void SetSnapTurn(bool enabled)
        {
            snapTurnProvider.enabled = enabled;
        }

        private void SetTeleportation(bool enabled, ControllerInputActionManager manager, XRRayInteractor rayInteractor)
        {
            SetSmoothMotion(!enabled, manager);
            rayInteractor.enabled = enabled;
        }

        private void DisableAllControls()
        {
            SetSnapTurn(false);
            SetTeleportation(false, leftControllerInputActionManager, leftTeleportRayInteractor);
            SetTeleportation(false, rightControllerInputActionManager, rightTeleportRayInteractor);
            AllowMovement(false);
        }

        public void UpdateLocomotionControls()
        {
            DisableAllControls();

            SetSnapTurn(enableSnapTurn);

            HandleLocomotionMode(leftHandLocomotion, leftControllerInputActionManager, leftTeleportRayInteractor);
            HandleLocomotionMode(rightHandLocomotion, rightControllerInputActionManager, rightTeleportRayInteractor);
        }

        void HandleLocomotionMode(LocomotionMode mode, ControllerInputActionManager manager, XRRayInteractor rayInteractor)
        {
            switch (mode)
            {
                case LocomotionMode.None:
                    SetSmoothMotion(false, manager);
                    break;
                case LocomotionMode.Smooth:
                    SetTeleportation(false, manager, rayInteractor);
                    AllowMovement(true);
                    break;
                case LocomotionMode.Teleport:
                    SetTeleportation(true, manager, rayInteractor);
                    break;
            }
        }
    }
}