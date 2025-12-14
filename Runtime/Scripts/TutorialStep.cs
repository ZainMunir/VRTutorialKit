using UnityEngine;

namespace ECDA.VRTutorialKit
{
    [CreateAssetMenu(fileName = "TutorialStep", menuName = "Tutorial/TutorialStep")]

    public class TutorialStep : ScriptableObject
    {
        [Header("UI Settings")]
        public string stepTitle = "New Lesson";
        [TextArea(3, 10)]
        public string stepDescription = "Instructions for this step.";
        public string mediaPath;

        [Header("Interaction Settings")]
        public GameObject interactionPrefab;

        [Header("Tooltip Settings")]
        public GameObject tooltipPrefab;
        public enum TooltipHand { Left, Right, Both }
        public TooltipHand tooltipHand;
    }
}