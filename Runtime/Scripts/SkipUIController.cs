using UnityEngine;
using UnityEngine.UIElements;

namespace ECDA.VRTutorialKit
{

    public class SkipUIController : MonoBehaviour
    {
        Button skipButton;
        TutorialManager tutorialManager;

        void Start()
        {
            tutorialManager = TutorialManager.Instance;
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            skipButton = root.Q<Button>("SkipButton");

            skipButton.clicked += SkipTutorial;
        }

        void OnDestroy()
        {
            skipButton.clicked -= SkipTutorial;
        }


        void SkipTutorial()
        {
            tutorialManager.FinishTutorial();
        }
    }
}
