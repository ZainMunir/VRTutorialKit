using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class ChangeMaterialOnComplete : MonoBehaviour
    {
        public Material newMaterial;
        private Renderer objectRenderer;
        [SerializeField] private TutorialSubStep subStep;

        void Awake()
        {
            objectRenderer = GetComponent<Renderer>();
            if (subStep == null)
            {
                Debug.LogWarning($"TutorialSubStep reference is not set on ChangeMaterialOnComplete on {name}.");
            }
        }

        void OnEnable()
        {
            subStep.OnSubStepCompleted.AddListener(ChangeMaterial);
        }

        void OnDisable()
        {
            subStep.OnSubStepCompleted.RemoveListener(ChangeMaterial);
        }

        void OnDestroy()
        {
            subStep.OnSubStepCompleted.RemoveListener(ChangeMaterial);
        }


        private void ChangeMaterial()
        {
            if (objectRenderer != null && newMaterial != null)
            {
                objectRenderer.material = newMaterial;
            }
        }
    }
}