using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class ProgressDrivenEffect : MonoBehaviour
    {
        [SerializeField, RequireInterface(typeof(IProgressProvider))] private MonoBehaviour progressProvider;
        [SerializeField] private ProgressEffect[] effects;

        private IProgressProvider cachedProvider;

        private void Awake()
        {
            if (progressProvider == null)
            {
                Debug.LogError($"{nameof(ProgressDrivenEffect)} on {name} is missing a progress provider reference.", this);
                return;
            }

            cachedProvider = progressProvider as IProgressProvider;
            if (cachedProvider == null)
            {
                Debug.LogError($"{nameof(ProgressDrivenEffect)} on {name} requires a component implementing {nameof(IProgressProvider)}.", this);
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