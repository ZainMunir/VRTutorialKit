using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class CompleteOnTriggerTag : CompleteOnTriggerBase
    {
        [SerializeField] private string targetTag;

        protected override bool Evaluate(Collider other)
        {
            if (string.IsNullOrEmpty(targetTag)) return false;
            return other.CompareTag(targetTag);
        }
    }
}
