using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class ActionOnOrientation : IConditionSource, ProgressProvider
    {
        [Header("Orientation Settings")]
        [SerializeField] private Vector3 worldDirection = Vector3.up;
        [SerializeField, Range(0f, 180f)] private float degreeLeniency = 15f;
        [SerializeField] private bool evaluateContinuously = true;

        [Header("Gizmos")]
        [SerializeField] private float gizmoRayLength = 0.5f;
        [SerializeField] private float currentDirectionLengthMultiplier = 1.15f;
        [SerializeField] private bool drawWhenNotSelected = false;
        [SerializeField] private Color currentUpColor = Color.cyan;
        [SerializeField] private Color targetDirectionColor = Color.yellow;
        [SerializeField] private Color toleranceConeColor = new Color(1f, 1f, 0f, 0.2f);

        public float Progress => IsConditionMet ? 1f : 0f;


        private void Update()
        {
            if (!evaluateContinuously) return;

            Evaluate();
        }

        public void Evaluate()
        {
            bool newState = CalculateOrientationMatch();
            bool wasConditionMet = IsConditionMet;
            SetConditionState(newState);

            if (!wasConditionMet && newState)
            {
                InvokeConditionAction();
            }
        }

        public void SetWorldDirection(Vector3 direction)
        {
            worldDirection = direction;
            Evaluate();
        }

        private bool CalculateOrientationMatch()
        {
            Vector3 normalizedWorldDirection = worldDirection.normalized;
            if (normalizedWorldDirection.sqrMagnitude < Mathf.Epsilon)
            {
                normalizedWorldDirection = Vector3.up;
            }

            float angle = Vector3.Angle(transform.up, normalizedWorldDirection);
            return angle <= degreeLeniency;
        }

        private void OnDrawGizmos()
        {
            if (drawWhenNotSelected)
            {
                DrawGizmoVisualization();
            }
        }

        private void OnDrawGizmosSelected()
        {
            DrawGizmoVisualization();
        }

        private void DrawGizmoVisualization()
        {
            Vector3 origin = transform.position;
            Vector3 upDirection = transform.up.normalized;
            Vector3 normalizedWorldDirection = worldDirection.normalized;

            if (normalizedWorldDirection.sqrMagnitude < Mathf.Epsilon)
            {
                normalizedWorldDirection = Vector3.up;
            }

            Gizmos.color = currentUpColor;
            float currentDirectionLength = gizmoRayLength * Mathf.Max(1f, currentDirectionLengthMultiplier);
            Gizmos.DrawLine(origin, origin + upDirection * currentDirectionLength);

            Gizmos.color = targetDirectionColor;
            // Gizmos.DrawLine(origin, origin + normalizedWorldDirection * gizmoRayLength);

            DrawToleranceCone(origin, normalizedWorldDirection);
        }

        private void DrawToleranceCone(Vector3 origin, Vector3 direction)
        {
            const int segments = 24;

            float clampedAngle = Mathf.Clamp(degreeLeniency, 0f, 89f);
            float coneLength = gizmoRayLength;
            float baseRadius = Mathf.Tan(clampedAngle * Mathf.Deg2Rad) * coneLength;

            if (baseRadius <= 0f)
            {
                return;
            }

            Vector3 baseCenter = origin + direction * coneLength;

            Vector3 tangent = Vector3.Cross(direction, Vector3.up);
            if (tangent.sqrMagnitude < 0.0001f)
            {
                tangent = Vector3.Cross(direction, Vector3.right);
            }

            tangent.Normalize();
            Vector3 bitangent = Vector3.Cross(direction, tangent).normalized;

            Gizmos.color = toleranceConeColor;

            Vector3 firstPoint = Vector3.zero;
            Vector3 previousPoint = Vector3.zero;

            for (int index = 0; index <= segments; index++)
            {
                float t = index / (float)segments;
                float radians = t * Mathf.PI * 2f;

                Vector3 ringOffset = (Mathf.Cos(radians) * tangent + Mathf.Sin(radians) * bitangent) * baseRadius;
                Vector3 point = baseCenter + ringOffset;

                if (index == 0)
                {
                    firstPoint = point;
                }
                else
                {
                    Gizmos.DrawLine(previousPoint, point);
                }

                previousPoint = point;
            }

            Gizmos.DrawLine(previousPoint, firstPoint);

            for (int edge = 0; edge < 4; edge++)
            {
                float radians = (edge / 4f) * Mathf.PI * 2f;
                Vector3 ringOffset = (Mathf.Cos(radians) * tangent + Mathf.Sin(radians) * bitangent) * baseRadius;
                Gizmos.DrawLine(origin, baseCenter + ringOffset);
            }
        }

    }
}