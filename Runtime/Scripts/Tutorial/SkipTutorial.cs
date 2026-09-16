using UnityEngine;
using UnityEngine.UIElements;

namespace ECDA.VRTutorialKit
{

    public class SkipTutorial : MonoBehaviour
    {
        TutorialManager tutorialManager;

        void Start()
        {
            tutorialManager = TutorialManager.Instance;

            if (tutorialManager == null)
            {
                Debug.LogError("TutorialManager instance not found.");
            }
        }

        public void Skip()
        {
            if (tutorialManager == null) return;
            tutorialManager.FinishTutorial();
        }
    }
}
