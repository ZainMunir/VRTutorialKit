using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class HingeAnimator : PreviewableAnimator
    {
        [Header("Hinge")]
        public Transform hinge;
        public float openAngle = 90f;

        public bool useYAxis = true;
        public bool useXAxis = false;
        public bool useZAxis = false;

        private float currentAngle = 0f;

        private Transform pivot;
        private Vector3 axis;
        private float startAngle;
        private float targetAngle;

        private struct PoseState
        {
            public Vector3 localPosition;
            public Quaternion localRotation;
            public float angle;
        }

        protected override string UndoLabel => "Hinge Animator";

        protected override bool IsAlive => this != null && pivot != null;

        public void Start()
        {
            if (hinge == null)
            {
                hinge = transform.parent;
                if (hinge == null)
                {
                    Debug.LogError("No parent found to use as hinge.");
                }
            }
        }

        protected override bool BeginAnimation(bool open)
        {
            pivot = hinge != null ? hinge : transform.parent;
            if (pivot == null)
            {
                Debug.LogError("No hinge assigned and no parent found to use as hinge.");
                return false;
            }

            axis = ResolveAxis();
            startAngle = currentAngle;
            targetAngle = open ? openAngle : 0f;
            return true;
        }

        private Vector3 ResolveAxis()
        {
            Vector3 resolved = Vector3.zero;
            if (useYAxis) resolved += Vector3.up;
            if (useXAxis) resolved += Vector3.right;
            if (useZAxis) resolved += Vector3.forward;
            return resolved;
        }

        protected override void ApplyProgress(float progress)
        {
            float smoothed = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(progress));
            float newAngle = Mathf.Lerp(startAngle, targetAngle, smoothed);

            transform.RotateAround(pivot.position, pivot.TransformDirection(axis), newAngle - currentAngle);
            currentAngle = newAngle;
        }

        protected override object CaptureRevertState() => new PoseState
        {
            localPosition = transform.localPosition,
            localRotation = transform.localRotation,
            angle = currentAngle
        };

        protected override void ApplyRevertState(object state)
        {
            PoseState pose = (PoseState)state;
            transform.localPosition = pose.localPosition;
            transform.localRotation = pose.localRotation;
            currentAngle = pose.angle;
        }
    }
}
