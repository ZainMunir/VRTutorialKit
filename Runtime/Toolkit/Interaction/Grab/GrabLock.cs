using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ECDA.VRTutorialKit
{
    /// <summary>
    /// While a hand holds this object, no other hand can hover, take or steal it. Sockets don't count as holding.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(XRGrabInteractable))]
    public class GrabLock : MonoBehaviour, IXRSelectFilter, IXRHoverFilter
    {
        [Tooltip("Only lock while the ParentOrChildrenGrab on this object has its children grabbable, so it can be passed between hands otherwise.")]
        [SerializeField] private bool onlyWhileChildrenGrabbable;
        private XRGrabInteractable grabInteractable;
        private ParentOrChildrenGrab parentOrChildren;
        public bool canProcess => isActiveAndEnabled;
        public bool IsLocked =>
            ParentOrChildrenGrab.IsHeld(grabInteractable) &&
            (!onlyWhileChildrenGrabbable || (parentOrChildren != null && parentOrChildren.ChildrenGrabbable));

        private void Awake()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
            parentOrChildren = GetComponent<ParentOrChildrenGrab>();

            if (onlyWhileChildrenGrabbable && parentOrChildren == null)
                Debug.LogWarning($"[{name}] Only While Children Grabbable needs a ParentOrChildrenGrab on this object; the lock stays off.", this);
        }

        private void OnEnable()
        {
            grabInteractable.selectFilters.Add(this);
            grabInteractable.hoverFilters.Add(this);
        }

        private void OnDisable()
        {
            grabInteractable.selectFilters.Remove(this);
            grabInteractable.hoverFilters.Remove(this);
        }

        public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
        {
            return !IsLocked || interactor.IsSelecting(interactable);
        }

        public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
        {
            return !IsLocked ||
                (interactor is IXRSelectInteractor selector && interactable is IXRSelectInteractable item && selector.IsSelecting(item));
        }
    }
}
