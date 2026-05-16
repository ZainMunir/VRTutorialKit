using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Video;

namespace ECDA.VRTutorialKit
{
    [CreateAssetMenu(fileName = "TutorialStep", menuName = "VRTutorialKit/TutorialStep")]

    public class TutorialStep : ScriptableObject
    {
        public enum TooltipHand { Left, Right }

        [Header("UI Settings")]
        public LocalizedString stepTitle;
        public LocalizedString stepDescription;
        public VideoClip videoClip;

        [Header("Interaction Settings")]
        public GameObject interactionPrefab;
        public bool immediateCompletion = false;


        [Header("Tooltip Settings")]
        public List<GameObject> leftTooltipPrefabs = new List<GameObject>();
        public List<GameObject> rightTooltipPrefabs = new List<GameObject>();

        [Header("Control Settings")]
        public bool enableSnapTurn = false;

        public RigController.LocomotionMode leftHandLocomotion = RigController.LocomotionMode.None;
        public RigController.LocomotionMode rightHandLocomotion = RigController.LocomotionMode.None;
    }
}