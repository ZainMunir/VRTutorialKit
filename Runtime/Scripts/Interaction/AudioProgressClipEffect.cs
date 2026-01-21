using UnityEngine;

namespace ECDA.VRTutorialKit
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioProgressClipEffect : ProgressEffect
    {
        [SerializeField] private AudioSource audioSource;

        [SerializeField] private float startThreshold = 0.01f;

        private void Reset()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.playOnAwake = false;
                audioSource.loop = false;
            }
        }

        public override void ApplyProgress(float progress)
        {
            if (audioSource == null) return;

            if (progress <= 0)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
                return;
            }

            if (progress >= startThreshold && !audioSource.isPlaying)
            {
                audioSource.Play();
            }

            if (progress >= 1f && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}