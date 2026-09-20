using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class MoveObject : TransformTweenConditionSource
    {
        [SerializeField] private float distance = 1f;

        private Vector3 animationStartLocalPosition;
        private Vector3 animationLocalOffset;

        protected override float DefaultAmount => distance;

        [ContextMenu("Move")]
        public void Move() => Move(distance);

        public void Move(float distanceOverride) => Play(distanceOverride, revertAfterwards: false);

        protected override void BeginAnimation(float amount)
        {
            Transform animated = ActiveTarget;
            Vector3 translation = new(
                (axes & Axes.X) != 0 ? amount : 0f,
                (axes & Axes.Y) != 0 ? amount : 0f,
                (axes & Axes.Z) != 0 ? amount : 0f);

            Vector3 worldOffset = space == Space.Self
                ? animated.TransformDirection(translation)
                : translation;

            animationStartLocalPosition = animated.localPosition;
            animationLocalOffset = animated.parent != null
                ? animated.parent.InverseTransformVector(worldOffset)
                : worldOffset;
        }

        // Driven in local space so the animation rides along with a moving parent.
        protected override void ApplyProgress(float progress) =>
            ActiveTarget.localPosition =
                animationStartLocalPosition + animationLocalOffset * Mathf.SmoothStep(0f, 1f, progress);

        protected override object CaptureRevertState() => ActiveTarget.localPosition;
        protected override void ApplyRevertState(object state) => ActiveTarget.localPosition = (Vector3)state;
    }
}
