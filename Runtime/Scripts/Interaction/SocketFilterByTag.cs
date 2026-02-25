using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ECDA.VRTutorialKit
{
    [RequireComponent(typeof(XRSocketInteractor))]
    public class SocketFilterByTag : MonoBehaviour, IXRSelectFilter, IXRHoverFilter
    {
        [Tooltip("The tag required for an object to be accepted.")]
        [SerializeField] private TargetTag requiredTag;

        [Tooltip("Event invoked when the valid object enters the socket.")]
        public UnityEvent onValidEntry;

        private XRSocketInteractor m_SocketInteractor;

        public bool canProcess => isActiveAndEnabled;

        void Awake()
        {
            m_SocketInteractor = GetComponent<XRSocketInteractor>();

            if (m_SocketInteractor != null)
            {
                m_SocketInteractor.selectFilters.Add(this);
                m_SocketInteractor.hoverFilters.Add(this);
            }
        }

        void OnDestroy()
        {
            if (m_SocketInteractor != null)
            {
                m_SocketInteractor.selectFilters.Remove(this);
                m_SocketInteractor.hoverFilters.Remove(this);
            }
        }

        void OnEnable()
        {
            if (m_SocketInteractor != null)
                m_SocketInteractor.selectEntered.AddListener(OnSelectEntered);
        }

        void OnDisable()
        {
            if (m_SocketInteractor != null)
                m_SocketInteractor.selectEntered.RemoveListener(OnSelectEntered);
        }

        void OnSelectEntered(SelectEnterEventArgs args)
        {
            // Since we are filtering, if something enters, it obeys the filter.
            onValidEntry?.Invoke();
        }

        public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
        {
            return Validate(interactable);
        }

        public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
        {
            return Validate(interactable);
        }

        private bool Validate(IXRInteractable interactable)
        {
            if (requiredTag == null) return true;

            if (interactable.transform.TryGetComponent<SocketInteractableTag>(out var interactableTag))
            {
                return interactableTag.enabled ? interactableTag.socketTag == requiredTag : false;
            }

            return false;
        }
    }
}