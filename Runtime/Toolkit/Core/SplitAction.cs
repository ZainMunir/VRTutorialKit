using UnityEngine;
using UnityEngine.Events;

namespace ECDA.VRTutorialKit
{
    public class SplitAction : MonoBehaviour
    {
        public UnityEvent onTrue;
        public UnityEvent onFalse;

        public void DoAction(bool value)
        {
            if (value) onTrue?.Invoke();
            else onFalse?.Invoke();
        }
    }
}
