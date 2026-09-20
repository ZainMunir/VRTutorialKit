using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class SlidingAnimator : PreviewableAnimator
    {
        [Header("Sliding")]
        public Vector3 openDirection = Vector3.forward;
        [Range(0, 1)] public float openFraction = 0.6f;

        private Vector3 closedPosition;
        private Vector3 openPosition;
        private Vector3 startPosition;
        private Vector3 targetPosition;

        protected override string UndoLabel => "Sliding Animator";

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

        protected override bool BeginAnimation(bool open)
        {
            // Outside play mode Start() has never run, and the object is sitting at its authored
            // pose, so that pose is the closed one.
            if (!Application.isPlaying) closedPosition = transform.localPosition;

            CalculateOpenPosition();
            startPosition = transform.localPosition;
            targetPosition = open ? openPosition : closedPosition;
            return true;
        }

        protected override void ApplyProgress(float progress)
        {
            float smoothed = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(progress));
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, smoothed);
        }

        protected override object CaptureRevertState() => transform.localPosition;

        protected override void ApplyRevertState(object state) => transform.localPosition = (Vector3)state;
    }
}
