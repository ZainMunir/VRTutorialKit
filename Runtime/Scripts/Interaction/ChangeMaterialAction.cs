using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class ChangeMaterialAction : MonoBehaviour
    {
        public Material newMaterial;
        private Renderer objectRenderer;
        void Awake()
        {
            objectRenderer = GetComponent<Renderer>();
        }
        public void ChangeMaterial()
        {
            if (objectRenderer != null && newMaterial != null)
            {
                objectRenderer.material = newMaterial;
            }
        }
    }
}