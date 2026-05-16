using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ECDA.VRTutorialKit
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class ActionWhileGrabbed : MonoBehaviour
    {
        [Header("Input Actions")]
        [SerializeField] private InputActionReference leftTriggerAction;
        [SerializeField] private InputActionReference rightTriggerAction;

        [Header("Events")]
        [SerializeField] private UnityEvent onTriggerPressed;

        private XRGrabInteractable grabInteractable;
        private InputAction currentAction;

        private void Awake()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
            grabInteractable.selectEntered.AddListener(OnGrab);
            grabInteractable.selectExited.AddListener(OnRelease);
        }

        private void OnDestroy()
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
            SetCurrentAction(null);
        }

        private void OnGrab(SelectEnterEventArgs args)
        {
            if (args.interactorObject is not XRBaseInputInteractor interactor)
                return;

            var action = interactor.handedness switch
            {
                InteractorHandedness.Left => leftTriggerAction?.action,
                InteractorHandedness.Right => rightTriggerAction?.action,
                _ => null
            };

            SetCurrentAction(action);
        }

        private void OnRelease(SelectExitEventArgs args) => SetCurrentAction(null);

        private void SetCurrentAction(InputAction action)
        {
            if (currentAction != null)
                currentAction.performed -= OnTriggerPerformed;

            currentAction = action;

            if (currentAction != null)
                currentAction.performed += OnTriggerPerformed;
        }

        private void OnTriggerPerformed(InputAction.CallbackContext context)
            => onTriggerPressed?.Invoke();
    }
}