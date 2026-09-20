using System;
using System.Collections;
using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public abstract class TransformTweenConditionSource : ConditionSourceBase
    {
        [Flags]
        public enum Axes
        {
            X = 1 << 0,
            Y = 1 << 1,
            Z = 1 << 2
        }

        [Header("Transform Settings")]
        [SerializeField] protected Transform target;
        [SerializeField] protected Axes axes = Axes.Y;
        [SerializeField] protected Space space = Space.Self;
        [Tooltip("Seconds the animation takes. 0 snaps instantly.")]
        [SerializeField] protected float duration = 0.25f;

        [Header("Preview")]
        [Tooltip("Seconds the preview holds the end pose before reverting.")]
        [SerializeField] protected float previewHoldDuration = 1f;

        private Coroutine activeRoutine;
        private Rigidbody targetBody;

        protected Transform ActiveTarget => target != null ? target : transform;

        protected abstract float DefaultAmount { get; }

        protected virtual string UndoLabel => GetType().Name;
        protected abstract void BeginAnimation(float amount);
        protected abstract void ApplyProgress(float progress);
        protected abstract object CaptureRevertState();
        protected abstract void ApplyRevertState(object state);

        protected virtual void Reset() => target = transform;

        protected virtual void Awake()
        {
            if (target == null) target = transform;
            targetBody = target.GetComponent<Rigidbody>();
            if (targetBody != null) targetBody.isKinematic = true;
        }

        [ContextMenu("Release To Physics")]
        public void ReleaseToPhysics()
        {
            if (targetBody == null) return;

            targetBody.isKinematic = false;
            targetBody.useGravity = true;
        }

        [ContextMenu("Preview")]
        public void Preview() => Play(DefaultAmount, revertAfterwards: true);

        [ContextMenu("Reset Condition")]
        public void ResetCondition()
        {
            SetConditionState(false);
            ResetConditionActionGate();
        }

        protected void Play(float amount, bool revertAfterwards)
        {
            if (Application.isPlaying)
            {
                if (activeRoutine != null) StopCoroutine(activeRoutine);
                activeRoutine = StartCoroutine(AnimateRoutine(amount, revertAfterwards));
                return;
            }

#if UNITY_EDITOR
            EditorPlay(amount, revertAfterwards);
#endif
        }
        private void CompleteCondition()
        {
            SetConditionState(true);
            InvokeConditionAction();
        }

        private IEnumerator AnimateRoutine(float amount, bool revertAfterwards)
        {
            Transform animated = ActiveTarget;
            object revertState = CaptureRevertState();
            BeginAnimation(amount);

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                ApplyProgress(Mathf.Clamp01(elapsed / duration));
                yield return null;
            }

            ApplyProgress(1f);
            activeRoutine = null;

            if (!revertAfterwards)
            {
                CompleteCondition();
                yield break;
            }

            yield return new WaitForSeconds(previewHoldDuration);
            if (animated != null) ApplyRevertState(revertState);
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            StopEditorPlay();
#endif
        }

#if UNITY_EDITOR
        private readonly EditorPreviewDriver preview = new EditorPreviewDriver();

        private void EditorPlay(float amount, bool revertAfterwards)
        {
            StopEditorPlay();

            Transform animated = ActiveTarget;
            object revertState = CaptureRevertState();
            BeginAnimation(amount);

            if (!revertAfterwards) UnityEditor.Undo.RecordObject(animated, UndoLabel);

            preview.Play(
                duration,
                previewHoldDuration,
                revertAfterwards,
                isAlive: () => this != null && animated != null,
                apply: ApplyProgress,
                revert: () => ApplyRevertState(revertState),
                onCompleted: CompleteCondition);
        }

        private void StopEditorPlay() => preview.Stop();
#endif
    }
}
