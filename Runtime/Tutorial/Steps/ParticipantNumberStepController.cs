using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class ParticipantNumberStepController : MonoBehaviour
    {
        [SerializeField] private NumberPad numberPad;
        [SerializeField] private TutorialSubStep tutorialSubStep;

        void OnEnable()
        {
            numberPad.onSubmitted.AddListener(OnSubmitted);
        }

        void OnDisable()
        {
            numberPad.onSubmitted.RemoveListener(OnSubmitted);
        }

        // Submitting again while the step is open replaces the number, so a typo can be corrected.
        void OnSubmitted(string number)
        {
            if (ParticipantProfile.Instance != null)
            {
                ParticipantProfile.Instance.SetParticipantNumber(number);
            }
            else
            {
                Debug.LogError("ParticipantProfile instance not found; the participant number was not recorded.");
            }

            tutorialSubStep.Complete();
        }
    }
}
