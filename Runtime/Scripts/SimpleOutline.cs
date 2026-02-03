using UnityEngine;
using System.Collections.Generic;

namespace ECDA.VRTutorialKit
{
    public class SimpleOutline : MonoBehaviour
    {
        private int originalLayer;
        private int outlineLayer;
        private Renderer[] renderers;
        private MaterialPropertyBlock propBlock;

        private static readonly int ColorID = Shader.PropertyToID("_OutlineColor");
        private static readonly int WidthID = Shader.PropertyToID("_OutlineWidth");

        private void Awake()
        {
            outlineLayer = LayerMask.NameToLayer("Outline");
            originalLayer = gameObject.layer;
            renderers = GetComponentsInChildren<Renderer>();
            propBlock = new MaterialPropertyBlock();
        }

        public void UpdateProperties(Color color, float width)
        {
            foreach (var renderer in renderers)
            {
                renderer.GetPropertyBlock(propBlock);
                propBlock.SetColor(ColorID, color);
                propBlock.SetFloat(WidthID, width);
                renderer.SetPropertyBlock(propBlock);
            }
        }

        public void SetOutlineActive(bool active)
        {
            SetLayerRecursive(gameObject, active ? outlineLayer : originalLayer);
        }

        private void SetLayerRecursive(GameObject obj, int newLayer)
        {
            obj.layer = newLayer;
            foreach (Transform child in obj.transform)
            {
                if (child.GetComponent<SimpleOutline>() == null)
                    SetLayerRecursive(child.gameObject, newLayer);
            }
        }
    }
}