using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class OnTriggerEnterEventObject : OnTriggerEnterEventBase
    {
        [SerializeField] private GameObject targetObject;

        protected override bool Evaluate(Collider other)
        {
            return other.gameObject == targetObject;
        }
    }
}
