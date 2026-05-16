using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class ProgressFlipper : MonoBehaviour, ProgressProvider
    {
        private float progress = 0f;
        public float Progress => progress;
        public void FlipProgress()
        {
            Debug.Log($"Flipping progress from {progress} to {1f - progress}");
            progress = 1f - progress;
        }
    }
}