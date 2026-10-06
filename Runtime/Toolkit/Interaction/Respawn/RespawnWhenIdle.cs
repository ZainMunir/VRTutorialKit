using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ECDA.VRTutorialKit
{
    [RequireComponent(typeof(RespawnableObject), typeof(Rigidbody))]
    public class RespawnWhenIdle : MonoBehaviour
    {
        [SerializeField] private float m_IdleDelay = 2.0f;
        [SerializeField] private float m_IdleSpeed = 0.05f;
        [SerializeField] private float m_MinDistance = 0.5f;

        private RespawnableObject m_Respawnable;
        private Rigidbody m_Rigidbody;
        private XRGrabInteractable m_Interactable;
        private float m_IdleTime;

        private void Awake()
        {
            m_Respawnable = GetComponent<RespawnableObject>();
            m_Rigidbody = GetComponent<Rigidbody>();
            m_Interactable = GetComponentInParent<XRGrabInteractable>();
        }

        private void OnEnable()
        {
            m_IdleTime = 0f;
        }

        private void Update()
        {
            if (!IsIdle())
            {
                m_IdleTime = 0f;
                return;
            }

            m_IdleTime += Time.deltaTime;
            if (m_IdleTime < m_IdleDelay) return;

            m_IdleTime = 0f;
            m_Respawnable.Respawn();
        }

        private bool IsIdle()
        {
            if (m_Rigidbody.isKinematic) return false;
            if (m_Interactable != null && m_Interactable.isSelected) return false;
            if (m_Rigidbody.linearVelocity.sqrMagnitude > m_IdleSpeed * m_IdleSpeed) return false;

            return (transform.position - m_Respawnable.SpawnPosition).sqrMagnitude > m_MinDistance * m_MinDistance;
        }
    }
}
