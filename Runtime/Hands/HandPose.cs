using UnityEngine;

namespace ECDA.VRTutorialKit
{

    [CreateAssetMenu(fileName = "HandPose", menuName = "VRTutorialKit/Hand Pose")]
    public class HandPose : ScriptableObject
    {
        [Header("Finger Curl (0 = open, 1 = fully curled)")]
        [Range(0f, 1f)][SerializeField] private float index;
        [Range(0f, 1f)][SerializeField] private float middle;
        [Range(0f, 1f)][SerializeField] private float ring;
        [Range(0f, 1f)][SerializeField] private float pinky;
        [Range(0f, 1f)][SerializeField] private float thumb;

        [Header("Transition")]
        [Tooltip("Curl units per second. Leave at 0 to use the driver's default curl speed.")]
        [Min(0f)][SerializeField] private float transitionSpeed;

        public float TransitionSpeed => transitionSpeed;

        public float GetCurl(FingerType finger)
        {
            switch (finger)
            {
                case FingerType.Index: return index;
                case FingerType.Middle: return middle;
                case FingerType.Ring: return ring;
                case FingerType.Pinky: return pinky;
                case FingerType.Thumb: return thumb;
                default: return 0f;
            }
        }

        public void CopyTo(float[] target)
        {
            if (target == null || target.Length < FingerConstants.Count) return;

            for (int i = 0; i < FingerConstants.Count; i++)
            {
                target[i] = GetCurl((FingerType)i);
            }
        }
    }
}
