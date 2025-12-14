using UnityEngine;

namespace ECDA.VRTutorialKit
{

    public class TutorialManager : MonoBehaviour
    {
        public TutorialConfig tutorialConfig;

        public TooltipController leftTooltipController;
        public TooltipController rightTooltipController;

        private int currentStepIndex = 0;

        void Start()
        {
            LoadCurrentStep();

        }
        void LoadCurrentStep()
        {
            TutorialStep step = tutorialConfig.tutorialSteps[currentStepIndex];

            if (step.tooltipHand == TutorialStep.TooltipHand.Left || step.tooltipHand == TutorialStep.TooltipHand.Both)
                leftTooltipController.AddTooltip(step.tooltipPrefab);

            if (step.tooltipHand == TutorialStep.TooltipHand.Right || step.tooltipHand == TutorialStep.TooltipHand.Both)
                rightTooltipController.AddTooltip(step.tooltipPrefab);
        }
    }
}
