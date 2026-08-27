using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class ProgressDrivenEffect : MonoBehaviour
    {
        [SerializeField, RequireInterface(typeof(ProgressProvider))] private MonoBehaviour progressProvider;
        [SerializeField] private ProgressEffect[] effects;

        private ProgressProvider cachedProvider;

        private void Awake()
        {
            if (progressProvider == null)
            {
                Debug.LogError($"{nameof(ProgressDrivenEffect)} on {name} is missing a progress provider reference.", this);
                return;
            }

            cachedProvider = progressProvider as ProgressProvider;
            if (cachedProvider == null)
            {
                Debug.LogError($"{nameof(ProgressDrivenEffect)} on {name} requires a component implementing {nameof(ProgressProvider)}.", this);
            }
        }

        private void Update()
        {
            if (cachedProvider == null) return;

            float progress = cachedProvider.Progress;
            foreach (var effect in effects)
            {
                if (effect != null)
                {
                    effect.ApplyProgress(progress);
                }
            }
        }
    }
}