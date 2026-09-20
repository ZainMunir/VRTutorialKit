using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ECDA.VRTutorialKit
{
    /// <summary>
    /// Shared sequencing for components that animate a transform between a closed and an open
    /// pose: the same animation runs from a coroutine in play mode and from
    /// <see cref="EditorPreviewDriver"/> in the editor, so a pose can be previewed and reverted
    /// while authoring.
    ///
    /// Subclasses decide what a pose is and how to interpolate towards it; everything about
    /// sequencing — timing, easing hand-off, events, undo, the preview hold and revert, and not
    /// letting two animations run at once — lives here.
    /// </summary>
    public abstract class PreviewableAnimator : MonoBehaviour
    {
        [Header("Animation")]
        [Tooltip("Animations per second: a pose change takes 1/speed seconds. 0 snaps instantly.")]
        public float speed = 2f;

        [Header("Preview")]
        [Tooltip("Seconds the preview holds the open pose before reverting.")]
        public float previewHoldDuration = 1f;

        [Header("Events")]
        public UnityEvent OnOpened;
        public UnityEvent OnClosed;

        protected bool isOpen = false;
        protected bool isAnimating = false;

        private Coroutine activeRoutine;

        protected float Duration => speed > 0f ? 1f / speed : 0f;

        /// <summary>Label for the editor undo entry when a pose is kept rather than reverted.</summary>
        protected abstract string UndoLabel { get; }

        /// <summary>
        /// Resolve whatever the animation needs and cache the start and end poses. Return false to
        /// abort, having reported why. Called before every animation, so inspector edits take effect.
        /// </summary>
        protected abstract bool BeginAnimation(bool open);

        /// <summary>Apply linear progress in 0..1. Easing is the subclass's business.</summary>
        protected abstract void ApplyProgress(float progress);

        protected abstract object CaptureRevertState();
        protected abstract void ApplyRevertState(object state);

        /// <summary>False once the animation can no longer be applied, which stops a preview.</summary>
        protected virtual bool IsAlive => this != null;

        public void Toggle() => SetState(!isOpen);

        [ContextMenu("Preview")]
        public void Preview() => Play(true, revertAfterwards: true);

        public void SetState(bool open)
        {
            if (isAnimating || isOpen == open) return;
            isOpen = open;
            Play(open, revertAfterwards: false);
        }

        protected void Play(bool open, bool revertAfterwards)
        {
            if (Application.isPlaying)
            {
                if (activeRoutine != null) StopCoroutine(activeRoutine);
                activeRoutine = StartCoroutine(AnimateRoutine(open, revertAfterwards));
                return;
            }

#if UNITY_EDITOR
            EditorPlay(open, revertAfterwards);
#endif
        }

        private void InvokeStateEvent(bool open)
        {
            if (open) OnOpened.Invoke(); else OnClosed.Invoke();
        }

        private IEnumerator AnimateRoutine(bool open, bool revertAfterwards)
        {
            if (!BeginAnimation(open))
            {
                activeRoutine = null;
                yield break;
            }

            object revertState = CaptureRevertState();
            float duration = Duration;

            isAnimating = true;
            if (!revertAfterwards) InvokeStateEvent(open);

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                ApplyProgress(Mathf.Clamp01(elapsed / duration));
                yield return null;
            }

            ApplyProgress(1f);
            isAnimating = false;
            activeRoutine = null;

            if (!revertAfterwards) yield break;

            yield return new WaitForSeconds(previewHoldDuration);
            ApplyRevertState(revertState);
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            StopEditorPlay();
#endif
        }

#if UNITY_EDITOR
        private readonly EditorPreviewDriver preview = new EditorPreviewDriver();

        private void EditorPlay(bool open, bool revertAfterwards)
        {
            StopEditorPlay();

            if (!BeginAnimation(open)) return;

            object revertState = CaptureRevertState();

            isAnimating = true;
            if (!revertAfterwards)
            {
                // The pose survives the animation, so keep it undoable.
                UnityEditor.Undo.RecordObject(transform, UndoLabel);
                InvokeStateEvent(open);
            }

            preview.Play(
                Duration,
                previewHoldDuration,
                revertAfterwards,
                isAlive: () => IsAlive,
                apply: ApplyProgress,
                revert: () => ApplyRevertState(revertState),
                onStopped: StopEditorPlay);
        }

        private void StopEditorPlay()
        {
            isAnimating = false;
            preview.Stop();
        }
#endif
    }
}
