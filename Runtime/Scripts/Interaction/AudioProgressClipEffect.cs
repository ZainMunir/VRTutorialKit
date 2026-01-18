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

            // 1. If progress is reset to 0, stop the sound so it can restart later
            if (progress <= 0)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
                return;
            }

            // 2. If progress starts and we aren't playing yet, Play!
            if (progress >= startThreshold && !audioSource.isPlaying)
            {
                audioSource.Play();
            }

            // 3. Optional: Stop the sound immediately if they finish 
            // (Standard if you have a separate "Success" chime that plays at 1.0)
            if (progress >= 1f && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}