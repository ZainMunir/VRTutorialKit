using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ECDA.VRTutorialKit
{
    [RequireComponent(typeof(Collider))]
    public class ObjectRespawnVolume : MonoBehaviour
    {
        [SerializeField] private float m_RespawnDelay = 3.0f;
        private Dictionary<RespawnableObject, Coroutine> m_PendingRespawns = new Dictionary<RespawnableObject, Coroutine>();

        private void Awake()
        {
            var col = GetComponent<Collider>();
            if (!col.isTrigger)
            {
                Debug.LogWarning($"Collider on {name} is not a trigger. Setting isTrigger to true.", this);
                col.isTrigger = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var respawnable = GetRespawnable(other);
            if (respawnable == null) return;

            if (m_PendingRespawns.TryGetValue(respawnable, out Coroutine routine))
            {
                if (routine != null) StopCoroutine(routine);
                m_PendingRespawns.Remove(respawnable);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            RespawnableObject respawnable = GetRespawnable(other);
            if (respawnable == null) return;

            if (!m_PendingRespawns.ContainsKey(respawnable))
            {
                Coroutine routine = StartCoroutine(RespawnRoutine(respawnable));
                m_PendingRespawns.Add(respawnable, routine);
            }
        }

        private RespawnableObject GetRespawnable(Collider col)
        {
            return col.GetComponentInParent<RespawnableObject>();
        }

        private IEnumerator RespawnRoutine(RespawnableObject target)
        {
            yield return new WaitForSeconds(m_RespawnDelay);

            if (target != null)
            {
                // Check if held
                var interactable = target.GetComponentInParent<XRGrabInteractable>();
                bool isHeld = interactable != null && interactable.isSelected;

                while (isHeld)
                {
                    yield return new WaitForSeconds(1.0f);
                    isHeld = interactable != null && interactable.isSelected;
                }

                target.Respawn();
            }

            m_PendingRespawns.Remove(target);
        }
    }
}
