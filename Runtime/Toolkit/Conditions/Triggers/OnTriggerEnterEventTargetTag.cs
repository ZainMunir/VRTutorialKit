using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class OnTriggerEnterEventTargetTag : OnTriggerEnterEventBase
    {
        [SerializeField] private TargetTag targetTag;
        protected override GameObject Match(Collider other)
        {
            var tagged = other.GetComponentInParent<SocketInteractableTag>();
            bool matches = tagged != null && targetTag != null && tagged.HasTag(targetTag);
            return matches ? tagged.gameObject : null;
        }
    }
}
