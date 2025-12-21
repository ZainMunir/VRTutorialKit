using UnityEngine;

namespace ECDA.VRTutorialKit
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioTrigger : MonoBehaviour
    {
        private AudioSource source;

        void Awake() => source = GetComponent<AudioSource>();

        public void PlayClip(AudioClip clip)
        {
            if (clip) source.PlayOneShot(clip);
        }
    }
}