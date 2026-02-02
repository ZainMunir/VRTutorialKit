using System;
using System.Collections;
using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class HighlightObject : MonoBehaviour
    {
        [SerializeField] private Color highlightColor = Color.yellow;
        [SerializeField, Range(0f, 10f)] private float outlineWidth = 10f;
        [SerializeField] private Outline.Mode outlineMode = Outline.Mode.OutlineAndSilhouette;
        [SerializeField] private bool startHighlighted = false;
        [Tooltip("Delay in seconds before applying the highlight at start. (only used if Start Highlighted is true)")]
        [SerializeField] private int highlightDelaySeconds = 1;
        [SerializeField] private bool startUnhighlighted = false;
        private Outline m_Outline;

        private void Awake()
        {
            m_Outline = GetComponent<Outline>();
            if (m_Outline == null)
            {
                m_Outline = gameObject.AddComponent<Outline>();
            }
        }

        private void Start()
        {
            if (startHighlighted)
            {
                // Wait 1 second to ensure all components are initialized
                StartCoroutine(WaitAndHighlight(Highlight, highlightDelaySeconds));
            }

            if (startUnhighlighted)
            {
                // Wait 1 second to ensure all components are initialized
                StartCoroutine(WaitAndHighlight(Unhighlight, 1));
            }
        }

        IEnumerator WaitAndHighlight(Action function = null, int delaySeconds = 1)
        {
            yield return new WaitForSeconds(delaySeconds);
            function?.Invoke();
        }

        public void Highlight()
        {
            m_Outline.OutlineColor = highlightColor;
            m_Outline.OutlineWidth = outlineWidth;
            m_Outline.OutlineMode = outlineMode;
            m_Outline.enabled = true;
        }

        public void Unhighlight()
        {
            m_Outline.enabled = false;
        }
    }
}
