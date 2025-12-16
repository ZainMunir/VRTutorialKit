using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class InteractionController : MonoBehaviour
    {
        TutorialManager tutorialManager;


        void Start()
        {
            tutorialManager = TutorialManager.Instance;

            if (tutorialManager == null)
            {
                Debug.LogError("TutorialManager instance not found.");
                return;
            }

            tutorialManager.OnTutorialStepChanged += OnTutorialStepChanged;
            UpdatePrefabsForCurrentStep();
        }

        void OnTutorialStepChanged(bool stepCompleted)
        {
            UpdatePrefabsForCurrentStep();
        }

        void UpdatePrefabsForCurrentStep()
        {
            var step = tutorialManager.GetCurrentStep();
            if (step == null) return;

            RemoveAllPrefabs();

            GameObject interactionPrefab = step.interactionPrefab;
            if (interactionPrefab != null)
            {
                Instantiate(interactionPrefab, transform);
            }
        }

        void RemoveAllPrefabs()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}