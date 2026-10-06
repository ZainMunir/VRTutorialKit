using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class OnTriggerEnterEventObject : OnTriggerEnterEventBase
    {
        [SerializeField] private GameObject targetObject;

        protected override GameObject Match(Collider other)
        {
            return other.gameObject == targetObject ? targetObject : null;
        }
    }
}
