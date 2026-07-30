using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ECDA.VRTutorialKit
{
    public class HandGripBinder : MonoBehaviour
    {
        [SerializeField] private XRBaseInteractor interactor;
        [SerializeField] private HandPoseDriver driver;

        private void Reset()
        {
            interactor = GetComponentInParent<XRBaseInteractor>();
            driver = GetComponentInChildren<HandPoseDriver>();
        }

        private void OnEnable()
        {
            if (interactor == null || driver == null)
            {
                Debug.LogWarning($"{nameof(HandGripBinder)} on '{name}' is missing an interactor or driver reference.", this);
                return;
            }

            interactor.selectEntered.AddListener(OnSelectEntered);
            interactor.selectExited.AddListener(OnSelectExited);
        }

        private void OnDisable()
        {
            if (interactor == null) return;

            interactor.selectEntered.RemoveListener(OnSelectEntered);
            interactor.selectExited.RemoveListener(OnSelectExited);
        }

        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            GripPoseProvider provider = FindProvider(args.interactableObject as Object);
            if (provider != null)
            {
                provider.RegisterHolder(driver);
            }
        }

        private void OnSelectExited(SelectExitEventArgs args)
        {
            GripPoseProvider provider = FindProvider(args.interactableObject as Object);
            if (provider != null)
            {
                provider.UnregisterHolder(driver);
            }
            else
            {
                driver.ClearGripPose();
            }
        }

        private static GripPoseProvider FindProvider(Object interactable)
        {
            Component component = interactable as Component;
            return component == null ? null : component.GetComponentInParent<GripPoseProvider>();
        }
    }
}
