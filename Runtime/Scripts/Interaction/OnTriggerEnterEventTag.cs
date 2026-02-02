using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class OnTriggerEnterEventTag : OnTriggerEnterEventBase
    {
        [SerializeField] private string targetTag;

        protected override bool Evaluate(Collider other)
        {
            return other.CompareTag(targetTag);
        }
    }
}
