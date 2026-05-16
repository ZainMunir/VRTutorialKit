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
        }

        public void Skip()
        {
            tutorialManager.FinishTutorial();
        }
    }
}
