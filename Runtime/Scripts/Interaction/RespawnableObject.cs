using UnityEngine;

namespace ECDA.VRTutorialKit
{

    public class RespawnableObject : MonoBehaviour
    {
        [SerializeField] private Transform m_SpawnPointOverride;
        private Vector3 m_SpawnPosition;
        private Quaternion m_SpawnRotation;
        private Rigidbody m_Rigidbody;

        private void Awake()
        {
            m_Rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            if (m_SpawnPointOverride != null)
            {
                m_SpawnPosition = m_SpawnPointOverride.position;
                m_SpawnRotation = m_SpawnPointOverride.rotation;
            }
            else
            {
                m_SpawnPosition = transform.position;
                m_SpawnRotation = transform.rotation;
            }
        }

        public void Respawn()
        {
            // Reset Physics
            if (m_Rigidbody != null)
            {
                m_Rigidbody.linearVelocity = Vector3.zero;
                m_Rigidbody.angularVelocity = Vector3.zero;
            }

            // Teleport
            transform.SetPositionAndRotation(m_SpawnPosition, m_SpawnRotation);
        }
    }
}
