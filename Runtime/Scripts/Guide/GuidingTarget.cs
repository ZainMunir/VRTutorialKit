using System.Collections;
using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class GuidingTarget : MonoBehaviour
    {
        private GuidingController guidingController;
        private Transform targetTransform;
        private Coroutine delayedTargetCoroutine;

        void Awake()
        {
            targetTransform = transform;
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
            if (delayedTargetCoroutine != null)
            {
                StopCoroutine(delayedTargetCoroutine);
                delayedTargetCoroutine = null;
            }
            guidingController.RemoveTarget(targetTransform);
        }
        public void SetSelfAsTargetDelayed(int secondsDelay = 10)
        {
            if (delayedTargetCoroutine != null)
                StopCoroutine(delayedTargetCoroutine);
            delayedTargetCoroutine = StartCoroutine(SetTargetAfterDelay(secondsDelay));
        }

        private IEnumerator SetTargetAfterDelay(int secondsDelay)
        {
            yield return new WaitForSeconds(secondsDelay);
            SetSelfAsTarget();
        }
    }
}