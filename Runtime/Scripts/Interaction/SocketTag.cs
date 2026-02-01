using UnityEngine;

namespace ECDA.VRTutorialKit
{
    [CreateAssetMenu(fileName = "SocketTag", menuName = "VRTutorialKit/Socket Tag")]
    public class SocketTag : ScriptableObject
    {
        [TextArea]
        public string description;
    }
}
