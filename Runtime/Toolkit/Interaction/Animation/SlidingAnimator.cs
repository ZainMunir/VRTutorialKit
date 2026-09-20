using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace ECDA.VRTutorialKit
{
    public class SlidingAnimator : MonoBehaviour
    {
        [Header("Settings")]
        public Vector3 openDirection = Vector3.forward;
        [Range(0, 1)] public float openFraction = 0.6f;
        public float speed = 2f;

        [Header("Preview")]
        [Tooltip("Seconds the preview holds the open pose before reverting.")]
        public float previewHoldDuration = 1f;

        [Header("Events")]
        public UnityEvent OnOpened;
        public UnityEvent OnClosed;
        private bool isOpen = false;
        private bool isAnimating = false;
        private Vector3 closedPosition;
        private Vector3 openPosition;

        void Start()
        {
            closedPosition = transform.localPosition;
            CalculateOpenPosition();
        }

        private void CalculateOpenPosition()
        {
            Renderer renderer = GetComponent<Renderer>();
            float length = 1f; // default
            if (renderer != null)
            {
                Vector3 size = renderer.bounds.size;
                length = Mathf.Abs(Vector3.Dot(size, openDirection.normalized));
            }
            else
            {
                Debug.LogWarning("No Renderer found on drawer. Using default length of 1 unit.");
            }
            openPosition = closedPosition + openDirection.normalized * (length * openFraction);
        }

        public void Toggle() => SetState(!isOpen);

        [ContextMenu("Preview")]
        public void Preview() => Play(true, revertAfterwards: true);

        public void SetState(bool open)
        {
            if (isAnimating || isOpen == open) return;
            CalculateOpenPosition(); // Recalculate based on current inspector values
            isOpen = open;
            Play(open, revertAfterwards: false);
        }

        private void Play(bool open, bool revertAfterwards)
        {
            if (Application.isPlaying)
            {
                StartCoroutine(Animate(open, revertAfterwards));
                return;
            }

#if UNITY_EDITOR
            EditorPlay(open, revertAfterwards);
#endif
        }

        private IEnumerator Animate(bool open, bool revertAfterwards)
        {
            isAnimating = true;
            if (!revertAfterwards)
            {
                if (open) OnOpened.Invoke(); else OnClosed.Invoke();
            }

            Vector3 targetPosition = open ? openPosition : closedPosition;
            Vector3 startPosition = transform.localPosition;
            float duration = speed > 0f ? 1f / speed : 0f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                transform.localPosition = Vector3.Lerp(startPosition, targetPosition, progress);
                yield return null;
            }

            isAnimating = false;

            if (revertAfterwards)
            {
                yield return new WaitForSeconds(previewHoldDuration);
                transform.localPosition = startPosition;
            }
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
            StopEditorPlay();

            Vector3 revertPosition = transform.localPosition;
            closedPosition = revertPosition;
            CalculateOpenPosition();

            isAnimating = true;
            Vector3 targetPosition = open ? openPosition : closedPosition;
            float duration = speed > 0f ? 1f / speed : 0f;
            double startTime = UnityEditor.EditorApplication.timeSinceStartup;

            editorStep = () =>
            {
                if (this == null)
                {
                    StopEditorPlay();
                    return;
                }

                float elapsed = (float)(UnityEditor.EditorApplication.timeSinceStartup - startTime);
                float progress = duration > 0f ? Mathf.Clamp01(elapsed / duration) : 1f;
                float smoothed = Mathf.SmoothStep(0f, 1f, progress);
                transform.localPosition = Vector3.Lerp(revertPosition, targetPosition, smoothed);
                UnityEditor.SceneView.RepaintAll();

                if (progress < 1f) return;

                if (revertAfterwards)
                {
                    if (elapsed < duration + previewHoldDuration) return;
                    transform.localPosition = revertPosition;
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