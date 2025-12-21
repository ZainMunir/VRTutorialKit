using System.Collections;
using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class HoverVisual : MonoBehaviour
    {
        public Renderer targetRenderer;

        private Material originalMaterial;
        private Color originalColor;

        void Start()
        {
            if (!targetRenderer) targetRenderer = GetComponent<Renderer>();
            originalMaterial = targetRenderer.material;
            originalColor = originalMaterial.color;
        }

        public void SetHover(bool isHovered)
        {

            targetRenderer.material.color = isHovered ? originalColor * 1.2f : originalColor;
        }
    }
}