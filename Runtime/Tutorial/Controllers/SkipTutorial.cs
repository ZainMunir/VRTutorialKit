using UnityEngine;

namespace ECDA.VRTutorialKit
{

    public class SkipTutorial : MonoBehaviour
    {
        [SerializeField] private GameObject skipPanel;

        TutorialManager tutorialManager;

        bool SkipAllowed => tutorialManager.tutorialConfig == null || tutorialManager.tutorialConfig.allowSkip;

        void Start()
        {
            tutorialManager = TutorialManager.Instance;

            if (tutorialManager == null)
            {
                Debug.LogError("TutorialManager instance not found.");
                return;
            }

            if (!SkipAllowed && skipPanel != null)
            {
                skipPanel.SetActive(false);
            }
        }

        public void Skip()
        {
            if (tutorialManager == null || !SkipAllowed) return;
            tutorialManager.ExitTutorial(TutorialManager.ExitReason.Skipped);
        }
    }
}
