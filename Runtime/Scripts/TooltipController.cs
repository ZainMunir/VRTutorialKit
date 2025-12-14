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

        public void AddTooltip(GameObject tooltipPrefab)
        {
            if (tooltipPrefab == null)
                return;

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

    }
}