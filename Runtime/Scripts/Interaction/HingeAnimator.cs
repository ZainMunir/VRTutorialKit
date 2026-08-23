using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace ECDA.VRTutorialKit
{
    public class HingeAnimator : MonoBehaviour
    {
        [Header("Settings")]
        public Transform hinge;
        public float openAngle = 90f;
        public float speed = 2f;

        public bool useYAxis = true;
        public bool useXAxis = false;
        public bool useZAxis = false;

        [Header("Preview")]
        [Tooltip("Seconds the preview holds the open pose before reverting.")]
        public float previewHoldDuration = 1f;

        [Header("Events")]
        public UnityEvent OnOpened;
        public UnityEvent OnClosed;

        private bool isOpen = false;
        private bool isAnimating = false;
        private float currentAngle = 0f;

        private Coroutine activeRoutine;

        private struct PoseState
        {
            public Vector3 localPosition;
            public Quaternion localRotation;
            public float angle;
        }

        public void Start()
        {
            if (hinge == null)
            {
                // Debug.LogWarning("Hinge Transform is not assigned. Using parent if available.");
                hinge = transform.parent;
                if (hinge == null)
                {
                    Debug.LogError("No parent found to use as hinge.");
                }
            }
        }

        public void Toggle() => SetState(!isOpen);

        [ContextMenu("Preview")]
        public void Preview() => Play(true, revertAfterwards: true);

        public void SetState(bool open)
        {
            if (isAnimating || isOpen == open) return;
            isOpen = open;
            Play(open, revertAfterwards: false);
        }

        private void Play(bool open, bool revertAfterwards)
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

        private IEnumerator AnimateRoutine(bool open, bool revertAfterwards)
        {
            Transform pivot = ResolveHinge();
            if (pivot == null)
            {
                Debug.LogError("No hinge assigned and no parent found to use as hinge.");
                yield break;
            }

            PoseState revertState = CapturePose();
            Vector3 axis = ResolveAxis();
            float startAngle = currentAngle;
            float targetAngle = open ? openAngle : 0f;
            float duration = speed > 0f ? 1f / speed : 0f;

            isAnimating = true;
            if (!revertAfterwards)
            {
                if (open) OnOpened.Invoke(); else OnClosed.Invoke();
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                ApplyProgress(pivot, axis, startAngle, targetAngle, elapsed / duration);
                yield return null;
            }

            ApplyProgress(pivot, axis, startAngle, targetAngle, 1f);
            isAnimating = false;
            activeRoutine = null;

            if (!revertAfterwards) yield break;

            yield return new WaitForSeconds(previewHoldDuration);
            ApplyPose(revertState);
        }

        private Transform ResolveHinge() => hinge != null ? hinge : transform.parent;

        private Vector3 ResolveAxis()
        {
            Vector3 axis = Vector3.zero;
            if (useYAxis) axis += Vector3.up;
            if (useXAxis) axis += Vector3.right;
            if (useZAxis) axis += Vector3.forward;
            return axis;
        }

        private void ApplyProgress(Transform pivot, Vector3 axis, float startAngle, float targetAngle, float progress)
        {
            float smoothed = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(progress));
            float newAngle = Mathf.Lerp(startAngle, targetAngle, smoothed);

            transform.RotateAround(pivot.position, pivot.TransformDirection(axis), newAngle - currentAngle);
            currentAngle = newAngle;
        }

        private PoseState CapturePose() => new PoseState
        {
            localPosition = transform.localPosition,
            localRotation = transform.localRotation,
            angle = currentAngle
        };

        private void ApplyPose(PoseState state)
        {
            transform.localPosition = state.localPosition;
            transform.localRotation = state.localRotation;
            currentAngle = state.angle;
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            StopEditorPlay();
#endif
        }

#if UNITY_EDITOR
        private UnityEditor.EditorApplication.CallbackFunction editorStep;

        private void EditorPlay(bool open, bool revertAfterwards)
        {
            Transform pivot = ResolveHinge();
            if (pivot == null)
            {
                Debug.LogError("No hinge assigned and no parent found to use as hinge.");
                return;
            }

            StopEditorPlay();

            PoseState revertState = CapturePose();
            Vector3 axis = ResolveAxis();
            float startAngle = currentAngle;
            float targetAngle = open ? openAngle : 0f;
            float duration = speed > 0f ? 1f / speed : 0f;

            isAnimating = true;
            if (!revertAfterwards)
            {
                // The pose survives the animation, so keep it undoable.
                UnityEditor.Undo.RecordObject(transform, "Hinge Animator");
                if (open) OnOpened.Invoke(); else OnClosed.Invoke();
            }

            double startTime = UnityEditor.EditorApplication.timeSinceStartup;
            editorStep = () =>
            {
                if (this == null || pivot == null)
                {
                    StopEditorPlay();
                    return;
                }

                float elapsed = (float)(UnityEditor.EditorApplication.timeSinceStartup - startTime);
                float progress = duration > 0f ? Mathf.Clamp01(elapsed / duration) : 1f;
                ApplyProgress(pivot, axis, startAngle, targetAngle, progress);
                UnityEditor.SceneView.RepaintAll();

                if (progress < 1f) return;

                if (revertAfterwards)
                {
                    if (elapsed < duration + previewHoldDuration) return;
                    ApplyPose(revertState);
                    UnityEditor.SceneView.RepaintAll();
                }

                StopEditorPlay();
            };

            UnityEditor.EditorApplication.update += editorStep;
        }

        private void StopEditorPlay()
        {
            isAnimating = false;
            if (editorStep == null) return;

            UnityEditor.EditorApplication.update -= editorStep;
            editorStep = null;
        }
#endif
    }
}
