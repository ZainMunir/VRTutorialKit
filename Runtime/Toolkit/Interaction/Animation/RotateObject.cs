using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class RotateObject : TransformTweenConditionSource
    {
        [SerializeField] private float degrees = 90f;

        private Quaternion animationStartLocalRotation;
        private Quaternion animationParentRotation;
        private Vector3 animationEuler;

        protected override float DefaultAmount => degrees;

        [ContextMenu("Rotate")]
        public void Rotate() => Rotate(degrees);

        public void Rotate(float degreesOverride) => Play(degreesOverride, revertAfterwards: false);

        protected override void BeginAnimation(float amount)
        {
            Transform animated = ActiveTarget;
            animationStartLocalRotation = animated.localRotation;
            animationParentRotation = animated.parent != null ? animated.parent.rotation : Quaternion.identity;
            animationEuler = new Vector3(
                (axes & Axes.X) != 0 ? amount : 0f,
                (axes & Axes.Y) != 0 ? amount : 0f,
                (axes & Axes.Z) != 0 ? amount : 0f);
        }

        protected override void ApplyProgress(float progress)
        {
            Quaternion delta = Quaternion.Euler(animationEuler * Mathf.SmoothStep(0f, 1f, progress));

            // Driven in local space so the animation rides along with a moving parent.
            ActiveTarget.localRotation = space == Space.Self
                ? animationStartLocalRotation * delta
                : Quaternion.Inverse(animationParentRotation) * delta * animationParentRotation * animationStartLocalRotation;
        }

        protected override object CaptureRevertState() => ActiveTarget.localRotation;
        protected override void ApplyRevertState(object state) => ActiveTarget.localRotation = (Quaternion)state;
    }
}
