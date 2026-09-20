using UnityEngine;


namespace ECDA.VRTutorialKit
{
    public class Target : MonoBehaviour
    {
        // Marker class for BouncingArrow
        public TargetTag targetTag;
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * 0.1f);

            Gizmos.DrawSphere(transform.position, 0.01f);
        }

        public bool ValidTarget(TargetTag tag)
        {
            return targetTag == tag;
        }
    }
}