using System.Collections.Generic;
using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class HoverVisual : MonoBehaviour
    {
        private struct BrightenedMaterial
        {
            public Renderer renderer;
            public int index;
            public Material material;
            public Color originalColor;
        }

        private Renderer[] _renderers = new Renderer[0];
        private readonly List<BrightenedMaterial> _brightened = new List<BrightenedMaterial>();
        private bool _isHovered;

        [SerializeField] private GameObject gameObjectsToHighlightRoot;

        void Start()
        {
            if (gameObjectsToHighlightRoot == null)
            {
                gameObjectsToHighlightRoot = this.gameObject;
            }
            _renderers = gameObjectsToHighlightRoot.GetComponentsInChildren<Renderer>();
        }

        public void SetHover(bool isHovered)
        {
            if (_isHovered == isHovered) return;
            _isHovered = isHovered;

            if (isHovered) Brighten();
            else Restore();
        }

        private void Brighten()
        {
            _brightened.Clear();
            foreach (var renderer in _renderers)
            {
                if (renderer == null) continue;

                var materials = renderer.materials;
                for (int i = 0; i < materials.Length; i++)
                {
                    var mat = materials[i];
                    if (!mat.HasProperty("_Color") || mat.name.Contains("Outline")) continue;  // Skip outline materials

                    _brightened.Add(new BrightenedMaterial
                    {
                        renderer = renderer,
                        index = i,
                        material = mat,
                        originalColor = mat.color
                    });
                    mat.color *= 1.2f;
                }
            }
        }

        // Skips any slot whose material was swapped while hovered, so the new material keeps its own color.
        private void Restore()
        {
            foreach (var entry in _brightened)
            {
                if (entry.renderer == null) continue;

                var current = entry.renderer.sharedMaterials;
                if (entry.index < current.Length && current[entry.index] == entry.material)
                    entry.material.color = entry.originalColor;
            }
            _brightened.Clear();
        }
    }
}
