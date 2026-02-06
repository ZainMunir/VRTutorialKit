using UnityEngine;

namespace VRTK.Examples
{
    public class RotateObject : MonoBehaviour
    {

        public void RotateX(int degrees)
        {
            Rotate(Vector3.right, degrees);
        }

        public void RotateY(int degrees)
        {
            Rotate(Vector3.up, degrees);
        }

        public void RotateZ(int degrees)
        {
            Rotate(Vector3.forward, degrees);
        }

        public void Rotate(Vector3 axis, int degrees)
        {
            transform.Rotate(axis, degrees);
        }


        [ContextMenu("RotateX 45")]
        private void RotateX45() => RotateX(45);
        [ContextMenu("RotateY 45")]
        private void RotateY45() => RotateY(45);
        [ContextMenu("RotateZ 45")]
        private void RotateZ45() => RotateZ(45);
    }
}