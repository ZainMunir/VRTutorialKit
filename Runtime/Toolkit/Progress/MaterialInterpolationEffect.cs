using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class MaterialInterpolationEffect : ProgressEffect
    {
        [SerializeField] private Renderer targetRenderer;
        [SerializeField] private Material startMaterial;
        [SerializeField] private Material endMaterial;
        private Material currentMaterial;

        private void Awake()
        {
            if (targetRenderer == null) targetRenderer = GetComponent<Renderer>();
            if (targetRenderer != null)
            {
                currentMaterial = targetRenderer.material;
                if (startMaterial == null)
                {
                    startMaterial = currentMaterial;
                }
            }
        }

        public override void ApplyProgress(float progress)
        {
            if (currentMaterial != null && startMaterial != null && endMaterial != null)
            {
                currentMaterial.color = Color.Lerp(startMaterial.color, endMaterial.color, progress);
            }
        }
    }
}