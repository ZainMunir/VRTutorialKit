using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class CompleteOnRecenter : MonoBehaviour
    {
        [SerializeField] private TutorialSubStep subStep;

        [SerializeField] private XRRecenterDetector recenterDetector;

        private void Awake()
        {
            if (recenterDetector != null)
            {
                recenterDetector.OnRecenterDetected.AddListener(OnRecenter);
            }
            else
            {
                Debug.LogError($"XRRecenterDetector not found in scene for {name}.");
            }
            if (subStep == null)
            {
                Debug.LogError($"{nameof(subStep)} reference is not set on {name}.");
            }
        }

        private void OnDestroy()
        {
            if (recenterDetector != null)
            {
                recenterDetector.OnRecenterDetected.RemoveListener(OnRecenter);
            }
        }

        private void OnRecenter()
        {
            if (subStep != null && !subStep.IsCompleted)
            {
                subStep.Complete();
            }
        }
    }
}