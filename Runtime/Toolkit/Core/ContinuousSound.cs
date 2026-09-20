using UnityEngine;

namespace ECDA.VRTutorialKit
{

    [RequireComponent(typeof(AudioSource))]
    public class ContinuousSound : MonoBehaviour
    {
        public AudioClip soundClip;
        [Range(0f, 1f)] public float volume = 0.5f;

        private AudioSource audioSource;

        public bool IsPlaying => audioSource != null && audioSource.isPlaying;

        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = soundClip;
            audioSource.loop = true;
            audioSource.volume = volume;
        }

        [ContextMenu("Play Sound")]
        public void PlaySound()
        {
            if (!IsPlaying) audioSource.Play();
        }

        [ContextMenu("Stop Sound")]
        public void StopSound()
        {
            if (IsPlaying) audioSource.Stop();
        }

        public void ToggleSound()
        {
            if (IsPlaying) StopSound();
            else PlaySound();
        }

        public void SetPlaying(bool shouldPlay)
        {
            if (shouldPlay) PlaySound();
            else StopSound();
        }
    }
}
