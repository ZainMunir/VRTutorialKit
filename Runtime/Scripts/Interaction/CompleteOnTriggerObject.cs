using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class CompleteOnTriggerObject : CompleteOnTriggerBase
    {
        [SerializeField] private GameObject targetObject;

        protected override bool Evaluate(Collider other)
        {
            return other.gameObject == targetObject;
        }
    }
}
