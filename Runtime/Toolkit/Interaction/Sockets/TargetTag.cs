using UnityEngine;

namespace ECDA.VRTutorialKit
{
    [CreateAssetMenu(fileName = "TargetTag", menuName = "VRTutorialKit/Target Tag")]
    public class TargetTag : ScriptableObject
    {
        [TextArea]
        public string description;
    }
}
