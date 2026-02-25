using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class BouncingArrowHandler : MonoBehaviour
    {
        [SerializeField] private GameObject bouncingArrowPrefab;
        [SerializeField] private TargetTag targetTag;
        private BouncingArrow activeArrow;

        [SerializeField] private Target overrideTarget;

        void ShowArrowAtTarget(GameObject target)
        {
            if (activeArrow != null)
            {
                Destroy(activeArrow.gameObject);
            }
            // Parent the arrow to the target to ensure it moves with it
            GameObject arrowInstance = Instantiate(bouncingArrowPrefab, target.transform);
            arrowInstance.transform.localPosition = Vector3.zero;
            activeArrow = arrowInstance.GetComponent<BouncingArrow>();
            activeArrow.StartBouncing();

        }

        [ContextMenu("Hide Arrow")]
        public void HideArrow()
        {
            if (activeArrow != null)
            {
                activeArrow.StopBouncing();
                Destroy(activeArrow.gameObject);
                activeArrow = null;
            }
        }

        [ContextMenu("Find Target and Show Arrow")]
        public void FindTargetAndShowArrow()
        {
            if (overrideTarget != null)
            {
                ShowArrowAtTarget(overrideTarget.gameObject);
                return;
            }

            Target[] targets = FindObjectsByType<Target>(FindObjectsSortMode.None);
            foreach (Target target in targets)
            {
                if (target.ValidTarget(targetTag))
                {
                    ShowArrowAtTarget(target.gameObject);
                    return;
                }
            }
            Debug.LogWarning($"No target found with tag: {targetTag.name}");
        }
    }
}