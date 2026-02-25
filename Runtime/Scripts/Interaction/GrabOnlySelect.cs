using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ECDA.VRTutorialKit
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class GrabOnlySelect : MonoBehaviour
    {
        [SerializeField] private XRGrabInteractable grabInteractable;
        [SerializeField] private UnityEvent onGrabEntered;
        [SerializeField] private UnityEvent onGrabExited;
        private void Reset()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
        }

        private void Awake()
        {
            if (grabInteractable == null)
                grabInteractable = GetComponent<XRGrabInteractable>();
        }

        private void OnEnable()
        {
            grabInteractable.selectEntered.AddListener(HandleSelectEntered);
            grabInteractable.selectExited.AddListener(HandleSelectExited);
        }

        private void OnDisable()
        {
            grabInteractable.selectEntered.RemoveListener(HandleSelectEntered);
            grabInteractable.selectExited.RemoveListener(HandleSelectExited);
        }

        private void HandleSelectEntered(SelectEnterEventArgs args)
        {
            if (IsGrabInteractor(args.interactorObject))
                onGrabEntered?.Invoke();
        }

        private void HandleSelectExited(SelectExitEventArgs args)
        {
            if (IsGrabInteractor(args.interactorObject))
                onGrabExited?.Invoke();
        }

        private static bool IsGrabInteractor(IXRSelectInteractor interactor)
        {
            // Ignore sockets; allow direct/ray grabs
            Debug.Log($"Interactor {interactor}");
            if (interactor is XRSocketInteractor) return false;
            return true;
        }
    }
}