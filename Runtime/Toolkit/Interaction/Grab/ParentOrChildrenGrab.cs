using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ECDA.VRTutorialKit
{

    [DisallowMultipleComponent]
    [RequireComponent(typeof(GrabbableToggle))]
    public class ParentOrChildrenGrab : MonoBehaviour
    {
        [SerializeField] private bool parentGrabbable = true;
        private GrabbableToggle parentGrab;
        [SerializeField] private XRSocketInteractor[] sockets;
        public UnityEvent<bool> onParentGrabbableChanged;
        private readonly List<IXRSelectInteractable> m_ItemBuffer = new List<IXRSelectInteractable>();
        private bool m_Locked;
        public bool ParentGrabbable => parentGrabbable && !m_Locked;
        public bool ChildrenGrabbable => !parentGrabbable && !m_Locked;
        public bool IsLocked => m_Locked;

        private void Reset()
        {
            parentGrab = GetComponent<GrabbableToggle>();
            GetSockets();
        }

        private void Awake()
        {
            parentGrab = GetComponent<GrabbableToggle>();
            if (sockets == null || sockets.Length == 0)
                GetSockets();
        }

        private void OnEnable()
        {
            foreach (XRSocketInteractor socket in sockets)
            {
                if (socket != null)
                    socket.selectEntered.AddListener(HandleItemSocketed);
            }
        }

        private void OnDisable()
        {
            foreach (XRSocketInteractor socket in sockets)
            {
                if (socket != null)
                    socket.selectEntered.RemoveListener(HandleItemSocketed);
            }
        }

        private void Start()
        {
            ApplyState();
        }

        public void SetGrabbable(bool value)
        {
            parentGrabbable = value;
            ApplyState();
        }

        public void SetChildrenGrabbable(bool value)
        {
            SetGrabbable(!value);
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

            bool childrenGrabbable = ChildrenGrabbable;

            foreach (XRSocketInteractor socket in sockets)
            {
                if (socket == null)
                    continue;

                m_ItemBuffer.Clear();
                m_ItemBuffer.AddRange(socket.interactablesSelected);

                foreach (IXRSelectInteractable item in m_ItemBuffer)
                    ApplyToItem(item, childrenGrabbable);
            }

            m_ItemBuffer.Clear();
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

        private void HandleItemSocketed(SelectEnterEventArgs args)
        {
            ApplyToItem(args.interactableObject, ChildrenGrabbable);
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
    }
}
