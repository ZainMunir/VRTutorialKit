using UnityEngine;
using UnityEngine.Events;

namespace ECDA.VRTutorialKit
{
    public class EffectDuringChange : ProgressEffect
    {
        [Tooltip("Progress per second below which the change counts as stopped.")]
        [SerializeField] private float progressRateThreshold = 0.05f;
        [Tooltip("Seconds below the threshold before the stop is reported.")]
        [SerializeField] private float stopDelay = 0.1f;

        public UnityEvent onProgressStarted;
        public UnityEvent onProgressStopped;
        public UnityEvent onPositiveProgressStarted;
        public UnityEvent onPositiveProgressStopped;
        public UnityEvent onNegativeProgressStarted;
        public UnityEvent onNegativeProgressStopped;

        private float lastProgress;
        private float idleTime;
        private bool hasLastProgress;
        private bool isProgressing;
        private int progressDirection;

        public bool IsProgressing => isProgressing;
        public int ProgressDirection => progressDirection;

        private void OnEnable()
        {
            hasLastProgress = false;
            idleTime = 0f;
        }

        private void OnDisable() => SetProgressing(false, 0);

        public override void ApplyProgress(float progress)
        {
            // The first value seen is a baseline, not a change.
            if (!hasLastProgress)
            {
                lastProgress = progress;
                hasLastProgress = true;
                return;
            }

            float dt = Time.deltaTime;
            float delta = progress - lastProgress;
            float rate = dt > 0f ? Mathf.Abs(delta) / dt : 0f;
            lastProgress = progress;

            if (rate > progressRateThreshold)
            {
                idleTime = 0f;
                SetProgressing(true, delta > 0f ? 1 : -1);
            }
            else
            {
                idleTime += dt;
                if (idleTime >= stopDelay) SetProgressing(false, 0);
            }
        }

        private void SetProgressing(bool value, int direction)
        {
            if (isProgressing == value && progressDirection == direction) return;

            if (progressDirection != direction)
            {
                if (progressDirection > 0) onPositiveProgressStopped?.Invoke();
                else if (progressDirection < 0) onNegativeProgressStopped?.Invoke();
            }

            bool wasProgressing = isProgressing;
            isProgressing = value;
            int previousDirection = progressDirection;
            progressDirection = direction;

            if (value != wasProgressing)
            {
                if (value) onProgressStarted?.Invoke();
                else onProgressStopped?.Invoke();
            }

            if (direction != previousDirection)
            {
                if (direction > 0) onPositiveProgressStarted?.Invoke();
                else if (direction < 0) onNegativeProgressStarted?.Invoke();
            }
        }
    }
}
