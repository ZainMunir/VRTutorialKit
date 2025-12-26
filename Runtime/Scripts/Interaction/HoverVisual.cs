using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class HoverVisual : MonoBehaviour
    {
        private class RendererData
        {
            public Material material;
            public Color originalColor;
        }

        private List<RendererData> _renderers = new List<RendererData>();

        void Start()
        {
            var renderers = GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                var mat = renderer.material;
                _renderers.Add(new RendererData
                {
                    material = mat,
                    originalColor = mat.color
                });
            }
        }

        public void SetHover(bool isHovered)
        {
            foreach (var data in _renderers)
            {
                if (data.material != null)
                {
                    data.material.color = isHovered ? data.originalColor * 1.2f : data.originalColor;
                }
            }
        }
    }
}