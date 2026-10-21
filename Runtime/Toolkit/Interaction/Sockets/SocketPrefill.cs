using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ECDA.VRTutorialKit
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(XRSocketInteractor))]
    public class SocketPrefill : MonoBehaviour
    {
        [Tooltip("Prefab spawned into this socket. Leave empty to not prefill. Its root needs an interactable the socket accepts.")]
        [SerializeField] private XRBaseInteractable prefab;
        [Tooltip("Fill the socket every time this component is enabled, if the socket is empty.")]
        [SerializeField] private bool fillOnEnable = true;
        private XRSocketInteractor socket;
        private XRBaseInteractable m_Placing;
        private bool m_Started;
        public XRBaseInteractable SpawnedItem { get; private set; }

        private void Awake()
        {
            socket = GetComponent<XRSocketInteractor>();

            if (prefab != null && socket.startingSelectedInteractable != null)
                Debug.LogWarning($"[{name}] Socket also has a Starting Selected Interactable; it and {prefab.name} will compete for the socket.", this);
        }

        private void Start()
        {
            m_Started = true;
            if (fillOnEnable)
                Fill();
        }

        private void OnEnable()
        {
            if (m_Started && fillOnEnable)
                Fill();
        }

        [ContextMenu("Fill")]
        public void Fill() => Fill(prefab);

        public void Fill(XRBaseInteractable source)
        {
            if (source == null || socket.hasSelection)
                return;

            Transform attach = socket.attachTransform != null ? socket.attachTransform : transform;
            SpawnedItem = Instantiate(source, attach.position, attach.rotation);

            m_Placing = SpawnedItem;
            socket.interactionManager.SelectEnter((IXRSelectInteractor)socket, (IXRSelectInteractable)SpawnedItem);
            m_Placing = null;
        }

        public bool IsPlacing(IXRSelectInteractable item)
        {
            return m_Placing != null && ReferenceEquals(m_Placing, item);
        }
    }
}
