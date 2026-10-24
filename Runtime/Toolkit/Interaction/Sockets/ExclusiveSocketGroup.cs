using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ECDA.VRTutorialKit
{
    public class ExclusiveSocketGroup : MonoBehaviour
    {
        [SerializeField] private List<SocketFilterByTag> sockets = new();

        public UnityEvent<GameObject> onSocketedObjectChanged;

        public SocketFilterByTag OccupiedSocket { get; private set; }

        public GameObject SocketedObject => OccupiedSocket != null ? OccupiedSocket.SocketedObject : null;

        public bool HasItem => OccupiedSocket != null;

        void OnEnable()
        {
            foreach (SocketFilterByTag socket in sockets)
            {
                socket.onObjectSocketed.AddListener(OnSocketChanged);
                socket.onObjectRemoved.AddListener(OnSocketChanged);
            }

            Refresh();
        }

        void OnDisable()
        {
            foreach (SocketFilterByTag socket in sockets)
            {
                socket.onObjectSocketed.RemoveListener(OnSocketChanged);
                socket.onObjectRemoved.RemoveListener(OnSocketChanged);
            }
        }

        private void OnSocketChanged(GameObject _)
        {
            Refresh();
        }

        private void Refresh()
        {
            SocketFilterByTag previous = OccupiedSocket;
            OccupiedSocket = null;

            foreach (SocketFilterByTag socket in sockets)
            {
                if (socket.HasItem)
                {
                    OccupiedSocket = socket;
                    break;
                }
            }

            foreach (SocketFilterByTag socket in sockets)
                socket.GetComponent<XRSocketInteractor>().socketActive = OccupiedSocket == null || OccupiedSocket == socket;

            if (OccupiedSocket != previous)
                onSocketedObjectChanged?.Invoke(SocketedObject);
        }
    }
}
