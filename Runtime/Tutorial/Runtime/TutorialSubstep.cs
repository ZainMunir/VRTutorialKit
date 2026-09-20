using UnityEngine;
using UnityEngine.Events;

namespace ECDA.VRTutorialKit
{
    public class TutorialSubStep : MonoBehaviour
    {
        [Tooltip("If true, this object will be disabled when the step is not active.")]
        public bool manageGameObjectActiveState = true;

        public UnityEvent OnSubStepActive;
        public UnityEvent OnSubStepCompleted;

        private bool m_IsCompleted;
        public bool IsCompleted => m_IsCompleted;

        private TutorialStepController m_Completer;

        public void Initialize(TutorialStepController completer)
        {
            m_Completer = completer;
            if (manageGameObjectActiveState)
            {
                gameObject.SetActive(false);
            }
        }

        public void SetActive(bool active)
        {
            if (manageGameObjectActiveState)
            {
                gameObject.SetActive(active);
            }

            if (active)
            {
                OnSubStepActive?.Invoke();
            }
        }

        [ContextMenu("Complete SubStep")]
        public void Complete()
        {
            if (m_IsCompleted) return;

            m_IsCompleted = true;
            OnSubStepCompleted?.Invoke();

            if (m_Completer != null)
            {
                m_Completer.OnSubStepCompleted(this);
            }
        }
    }
}
