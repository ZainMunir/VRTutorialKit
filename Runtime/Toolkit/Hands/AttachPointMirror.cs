using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ECDA.VRTutorialKit
{

    public class AttachPointMirror : MonoBehaviour
    {
        public enum MirrorAxis
        {
            X,
            Y,
            Z,
        }

        [SerializeField] Transform leftAttach;
        [SerializeField] Transform rightAttach;

        [Tooltip("Axis normal to the mirror plane, expressed in the mirror space. " +
                 "X is correct for the usual left/right symmetry of a held object.")]
        [SerializeField] MirrorAxis mirrorAxis = MirrorAxis.X;

        [Tooltip("Transform whose position and rotation define the mirror plane. " +
                 "Leave empty to use this transform.")]
        [SerializeField] Transform mirrorSpace;

        public Transform LeftAttach => leftAttach;
        public Transform RightAttach => rightAttach;

        Transform ActiveMirrorSpace => mirrorSpace != null ? mirrorSpace : transform;

        void Reset()
        {
            foreach (Transform child in transform)
            {
                var childName = child.name.ToLowerInvariant();
                if (leftAttach == null && childName.Contains("left"))
                    leftAttach = child;
                else if (rightAttach == null && childName.Contains("right"))
                    rightAttach = child;
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Mirror Left \u2192 Right")]
        void MirrorLeftToRight() => Mirror(leftAttach, rightAttach);

        [ContextMenu("Mirror Right \u2192 Left")]
        void MirrorRightToLeft() => Mirror(rightAttach, leftAttach);

        void Mirror(Transform source, Transform destination)
        {
            if (source == null || destination == null)
            {
                Debug.LogWarning($"{name}: assign both left and right attach transforms before mirroring.", this);
                return;
            }

            if (source == destination)
            {
                Debug.LogWarning($"{name}: left and right attach reference the same transform.", this);
                return;
            }

            // Scale is deliberately excluded from the space matrix; a scaled parent
            // should not skew the mirrored offset.
            var space = ActiveMirrorSpace;
            var toWorld = Matrix4x4.TRS(space.position, space.rotation, Vector3.one);
            var toLocal = toWorld.inverse;

            var localPosition = toLocal.MultiplyPoint3x4(source.position);
            var localRotation = Quaternion.Inverse(space.rotation) * source.rotation;

            var mirroredPosition = MirrorPosition(localPosition, mirrorAxis);
            var mirroredRotation = MirrorRotation(localRotation, mirrorAxis);

            Undo.RecordObject(destination, "Mirror Attach Point");
            destination.position = toWorld.MultiplyPoint3x4(mirroredPosition);
            destination.rotation = space.rotation * mirroredRotation;

            // Copied rather than mirrored: a negative scale on an attach transform
            // breaks XRIT's pose maths.
            destination.localScale = source.localScale;

            EditorUtility.SetDirty(destination);
        }
#endif

        static Vector3 MirrorPosition(Vector3 position, MirrorAxis axis)
        {
            switch (axis)
            {
                case MirrorAxis.X: return new Vector3(-position.x, position.y, position.z);
                case MirrorAxis.Y: return new Vector3(position.x, -position.y, position.z);
                default: return new Vector3(position.x, position.y, -position.z);
            }
        }

        /// <summary>
        /// Reflecting a rotation across a plane keeps the quaternion component along the
        /// plane normal and negates the other two vector components, leaving w untouched.
        /// This is the R' = M * R * M conjugation, which stays a proper rotation.
        /// </summary>
        static Quaternion MirrorRotation(Quaternion rotation, MirrorAxis axis)
        {
            switch (axis)
            {
                case MirrorAxis.X: return new Quaternion(rotation.x, -rotation.y, -rotation.z, rotation.w);
                case MirrorAxis.Y: return new Quaternion(-rotation.x, rotation.y, -rotation.z, rotation.w);
                default: return new Quaternion(-rotation.x, -rotation.y, rotation.z, rotation.w);
            }
        }
    }
}