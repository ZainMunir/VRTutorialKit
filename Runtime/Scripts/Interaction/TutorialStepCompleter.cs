using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class TutorialStepCompleter : MonoBehaviour
    {
        public void CompleteCurrentStep()
        {
            if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.CompleteStep();
            }
        }
    }
}