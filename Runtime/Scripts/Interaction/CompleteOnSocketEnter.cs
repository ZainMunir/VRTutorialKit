using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ECDA.VRTutorialKit
{
    [RequireComponent(typeof(XRSocketInteractor))]
    public class CompleteOnSocketEnter : MonoBehaviour, IXRSelectFilter, IXRHoverFilter
    {
        private XRSocketInteractor m_socketInteractor;
        [SerializeField] private TutorialSubStep subStep;

        [SerializeField] private GameObject targetObject;

        public bool canProcess => isActiveAndEnabled;

        void Awake()
        {
            m_socketInteractor = GetComponent<XRSocketInteractor>();
            if (subStep == null)
            {
                Debug.LogError("TutorialSubStep reference is not set on CompleteOnSocketEnter.");
            }

            if (m_socketInteractor != null)
            {
                m_socketInteractor.selectFilters.Add(this);
                m_socketInteractor.hoverFilters.Add(this);
            }
        }

        void OnDestroy()
        {
            if (m_socketInteractor != null)
            {
                m_socketInteractor.selectFilters.Remove(this);
                m_socketInteractor.hoverFilters.Remove(this);
            }
        }

        void OnEnable()
        {
            if (m_socketInteractor != null)
                m_socketInteractor.selectEntered.AddListener(OnSelectEntered);
        }

        void OnDisable()
        {
            if (m_socketInteractor != null)
                m_socketInteractor.selectEntered.RemoveListener(OnSelectEntered);
        }

        void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (subStep != null)
            {
                subStep.Complete();
            }
        }

        public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
        {
            if (targetObject == null) return true;
            return interactable.transform.gameObject == targetObject;
        }

        public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
        {
            if (targetObject == null) return true;
            return interactable.transform.gameObject == targetObject;
        }
    }
}