using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ECDA.VRTutorialKit
{

    [DisallowMultipleComponent]
    [RequireComponent(typeof(XRGrabInteractable))]
    public class GrabbableToggle : MonoBehaviour
    {
        private XRGrabInteractable grabInteractable;

        [SerializeField] private bool grabbable = true;
        [SerializeField] private InteractionLayerMask grabbableLayers = 1;
        [SerializeField] private InteractionLayerMask notGrabbableLayers = 0;
        [SerializeField] private GameObject[] physicsLayerTargets;
        [SerializeField] private string grabbablePhysicsLayer = "Default";
        [SerializeField] private string notGrabbablePhysicsLayer = "Ignore Raycast";
        [SerializeField] private bool disableCollisionsWhileSocketed = true;
        public UnityEvent<bool> onGrabbableChanged;
        private Rigidbody m_Rigidbody;
        public XRGrabInteractable GrabInteractable => grabInteractable;
        public bool IsGrabbable => grabbable;
        public bool IsSocketed
        {
            get
            {
                if (grabInteractable == null)
                    return false;

                var interactors = grabInteractable.interactorsSelecting;
                for (int i = 0; i < interactors.Count; i++)
                {
                    if (interactors[i] is XRSocketInteractor)
                        return true;
                }

                return false;
            }
        }

        private void Reset()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();

            if (grabInteractable != null)
                grabbableLayers = grabInteractable.interactionLayers;
        }

        private void Awake()
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
            m_Rigidbody = GetComponent<Rigidbody>();

            Apply();
        }

        private void OnEnable()
        {
            if (grabInteractable == null)
                return;

            grabInteractable.selectEntered.AddListener(HandleSelectEntered);
            grabInteractable.selectExited.AddListener(HandleSelectExited);
        }

        private void OnDisable()
        {
            if (grabInteractable == null)
                return;

            grabInteractable.selectEntered.RemoveListener(HandleSelectEntered);
            grabInteractable.selectExited.RemoveListener(HandleSelectExited);
        }

        public void SetGrabbable(bool value)
        {
            grabbable = value;
            Apply();
        }


        private void Apply()
        {
            if (grabInteractable != null)
                grabInteractable.interactionLayers = grabbable ? grabbableLayers : notGrabbableLayers;

            ApplyPhysicsLayer();
            ApplyCollisions();

            onGrabbableChanged?.Invoke(grabbable);
        }

        private void ApplyPhysicsLayer()
        {
            if (physicsLayerTargets == null || physicsLayerTargets.Length == 0)
                return;

            string layerName = grabbable ? grabbablePhysicsLayer : notGrabbablePhysicsLayer;
            int layer = LayerMask.NameToLayer(layerName);

            if (layer < 0)
            {
                Debug.LogWarning($"[{name}] Physics layer '{layerName}' does not exist. Add it under Tags & Layers.", this);
                return;
            }

            foreach (GameObject target in physicsLayerTargets)
            {
                if (target != null)
                    target.layer = layer;
            }
        }

        private void ApplyCollisions()
        {
            if (!disableCollisionsWhileSocketed || m_Rigidbody == null)
                return;

            m_Rigidbody.detectCollisions = grabbable || !IsSocketed;
        }

        private void HandleSelectEntered(SelectEnterEventArgs args)
        {
            ApplyCollisions();
        }

        private void HandleSelectExited(SelectExitEventArgs args)
        {
            ApplyCollisions();
        }
    }
}
