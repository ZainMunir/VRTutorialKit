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

        public void SetState(bool open)
        {
            if (isAnimating || isOpen == open) return;
            CalculateOpenPosition(); // Recalculate based on current inspector values
            isOpen = open;
            StartCoroutine(Animate());
        }

        private IEnumerator Animate()
        {
            isAnimating = true;
            if (isOpen) OnOpened.Invoke(); else OnClosed.Invoke();
            Vector3 targetPosition = isOpen ? openPosition : closedPosition;
            Vector3 startPosition = transform.localPosition;
            float duration = 1f / speed;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                transform.localPosition = Vector3.Lerp(startPosition, targetPosition, progress);
                yield return null;
            }

            isAnimating = false;
        }
    }
}