using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class OnTriggerEnterEventTargetTag : OnTriggerEnterEventBase
    {
        [SerializeField] private TargetTag targetTag;
        protected override bool Evaluate(Collider other)
        {
            var tagged = other.GetComponentInParent<SocketInteractableTag>();
            return tagged != null && targetTag != null && tagged.HasTag(targetTag);
        }
    }
}
