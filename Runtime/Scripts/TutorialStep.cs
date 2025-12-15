using System.Collections.Generic;
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
        public List<GameObject> tooltipPrefabs = new List<GameObject>();
        public enum TooltipHand { Left, Right, Both }
        public TooltipHand tooltipHand;
    }
}