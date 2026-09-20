using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class SocketInteractableTag : MonoBehaviour
    {
        [Tooltip("The tag associated with this interactable object.")]
        public TargetTag[] socketTag;

        public bool HasTag(TargetTag tag)
        {
            foreach (var t in socketTag)
            {
                if (t == tag) return true;
            }
            return false;
        }
    }
}
