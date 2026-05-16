using UnityEngine;
using System.Collections.Generic;

namespace ECDA.VRTutorialKit
{
    public class InteractionController : MonoBehaviour
    {
        TutorialManager tutorialManager;
        private List<GameObject> instantiatedObjects = new List<GameObject>();


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
                GameObject instance = Instantiate(interactionPrefab, transform);
                Transform[] allTransforms = instance.GetComponentsInChildren<Transform>(true);
                foreach (Transform t in allTransforms)
                {
                    instantiatedObjects.Add(t.gameObject);
                }
            }
        }

        void RemoveAllPrefabs()
        {
            foreach (var obj in instantiatedObjects)
            {
                if (obj != null)
                {
                    Destroy(obj);
                }
            }
            instantiatedObjects.Clear();
        }
    }
}