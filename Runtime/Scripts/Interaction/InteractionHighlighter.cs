using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ECDA.VRTutorialKit
{
    [RequireComponent(typeof(XRBaseInteractable))]
    public class InteractionHighlighter : MonoBehaviour
    {

        [SerializeField] private List<GameObject> linkedObjects = new List<GameObject>();

        private XRBaseInteractable m_Interactable;

        private void Awake()
        {
            m_Interactable = GetComponent<XRBaseInteractable>();

        }

        private void OnEnable()
        {
            if (m_Interactable != null)
            {
                m_Interactable.selectEntered.AddListener(OnSelectEntered);
                m_Interactable.selectExited.AddListener(OnSelectExited);
            }
        }

        private void OnDisable()
        {
            if (m_Interactable != null)
            {
                m_Interactable.selectEntered.RemoveListener(OnSelectEntered);
                m_Interactable.selectExited.RemoveListener(OnSelectExited);
            }
        }

        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            SetHighlightState(true);
        }

        private void OnSelectExited(SelectExitEventArgs args)
        {
            SetHighlightState(false);
        }

        private void SetHighlightState(bool active)
        {
            foreach (var obj in linkedObjects)
            {
                HighlightObject highlightable = obj.GetComponent<HighlightObject>();
                if (highlightable == null)
                {
                    highlightable = obj.AddComponent<HighlightObject>();
                }

                if (active) highlightable.Highlight();
                else highlightable.Unhighlight();

            }
        }
    }
}