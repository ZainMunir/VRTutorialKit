using UnityEngine;

namespace ECDA.VRTutorialKit
{
    [RequireComponent(typeof(Collider))]
    public abstract class CompleteOnTriggerBase : MonoBehaviour
    {
        private Collider m_collider;
        [SerializeField] protected TutorialSubStep subStep;

        [Tooltip("Time in seconds to wait after enabling before checking for collisions. Prevents triggering on spawn.")]
        [SerializeField] private float startDelay = 0.5f;
        private float m_TimeEnabled;

        protected virtual void Awake()
        {
            m_collider = GetComponent<Collider>();
            if (!m_collider.isTrigger)
            {
                Debug.LogWarning($"Collider on {name} is not set as Trigger. Setting isTrigger to true.");
                m_collider.isTrigger = true;
            }
            if (subStep == null)
            {
                Debug.LogError($"TutorialSubStep reference is not set on {GetType().Name} on {name}.");
            }
        }

        protected virtual void OnEnable()
        {
            m_TimeEnabled = Time.time;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (Time.time < m_TimeEnabled + startDelay) return;

            if (Evaluate(other))
            {
                if (subStep != null)
                {
                    subStep.Complete();
                }
            }
        }

        protected abstract bool Evaluate(Collider other);
    }
}
