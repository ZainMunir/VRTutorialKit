using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class TriggerParenting : MonoBehaviour
    {
        [Tooltip("If true, objects entering the trigger will be parented to this object.")]
        public bool parentObjects = true;

        private void OnTriggerStay(Collider other)
        {
            if (parentObjects && other.attachedRigidbody != null && !other.attachedRigidbody.isKinematic)
            {
                if (other.attachedRigidbody.transform.parent != transform)
                {
                    other.attachedRigidbody.transform.SetParent(transform);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (parentObjects && other.attachedRigidbody != null)
            {
                if (other.attachedRigidbody.transform.parent == transform)
                {
                    other.attachedRigidbody.transform.SetParent(null);
                }
            }
        }
    }
}