using UnityEngine;
using UnityEngine.Events;

namespace ECDA.VRTutorialKit
{
    public class ExecuteOnce : MonoBehaviour
    {
        private bool hasExecuted = false;
        public UnityEvent onActionExecuted;

        public void Execute()
        {
            if (hasExecuted)
            {
                Debug.LogWarning("ExecuteOnce: Attempted to execute action more than once. Ignoring.");
                return;
            }

            hasExecuted = true;
            onActionExecuted?.Invoke();
        }

        public void CancelAction()
        {
            if (hasExecuted) return;
            hasExecuted = true;
        }
    }
}