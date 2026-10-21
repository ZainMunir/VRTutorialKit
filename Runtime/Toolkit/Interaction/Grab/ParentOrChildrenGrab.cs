using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ECDA.VRTutorialKit
{

    [DisallowMultipleComponent]
    [RequireComponent(typeof(GrabbableToggle))]
    public class ParentOrChildrenGrab : MonoBehaviour, IXRSelectFilter, IXRHoverFilter
    {
        public enum ChildrenGrabMode
        {
            OppositeOfParent,
            Grabbable,
            NotGrabbable
        }

        [SerializeField] private bool parentGrabbable = true;
        [Tooltip("Opposite Of Parent: either the parent or the children are grabbable, never both. Grabbable / Not Grabbable: set independently of the parent.")]
        [SerializeField] private ChildrenGrabMode childrenGrabbable = ChildrenGrabMode.OppositeOfParent;
        [Tooltip("Children are only interactable while the parent is held by something other than a socket.")]
        [SerializeField] private bool requireParentHeld;
        [Tooltip("Sockets reject new items while the children are not interactable. Items already in a socket, or placed by its SocketPrefill, are let through.")]
        [SerializeField] private bool gateInsertion;
        [Tooltip("Socketed items don't collide with the colliders of the parent's interactable.")]
        [SerializeField] private bool ignoreParentCollisions;
        private GrabbableToggle parentGrab;
        private XRGrabInteractable parentInteractable;
        [SerializeField] private XRSocketInteractor[] sockets;
        public UnityEvent<bool> onParentGrabbableChanged;
        private readonly List<IXRSelectInteractable> m_ItemBuffer = new List<IXRSelectInteractable>();
        private readonly List<XRBaseInteractable> m_PendingRestore = new List<XRBaseInteractable>();
        private bool m_Locked;
        public bool ParentGrabbable => parentGrabbable && !m_Locked;
        public bool ChildrenFlag => childrenGrabbable switch
        {
            ChildrenGrabMode.Grabbable => true,
            ChildrenGrabMode.NotGrabbable => false,
            _ => !parentGrabbable
        };
        public bool ChildrenGrabbable => ChildrenFlag && !m_Locked && (!requireParentHeld || IsParentHeld);
        public bool IsParentHeld => IsHeld(parentInteractable);
        public bool IsLocked => m_Locked;
        public bool canProcess => isActiveAndEnabled && gateInsertion;

        private void Reset()
        {
            parentGrab = GetComponent<GrabbableToggle>();
            GetSockets();
        }

        private void Awake()
        {
            parentGrab = GetComponent<GrabbableToggle>();
            parentInteractable = GetComponent<XRGrabInteractable>();
            if (sockets == null || sockets.Length == 0)
                GetSockets();
        }

        private void OnEnable()
        {
            if (parentInteractable != null)
            {
                parentInteractable.selectEntered.AddListener(HandleParentSelectEntered);
                parentInteractable.selectExited.AddListener(HandleParentSelectExited);
            }

            foreach (XRSocketInteractor socket in sockets)
            {
                if (socket == null)
                    continue;

                socket.selectEntered.AddListener(HandleItemSocketed);
                socket.selectExited.AddListener(HandleItemUnsocketed);
                socket.selectFilters.Add(this);
                socket.hoverFilters.Add(this);

                if (!ignoreParentCollisions)
                    continue;

                foreach (IXRSelectInteractable item in socket.interactablesSelected)
                    SetIgnoreParentCollisions(item as XRBaseInteractable, true);
            }
        }

        private void OnDisable()
        {
            if (parentInteractable != null)
            {
                parentInteractable.selectEntered.RemoveListener(HandleParentSelectEntered);
                parentInteractable.selectExited.RemoveListener(HandleParentSelectExited);
            }

            foreach (XRSocketInteractor socket in sockets)
            {
                if (socket == null)
                    continue;

                socket.selectEntered.RemoveListener(HandleItemSocketed);
                socket.selectExited.RemoveListener(HandleItemUnsocketed);
                socket.selectFilters.Remove(this);
                socket.hoverFilters.Remove(this);

                if (!ignoreParentCollisions)
                    continue;

                foreach (IXRSelectInteractable item in socket.interactablesSelected)
                    SetIgnoreParentCollisions(item as XRBaseInteractable, false);
            }

            foreach (XRBaseInteractable item in m_PendingRestore)
                SetIgnoreParentCollisions(item, false);

            m_PendingRestore.Clear();
        }

        private void Start()
        {
            ApplyState();
        }

        // Restore only once the removed item has cleared the parent, so the two aren't pushed apart.
        private void FixedUpdate()
        {
            for (int i = m_PendingRestore.Count - 1; i >= 0; i--)
            {
                XRBaseInteractable item = m_PendingRestore[i];
                if (item != null && OverlapsParent(item))
                    continue;

                SetIgnoreParentCollisions(item, false);
                m_PendingRestore.RemoveAt(i);
            }
        }

        public void SetGrabbable(bool value)
        {
            parentGrabbable = value;
            childrenGrabbable = ChildrenGrabMode.OppositeOfParent;
            ApplyState();
        }

        public void SetChildrenGrabbable(bool value)
        {
            SetGrabbable(!value);
        }

        public void SetChildrenFlag(bool value)
        {
            childrenGrabbable = value ? ChildrenGrabMode.Grabbable : ChildrenGrabMode.NotGrabbable;
            ApplyChildren();
        }

        public void SetLocked(bool value)
        {
            m_Locked = value;
            ApplyState();
        }

        [ContextMenu("Apply State")]
        public void ApplyState()
        {
            if (parentGrab != null)
                parentGrab.SetGrabbable(ParentGrabbable);

            ApplyChildren();
            onParentGrabbableChanged?.Invoke(ParentGrabbable);
        }

        [ContextMenu("Swap Grabbable")]
        public void SwapGrabbable()
        {
            SetGrabbable(!parentGrabbable);
        }

        [ContextMenu("Get Sockets")]
        public void GetSockets()
        {
            var found = new List<XRSocketInteractor>();

            foreach (XRSocketInteractor socket in GetComponentsInChildren<XRSocketInteractor>(true))
            {
                if (socket.GetComponentInParent<ParentOrChildrenGrab>(true) == this)
                    found.Add(socket);
            }

            sockets = found.ToArray();
        }

        public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
        {
            return ChildrenGrabbable || interactor.IsSelecting(interactable) ||
                (interactor.transform.TryGetComponent(out SocketPrefill prefill) && prefill.IsPlacing(interactable));
        }

        public bool Process(IXRHoverInteractor interactor, IXRHoverInteractable interactable)
        {
            return ChildrenGrabbable ||
                (interactor is IXRSelectInteractor selector && interactable is IXRSelectInteractable item && selector.IsSelecting(item));
        }

        public static bool IsHeld(IXRSelectInteractable interactable)
        {
            if (interactable == null)
                return false;

            var interactors = interactable.interactorsSelecting;
            for (int i = 0; i < interactors.Count; i++)
            {
                if (interactors[i] is not XRSocketInteractor)
                    return true;
            }

            return false;
        }

        private void ApplyChildren()
        {
            bool grabbable = ChildrenGrabbable;

            foreach (XRSocketInteractor socket in sockets)
            {
                if (socket == null)
                    continue;

                m_ItemBuffer.Clear();
                m_ItemBuffer.AddRange(socket.interactablesSelected);

                foreach (IXRSelectInteractable item in m_ItemBuffer)
                    ApplyToItem(item, grabbable);
            }

            m_ItemBuffer.Clear();
        }

        private void HandleParentSelectEntered(SelectEnterEventArgs args)
        {
            if (requireParentHeld)
                ApplyChildren();
        }

        private void HandleParentSelectExited(SelectExitEventArgs args)
        {
            if (requireParentHeld)
                ApplyChildren();
        }

        private void HandleItemSocketed(SelectEnterEventArgs args)
        {
            ApplyToItem(args.interactableObject, ChildrenGrabbable);

            if (ignoreParentCollisions && args.interactableObject is XRBaseInteractable item)
            {
                m_PendingRestore.Remove(item);
                SetIgnoreParentCollisions(item, true);
            }
        }

        private void HandleItemUnsocketed(SelectExitEventArgs args)
        {
            if (ignoreParentCollisions && args.interactableObject is XRBaseInteractable item && !m_PendingRestore.Contains(item))
                m_PendingRestore.Add(item);
        }

        private void ApplyToItem(IXRSelectInteractable item, bool grabbable)
        {
            Transform itemTransform = item?.transform;
            if (itemTransform == null)
                return;

            if (itemTransform.TryGetComponent(out ParentOrChildrenGrab nested) && nested != this)
            {
                nested.SetLocked(!grabbable);
                return;
            }

            if (itemTransform.TryGetComponent(out GrabbableToggle toggle))
                toggle.SetGrabbable(grabbable);
        }

        private void SetIgnoreParentCollisions(XRBaseInteractable item, bool ignore)
        {
            if (item == null || parentInteractable == null)
                return;

            foreach (Collider itemCollider in item.colliders)
            {
                if (itemCollider == null || !itemCollider.gameObject.activeInHierarchy)
                    continue;

                foreach (Collider parentCollider in parentInteractable.colliders)
                {
                    if (parentCollider == null || parentCollider == itemCollider || !parentCollider.gameObject.activeInHierarchy)
                        continue;

                    Physics.IgnoreCollision(itemCollider, parentCollider, ignore);
                }
            }
        }

        private bool OverlapsParent(XRBaseInteractable item)
        {
            foreach (Collider itemCollider in item.colliders)
            {
                if (itemCollider == null)
                    continue;

                foreach (Collider parentCollider in parentInteractable.colliders)
                {
                    if (parentCollider != null && parentCollider != itemCollider && itemCollider.bounds.Intersects(parentCollider.bounds))
                        return true;
                }
            }

            return false;
        }
    }
}
