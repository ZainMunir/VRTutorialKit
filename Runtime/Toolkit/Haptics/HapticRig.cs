using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Haptics;

namespace ECDA.VRTutorialKit
{
    public class HapticRig : SingletonBehaviour<HapticRig>
    {
        [SerializeField] private HapticImpulsePlayer left;
        [SerializeField] private HapticImpulsePlayer right;

        public void Play(HapticHand hand, float amplitude, float duration)
        {
            amplitude = Mathf.Clamp01(amplitude);

            if (hand != HapticHand.Right) Send(left, amplitude, duration);
            if (hand != HapticHand.Left) Send(right, amplitude, duration);
        }

        private void Send(HapticImpulsePlayer player, float amplitude, float duration)
        {
            if (player == null)
            {
                Debug.LogWarning($"[{name}] Missing HapticImpulsePlayer reference.", this);
                return;
            }
            player.SendHapticImpulse(amplitude, duration);
        }
    }
}
