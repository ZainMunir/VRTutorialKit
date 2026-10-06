using UnityEngine;
using UnityEngine.Events;

namespace ECDA.VRTutorialKit
{

    public class RespawnableObject : MonoBehaviour
    {
        [SerializeField] private Transform m_SpawnPointOverride;
        [SerializeField] private bool m_FollowSpawnPoint;
        [SerializeField] private bool m_ResetScale;
        private Vector3 m_SpawnPosition;
        private Quaternion m_SpawnRotation;
        private Vector3 m_SpawnScale;
        private Rigidbody m_Rigidbody;
        public TargetTag targetTag;

        public UnityEvent onRespawn;

        private bool FollowsSpawnPoint => m_FollowSpawnPoint && m_SpawnPointOverride != null;
        public Vector3 SpawnPosition => FollowsSpawnPoint ? m_SpawnPointOverride.position : m_SpawnPosition;
        public Quaternion SpawnRotation => FollowsSpawnPoint ? m_SpawnPointOverride.rotation : m_SpawnRotation;

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

            m_SpawnScale = transform.localScale;
        }

        public void SetSpawnPoint(Transform spawnPoint)
        {
            m_SpawnPointOverride = spawnPoint;

            if (spawnPoint != null)
            {
                m_SpawnPosition = spawnPoint.position;
                m_SpawnRotation = spawnPoint.rotation;
            }
        }

        public void Respawn()
        {
            // Reset Physics
            if (m_Rigidbody != null && !m_Rigidbody.isKinematic)
            {
                m_Rigidbody.linearVelocity = Vector3.zero;
                m_Rigidbody.angularVelocity = Vector3.zero;
            }

            // Teleport
            transform.SetPositionAndRotation(SpawnPosition, SpawnRotation);

            if (m_ResetScale)
                transform.localScale = m_SpawnScale;

            onRespawn?.Invoke();
        }
    }
}
