using UnityEditor;
using UnityEngine;

namespace ECDA.VRTutorialKit
{
    [CreateAssetMenu(fileName = "TutorialConfig", menuName = "Tutorial/TutorialConfig")]
    public class TutorialConfig : ScriptableObject
    {
        public TutorialStep[] tutorialSteps;
        public SceneAsset startingScene;
    }
}
