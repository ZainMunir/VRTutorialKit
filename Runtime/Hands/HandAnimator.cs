using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ECDA.VRTutorialKit
{
    public class HandAnimator : MonoBehaviour
    {
        [SerializeField] private InputActionReference mainTrigger;
        [SerializeField] private InputActionReference secondaryTrigger;
        [SerializeField] private NearFarInteractor hoverInteractor;
        [SerializeField] private HandPoseDriver driver;

        [Header("Hover")]
        [SerializeField] private HandPose hoverPose;

        private void Reset()
        {
            driver = GetComponent<HandPoseDriver>();
            hoverInteractor = GetComponentInParent<NearFarInteractor>();
        }

        private void Update()
        {
            if (driver == null) return;

            float triggerCurl = ReadAxis(mainTrigger);
            float gripCurl = ReadAxis(secondaryTrigger);

            bool hasHover = hoverInteractor != null && hoverInteractor.hasHover;
            HandPose floor = hasHover ? hoverPose : null;

            ApplyCurl(FingerType.Index, triggerCurl, floor);
            ApplyCurl(FingerType.Middle, gripCurl, floor);
            ApplyCurl(FingerType.Ring, gripCurl, floor);
            ApplyCurl(FingerType.Pinky, gripCurl, floor);
            ApplyCurl(FingerType.Thumb, gripCurl, floor);
        }

        private void ApplyCurl(FingerType finger, float inputCurl, HandPose floor)
        {
            float value = floor != null
                ? Mathf.Max(inputCurl, floor.GetCurl(finger))
                : inputCurl;

            driver.SetBaseCurl(finger, value);
        }

        private static float ReadAxis(InputActionReference reference)
        {
            if (reference == null || reference.action == null) return 0f;
            return Mathf.Clamp01(reference.action.ReadValue<float>());
        }
    }
}
