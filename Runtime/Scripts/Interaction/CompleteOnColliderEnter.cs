using UnityEngine;

namespace ECDA.VRTutorialKit
{
    [RequireComponent(typeof(Collider))]
    public class CompleteOnColliderEnter : MonoBehaviour
    {
        private Collider m_collider;
        [SerializeField] private TutorialSubStep subStep;

        [SerializeField] private GameObject targetObject;
        [SerializeField] private string targetTag;

        [Tooltip("Time in seconds to wait after enabling before checking for collisions. Prevents triggering on spawn.")]
        [SerializeField] private float startDelay = 0.5f;
        private float m_TimeEnabled;

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

        void OnEnable()
        {
            m_TimeEnabled = Time.time;
        }

        void OnTriggerEnter(Collider other)
        {
            if (Time.time < m_TimeEnabled + startDelay) return;

            if (other.gameObject == targetObject)
            {
                if (subStep != null)
                {
                    subStep.Complete();
                }
            }
            else if (!string.IsNullOrEmpty(targetTag))
            {
                if (other.CompareTag(targetTag))
                {
                    if (subStep != null)
                    {
                        subStep.Complete();
                    }
                }
            }
        }
    }
}