using UnityEngine;

namespace ECDA.VRTutorialKit
{
    [RequireComponent(typeof(RigController))]
    public class TutorialRigController : MonoBehaviour
    {
        private TutorialManager tutorialManager;
        [SerializeField] private RigController rigController;

        void Start()
        {
            tutorialManager = TutorialManager.Instance;
            if (tutorialManager == null)
            {
                Debug.LogError("TutorialManager instance not found.");
                return;
            }
            rigController = GetComponent<RigController>();
            if (rigController == null)
            {
                Debug.LogError("RigController component not found on the same GameObject.");
                return;
            }
            tutorialManager.OnTutorialStepChanged += OnTutorialStepChanged;
        }

        void OnTutorialStepChanged(bool stepCompleted)
        {
            var step = tutorialManager.GetCurrentStep();
            if (step == null) return;

            rigController.SetEnableSnapTurn(step.enableSnapTurn);
            rigController.SetLeftHandLocomotion(step.leftHandLocomotion);
            rigController.SetRightHandLocomotion(step.rightHandLocomotion);

            rigController.UpdateLocomotionControls();
        }

        void OnDestroy()
        {
            if (tutorialManager != null)
            {
                tutorialManager.OnTutorialStepChanged -= OnTutorialStepChanged;
            }
        }
    }
}