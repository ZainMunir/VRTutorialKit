using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class HapticCue : MonoBehaviour
    {
        [SerializeField] private HapticHand hand = HapticHand.Both;
        [SerializeField, Range(0f, 1f)] private float amplitude = 0.5f;
        [SerializeField, Min(0f)] private float duration = 0.1f;

        public void Play()
        {
            if (HapticRig.Instance == null)
            {
                Debug.LogWarning($"[{name}] No HapticRig in the scene.", this);
                return;
            }
            HapticRig.Instance.Play(hand, amplitude, duration);
        }
    }
}
