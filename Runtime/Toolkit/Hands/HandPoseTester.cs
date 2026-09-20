using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class HandPoseTester : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private HandPoseDriver driver;

        [Tooltip("Disabled while this tester is enabled so live controller input does not overwrite the sliders.")]
        [SerializeField] private HandAnimator handAnimatorToSuppress;

        [Header("Curl (0 = open, 1 = fully curled)")]
        [Range(0f, 1f)][SerializeField] private float index;
        [Range(0f, 1f)][SerializeField] private float middle;
        [Range(0f, 1f)][SerializeField] private float ring;
        [Range(0f, 1f)][SerializeField] private float pinky;
        [Range(0f, 1f)][SerializeField] private float thumb;

        [Header("Pose Preview")]
        [Tooltip("When set, this pose overrides the sliders above, exactly as a grip pose would.")]
        [SerializeField] private HandPose livePose;

        private HandPose appliedPose;
        private bool suppressedAnimator;

        private void Reset()
        {
            driver = GetComponentInChildren<HandPoseDriver>();
            handAnimatorToSuppress = GetComponentInChildren<HandAnimator>();
        }

        private void Awake()
        {
            if (driver == null)
            {
                driver = GetComponentInChildren<HandPoseDriver>();
            }
        }

        private void OnEnable()
        {
            suppressedAnimator = handAnimatorToSuppress != null && handAnimatorToSuppress.enabled;

            if (suppressedAnimator)
            {
                handAnimatorToSuppress.enabled = false;
            }
        }

        private void OnDisable()
        {
            if (suppressedAnimator)
            {
                handAnimatorToSuppress.enabled = true;
                suppressedAnimator = false;
            }
        }

        private void Update()
        {
            if (driver == null) return;

            if (livePose != appliedPose)
            {
                appliedPose = livePose;

                if (livePose != null)
                {
                    driver.SetGripPose(livePose);
                }
                else
                {
                    driver.ClearGripPose();
                }
            }

            driver.SetBaseCurl(FingerType.Index, index);
            driver.SetBaseCurl(FingerType.Middle, middle);
            driver.SetBaseCurl(FingerType.Ring, ring);
            driver.SetBaseCurl(FingerType.Pinky, pinky);
            driver.SetBaseCurl(FingerType.Thumb, thumb);
        }
    }
}
