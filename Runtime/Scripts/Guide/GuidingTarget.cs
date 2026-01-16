using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class GuidingTarget : MonoBehaviour
    {
        private GuidingController guidingController;
        private Transform targetTransform;

        void Awake()
        {
            targetTransform = this.transform;
            guidingController = FindAnyObjectByType<GuidingController>();

            if (guidingController == null)
            {
                Debug.LogError($"No {nameof(GuidingController)} found in the scene for {name}.");
                return;
            }
        }

        public void SetSelfAsTarget()
        {
            guidingController.SetNewTarget(targetTransform);
        }

        public void RemoveSelfAsTarget()
        {

            guidingController.RemoveTarget(targetTransform);
        }

    }
}