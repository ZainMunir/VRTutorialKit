using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ECDA.VRTutorialKit
{
    [CreateAssetMenu(fileName = "TutorialConfig", menuName = "Tutorial/TutorialConfig")]
    public class TutorialConfig : ScriptableObject
    {
        public List<TutorialStep> tutorialSteps = new List<TutorialStep>();
        [Scene] public string startingScene;
    }
}
