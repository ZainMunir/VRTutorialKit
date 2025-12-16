using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ECDA.VRTutorialKit
{
    [RequireComponent(typeof(Collider))]
    public class CompleteOnColliderEnter : MonoBehaviour
    {
        private Collider m_collider;
        public TutorialStepCompleter stepCompleter;

        void Awake()
        {
            m_collider = GetComponent<Collider>();
            if (!m_collider.isTrigger)
            {
                Debug.LogWarning("Collider is not set as Trigger. Setting isTrigger to true.");
                m_collider.isTrigger = true;
            }
        }

        void OnTriggerEnter(Collider other)
        {
            XRGrabInteractable grabInteractable = other.GetComponent<XRGrabInteractable>();
            if (grabInteractable != null && stepCompleter != null)
            {
                stepCompleter.CompleteCurrentStep();
            }
        }
    }
}