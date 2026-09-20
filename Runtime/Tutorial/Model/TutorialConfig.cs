using System.Collections.Generic;
using UnityEngine;

namespace ECDA.VRTutorialKit
{
    [CreateAssetMenu(fileName = "TutorialConfig", menuName = "VRTutorialKit/TutorialConfig")]
    public class TutorialConfig : ScriptableObject
    {
        public List<TutorialStep> tutorialSteps = new List<TutorialStep>();
        [Scene] public string startingScene;
    }
}
