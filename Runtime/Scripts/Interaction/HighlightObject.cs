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
        [SerializeField] private bool startUnhighlighted = false;
        [SerializeField] private int highlightDelaySeconds = 1;
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
            if (startHighlighted && startUnhighlighted)
            {
                Debug.LogWarning("Both startHighlighted and startUnhighlighted are set to true. Object will start unhighlighted.");
                startHighlighted = false;
            }

            if (startHighlighted)
                StartCoroutine(WaitAndHighlight(Highlight, highlightDelaySeconds));

            if (startUnhighlighted)
                StartCoroutine(WaitAndHighlight(Unhighlight, highlightDelaySeconds));
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
