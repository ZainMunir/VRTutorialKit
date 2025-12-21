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
        public bool useXAxis = false;

        [Header("Events")]
        public UnityEvent OnOpened;
        public UnityEvent OnClosed;

        private bool isOpen = false;
        private bool isAnimating = false;
        private float currentAngle = 0f;

        public void Toggle() => SetState(!isOpen);

        public void SetState(bool open)
        {
            if (isAnimating || isOpen == open) return;
            isOpen = open;
            StartCoroutine(Animate());
        }

        private IEnumerator Animate()
        {
            isAnimating = true;
            if (isOpen) OnOpened.Invoke(); else OnClosed.Invoke();
            float targetAngle = isOpen ? openAngle : 0f;
            float startAngle = currentAngle;
            float duration = 1f / speed;
            float elapsed = 0f;

            Vector3 axis = useXAxis ? Vector3.right : Vector3.up;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float progress = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                float newAngle = Mathf.Lerp(startAngle, targetAngle, progress);

                transform.RotateAround(hinge.position, hinge.TransformDirection(axis), newAngle - currentAngle);
                currentAngle = newAngle;
                yield return null;
            }

            isAnimating = false;
        }
    }
}