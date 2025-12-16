using ECDA.VRTutorialKit;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;

namespace ECDA.VRTutorialKit
{
    [RequireComponent(typeof(UIDocument))]
    [RequireComponent(typeof(VideoPlayer))]
    public class UIController : MonoBehaviour
    {
        TutorialManager tutorialManager;

        Label stepTitleLabel;
        Label stepDescriptionLabel;

        Button previousButton;
        Button nextButton;
        Button finishButton;

        VideoPlayer videoPlayer;

        void Awake()
        {
            videoPlayer = GetComponent<VideoPlayer>();
        }

        void Start()
        {

            tutorialManager = TutorialManager.Instance;

            if (tutorialManager == null)
            {
                Debug.LogError("TutorialManager instance not found.");
                return;
            }

            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            previousButton = root.Q<Button>("PreviousButton");
            nextButton = root.Q<Button>("NextButton");
            finishButton = root.Q<Button>("FinishButton");

            finishButton.SetEnabled(false);


            stepTitleLabel = root.Q<Label>("StepTitle");
            stepDescriptionLabel = root.Q<Label>("StepDescription");


            previousButton.clicked += PreviousStep;
            nextButton.clicked += NextStep;
            finishButton.clicked += FinishTutorial;

            tutorialManager.OnStepCompleted += (bool completed) => UpdateStepUI();
            tutorialManager.OnTutorialStepChanged += (bool changed) => UpdateStepUI();
            tutorialManager.OnTutorialFinished += (bool finished) => finishButton.SetEnabled(finished);

            UpdateStepUI();
        }

        void NextStep()
        {
            tutorialManager.NextStep();
        }

        void PreviousStep()
        {
            tutorialManager.PreviousStep();
        }

        void UpdateStepUI()
        {
            TutorialStep step = tutorialManager.GetCurrentStep();
            if (step == null)
                return;

            stepTitleLabel.text = step.stepTitle;
            stepDescriptionLabel.text = step.stepDescription;

            if (step.videoClip != null)
            {
                videoPlayer.Stop();
                videoPlayer.clip = step.videoClip;
                videoPlayer.Play();
            }
            else
            {
                videoPlayer.Stop();
                videoPlayer.clip = null;
            }

            previousButton.SetEnabled(tutorialManager.HasPreviousStep);
            nextButton.SetEnabled(tutorialManager.IsCurrentStepCompleted && tutorialManager.HasNextStep);
        }

        void FinishTutorial()
        {
            tutorialManager.FinishTutorial();
        }
    }
}