using UnityEngine;
using UnityEngine.Events;

namespace ECDA.VRTutorialKit
{
    public class ToggleAction : MonoBehaviour
    {
        public UnityEvent actionOne;
        public UnityEvent actionTwo;
        [SerializeField] private int index = 0;

        public void DoAction()
        {
            if (index == 0)
                actionOne?.Invoke();
            else actionTwo?.Invoke();
            index = (index + 1) % 2;
        }
    }
}
