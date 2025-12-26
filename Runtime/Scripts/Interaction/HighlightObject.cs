using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class HighlightObject : MonoBehaviour
    {
        [SerializeField] private Color highlightColor = Color.yellow;
        [SerializeField, Range(0f, 10f)] private float outlineWidth = 10f;
        [SerializeField] private Outline.Mode outlineMode = Outline.Mode.OutlineAndSilhouette;
        private Outline m_Outline;

        private void Awake()
        {
            m_Outline = GetComponent<Outline>();
            if (m_Outline == null)
            {
                m_Outline = gameObject.AddComponent<Outline>();
                m_Outline.enabled = false;
            }
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
