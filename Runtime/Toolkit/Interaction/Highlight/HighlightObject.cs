using System;
using System.Collections;
using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class HighlightObject : MonoBehaviour
    {
        [SerializeField] private Color highlightColor = Color.yellow;
        [SerializeField, Range(0f, 20f)] private float outlineWidth = 10f;
        [SerializeField] private bool startHighlighted = false;
        [SerializeField] private int highlightDelaySeconds = 1;
        private SimpleOutline m_SimpleOutline;

        private void Awake()
        {
            m_SimpleOutline = GetComponent<SimpleOutline>() ?? gameObject.AddComponent<SimpleOutline>();
            m_SimpleOutline.UpdateProperties(highlightColor, outlineWidth);
        }

        private void Start()
        {
            if (startHighlighted)
                StartCoroutine(WaitAndExecute(Highlight, highlightDelaySeconds));
        }

        private void OnValidate()
        {
            if (m_SimpleOutline != null)
                m_SimpleOutline.UpdateProperties(highlightColor, outlineWidth);
        }

        private IEnumerator WaitAndExecute(Action action, int delay)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }

        [ContextMenu("Highlight Object")]
        public void Highlight() => m_SimpleOutline.SetOutlineActive(true);
        [ContextMenu("Unhighlight Object")]
        public void Unhighlight() => m_SimpleOutline.SetOutlineActive(false);
    }
}