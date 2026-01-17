using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class ProgressDrivenEffect : MonoBehaviour
    {
        [SerializeField] private ProgressProvider progressProvider;
        [SerializeField] private ProgressEffect[] effects;

        private void Update()
        {
            if (progressProvider == null) return;

            float progress = progressProvider.Progress;
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