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
            public List<(int index, Color originalColor)> colorData;
        }

        private List<RendererData> _renderers = new List<RendererData>();

        void Start()
        {
            var renderers = GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                var colorData = new List<(int, Color)>();
                for (int i = 0; i < renderer.materials.Length; i++)
                {
                    var mat = renderer.materials[i];
                    if (mat.HasProperty("_Color") && !mat.name.Contains("Outline"))  // Skip outline materials
                        colorData.Add((i, mat.color));
                    else
                        Debug.LogWarning($"Material on {renderer.gameObject.name} does not have a _Color property or is an outline material.");
                }
                _renderers.Add(new RendererData
                {
                    renderer = renderer,
                    colorData = colorData
                });
            }
        }

        public void SetHover(bool isHovered)
        {
            foreach (var data in _renderers)
            {
                foreach (var (index, originalColor) in data.colorData)
                {
                    data.renderer.materials[index].color = isHovered ? originalColor * 1.2f : originalColor;
                }
            }
        }
    }
}