using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


namespace ECDA.VRTutorialKit
{

    public class TutorialManager : MonoBehaviour
    {
        public TutorialConfig tutorialConfig;

        public TooltipController leftTooltipController;
        public TooltipController rightTooltipController;

        [SerializeField] private InputActionReference _leftActivateAction;
        [SerializeField] private InputActionReference _rightActivateAction;


        private int currentStepIndex = 0;

        void Start()
        {
            LoadCurrentStep();

            _leftActivateAction.action.performed += ctx => CycleStep();
            _rightActivateAction.action.performed += ctx => FinishTutorial();
        }

        void LoadCurrentStep()
        {
            TutorialStep step = tutorialConfig.tutorialSteps[currentStepIndex];

            switch (step.tooltipHand)
            {
                case TutorialStep.TooltipHand.Left:
                    leftTooltipController.ReplaceTooltip(step.tooltipPrefabs);
                    rightTooltipController.RemoveAllTooltips();
                    break;
                case TutorialStep.TooltipHand.Right:
                    rightTooltipController.ReplaceTooltip(step.tooltipPrefabs);
                    leftTooltipController.RemoveAllTooltips();
                    break;
                case TutorialStep.TooltipHand.Both:
                    leftTooltipController.ReplaceTooltip(step.tooltipPrefabs);
                    rightTooltipController.ReplaceTooltip(step.tooltipPrefabs);
                    break;
            }
        }

        [ContextMenu("Complete Step")]
        void CompleteStep()
        {
            currentStepIndex++;
            if (currentStepIndex < tutorialConfig.tutorialSteps.Count)
            {
                LoadCurrentStep();
            }
        }

        [ContextMenu("Finish Tutorial")]
        void FinishTutorial()
        {
            Debug.Log("Tutorial Finished!");
            SceneManager.LoadScene(tutorialConfig.startingScene);
        }

        void CycleStep()
        {
            currentStepIndex = (currentStepIndex + 1) % tutorialConfig.tutorialSteps.Count;
            LoadCurrentStep();
        }
    }
}
