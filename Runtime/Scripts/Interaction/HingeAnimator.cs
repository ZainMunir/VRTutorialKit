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

        [Header("Events")]
        public UnityEvent OnOpened;
        public UnityEvent OnClosed;

        private bool isOpen = false;
        private bool isAnimating = false;
        private float currentAngle = 0f;

        public void Start()
        {
            if (hinge == null)
            {
                Debug.LogWarning("Hinge Transform is not assigned. Using parent if available.");
                hinge = transform.parent;
                if (hinge == null)
                {
                    Debug.LogError("No parent found to use as hinge.");
                }
            }
        }

        [ContextMenu("Toggle")]
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

            Vector3 axis = Vector3.zero;
            if (useYAxis) axis += Vector3.up;
            if (useXAxis) axis += Vector3.right;
            if (useZAxis) axis += Vector3.forward;

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