using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class HoverVisual : MonoBehaviour
    {
        private class RendererData
        {
            public Renderer renderer;
            public List<Color> originalColors;
        }

        private List<RendererData> _renderers = new List<RendererData>();

        void Start()
        {
            var renderers = GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                var originalColors = new List<Color>();
                foreach (var mat in renderer.materials)
                {
                    originalColors.Add(mat.color);
                }
                _renderers.Add(new RendererData
                {
                    renderer = renderer,
                    originalColors = originalColors
                });
            }
        }

        public void SetHover(bool isHovered)
        {
            foreach (var data in _renderers)
            {
                for (int i = 0; i < data.renderer.materials.Length; i++)
                {
                    data.renderer.materials[i].color = isHovered ? data.originalColors[i] * 1.2f : data.originalColors[i];
                }
            }
        }
    }
}