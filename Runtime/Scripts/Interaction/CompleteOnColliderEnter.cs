using UnityEngine;

namespace ECDA.VRTutorialKit
{
    [RequireComponent(typeof(Collider))]
    public class CompleteOnColliderEnter : MonoBehaviour
    {
        private Collider m_collider;
        [SerializeField] private TutorialSubStep subStep;

        [SerializeField] private GameObject targetObject;

        void Awake()
        {
            m_collider = GetComponent<Collider>();
            if (!m_collider.isTrigger)
            {
                Debug.LogWarning("Collider is not set as Trigger. Setting isTrigger to true.");
                m_collider.isTrigger = true;
            }
            if (subStep == null)
            {
                Debug.LogError("TutorialSubStep reference is not set on CompleteOnColliderEnter.");
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject == targetObject)
            {
                if (subStep != null)
                {
                    subStep.Complete();
                }
            }
        }
    }
}