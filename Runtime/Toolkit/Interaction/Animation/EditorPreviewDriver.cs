#if UNITY_EDITOR
using System;
using UnityEngine;

namespace ECDA.VRTutorialKit
{
    /// <summary>
    /// Runs a timed preview off <see cref="UnityEditor.EditorApplication.update"/>, so a component
    /// can animate outside play mode where coroutines are unavailable.
    ///
    /// The driver owns the scheduling — elapsed time, progress, scene repaints, the hold before
    /// reverting, and unsubscribing — while the caller supplies what to apply and how to undo it.
    /// </summary>
    internal sealed class EditorPreviewDriver
    {
        private UnityEditor.EditorApplication.CallbackFunction step;

        public bool IsRunning => step != null;

        /// <param name="duration">Seconds the animation runs for. Zero snaps straight to the end pose.</param>
        /// <param name="holdDuration">Seconds to hold the end pose before reverting. Ignored unless <paramref name="revertAfterwards"/>.</param>
        /// <param name="revertAfterwards">Restore the starting pose once the hold elapses, rather than keeping the end pose.</param>
        /// <param name="isAlive">Checked each tick; returning false stops the preview. Guards against the object being destroyed mid-preview.</param>
        /// <param name="apply">Receives progress in 0..1. Any easing is the caller's business.</param>
        /// <param name="revert">Restores the pose the caller captured before starting.</param>
        /// <param name="onCompleted">
        /// Runs once the animation reaches its end pose and that pose is being kept, i.e. only when
        /// <paramref name="revertAfterwards"/> is false. This is the commit point for callers whose
        /// side effects belong at the end of the animation rather than at its start; callers that
        /// fire theirs up front simply leave it null.
        /// </param>
        /// <param name="onStopped">Runs whenever the preview stops, however it stops, including when abandoned.</param>
        public void Play(
            float duration,
            float holdDuration,
            bool revertAfterwards,
            Func<bool> isAlive,
            Action<float> apply,
            Action revert,
            Action onCompleted = null,
            Action onStopped = null)
        {
            Stop();

            double startTime = UnityEditor.EditorApplication.timeSinceStartup;
            step = () =>
            {
                if (!isAlive())
                {
                    Stop();
                    onStopped?.Invoke();
                    return;
                }

                float elapsed = (float)(UnityEditor.EditorApplication.timeSinceStartup - startTime);
                float progress = duration > 0f ? Mathf.Clamp01(elapsed / duration) : 1f;
                apply(progress);
                UnityEditor.SceneView.RepaintAll();

                if (progress < 1f) return;

                if (revertAfterwards)
                {
                    if (elapsed < duration + holdDuration) return;
                    revert();
                    UnityEditor.SceneView.RepaintAll();
                }
                else
                {
                    onCompleted?.Invoke();
                }

                Stop();
                onStopped?.Invoke();
            };

            UnityEditor.EditorApplication.update += step;
        }

        public void Stop()
        {
            if (step == null) return;

            UnityEditor.EditorApplication.update -= step;
            step = null;
        }
    }
}
#endif
