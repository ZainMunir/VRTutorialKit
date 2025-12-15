using System.Collections.Generic;
using UnityEngine;

namespace ECDA.VRTutorialKit
{
    [RequireComponent(typeof(CalloutGazeController))]
    public class TooltipController : MonoBehaviour
    {
        CalloutGazeController m_CalloutGazeController;
        void Awake()
        {
            m_CalloutGazeController = GetComponent<CalloutGazeController>();
        }

        void AddTooltip(GameObject tooltipPrefab)
        {
            var instance = Instantiate(tooltipPrefab, transform);
            Callout callout = instance.GetComponent<Callout>();
            if (callout == null)
            {
                Debug.LogWarning("Tooltip prefab does not contain a Callout component.");
                return;
            }

            // Attach GazeHoverStart and GazeHoverEnd to the CalloutGazeController events
            if (m_CalloutGazeController != null)
            {
                m_CalloutGazeController.facingEntered.AddListener(callout.GazeHoverStart);
                m_CalloutGazeController.facingExited.AddListener(callout.GazeHoverEnd);
            }
        }

        public void AddTooltips(List<GameObject> tooltipPrefabs)
        {
            if (tooltipPrefabs == null)
                return;

            foreach (var prefab in tooltipPrefabs)
            {
                AddTooltip(prefab);
            }
        }

        public void RemoveAllTooltips()
        {
            foreach (Transform child in transform)
            {
                Callout callout = child.GetComponent<Callout>();
                if (callout != null && m_CalloutGazeController != null)
                {
                    m_CalloutGazeController.facingEntered.RemoveListener(callout.GazeHoverStart);
                    m_CalloutGazeController.facingExited.RemoveListener(callout.GazeHoverEnd);
                }
                Destroy(child.gameObject);
            }
        }

        public void ReplaceTooltip(List<GameObject> tooltipPrefabs)
        {
            RemoveAllTooltips();
            AddTooltips(tooltipPrefabs);
        }
    }
}