using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace ECDA.VRTutorialKit
{
    public class HandAnimator : MonoBehaviour
    {
        [SerializeField] private InputActionReference mainTrigger;
        [SerializeField] private InputActionReference secondaryTrigger;
        [SerializeField] private NearFarInteractor hoverInteractor;
        [SerializeField] private HandPoseDriver driver;

        [Header("Hover")]
        [SerializeField] private HandPose hoverPose;

        private GameObject hoveredUGUIObject;
        private VisualElement hoveredVisualElement;

        private void Reset()
        {
            driver = GetComponent<HandPoseDriver>();
            hoverInteractor = GetComponentInParent<NearFarInteractor>();
        }

        private void OnEnable()
        {
            if (hoverInteractor == null) return;
            hoverInteractor.uiHoverEntered.AddListener(OnUIHoverEntered);
            hoverInteractor.uiHoverExited.AddListener(OnUIHoverExited);
        }

        private void OnDisable()
        {
            if (hoverInteractor != null)
            {
                hoverInteractor.uiHoverEntered.RemoveListener(OnUIHoverEntered);
                hoverInteractor.uiHoverExited.RemoveListener(OnUIHoverExited);
            }

            hoveredUGUIObject = null;
            hoveredVisualElement = null;
        }

        private void Update()
        {
            if (driver == null) return;

            float triggerCurl = ReadAxis(mainTrigger);
            float gripCurl = ReadAxis(secondaryTrigger);

            bool hasHover = hoverInteractor != null &&
                (hoverInteractor.hasHover || IsHoveringUIControl());
            HandPose floor = hasHover ? hoverPose : null;

            ApplyCurl(FingerType.Index, triggerCurl, floor);
            ApplyCurl(FingerType.Middle, gripCurl, floor);
            ApplyCurl(FingerType.Ring, gripCurl, floor);
            ApplyCurl(FingerType.Pinky, gripCurl, floor);
            ApplyCurl(FingerType.Thumb, gripCurl, floor);
        }

        private void OnUIHoverEntered(UIHoverEventArgs args)
        {
            switch (args.uiSystem)
            {
                case UIHoverEventArgs.UISystem.UnityUI:
                    hoveredUGUIObject = args.uiObject;
                    break;
                case UIHoverEventArgs.UISystem.UIToolkit:
                    hoveredVisualElement = args.visualElement;
                    break;
            }
        }

        private void OnUIHoverExited(UIHoverEventArgs args)
        {
            switch (args.uiSystem)
            {
                case UIHoverEventArgs.UISystem.UnityUI:
                    if (hoveredUGUIObject == args.uiObject) hoveredUGUIObject = null;
                    break;
                case UIHoverEventArgs.UISystem.UIToolkit:
                    if (hoveredVisualElement == args.visualElement) hoveredVisualElement = null;
                    break;
            }
        }

        // Interactability is checked every frame, since a control can be disabled while hovered.
        private bool IsHoveringUIControl()
        {
            return IsInteractableSelectable(hoveredUGUIObject) || IsInteractableControl(hoveredVisualElement);
        }

        private static bool IsInteractableSelectable(GameObject uiObject)
        {
            if (uiObject == null || !uiObject.activeInHierarchy) return false;

            var selectable = uiObject.GetComponentInParent<Selectable>();
            return selectable != null && selectable.IsInteractable();
        }

        private static bool IsInteractableControl(VisualElement element)
        {
            if (element == null || element.panel == null) return false;

            // Controls (buttons, toggles, sliders, fields) are focusable; labels and containers aren't.
            for (var current = element; current != null; current = current.parent)
            {
                if (current.focusable)
                    return current.enabledInHierarchy;
            }

            return false;
        }

        private void ApplyCurl(FingerType finger, float inputCurl, HandPose floor)
        {
            float value = floor != null
                ? Mathf.Max(inputCurl, floor.GetCurl(finger))
                : inputCurl;

            driver.SetBaseCurl(finger, value);
        }

        private static float ReadAxis(InputActionReference reference)
        {
            if (reference == null || reference.action == null) return 0f;
            return Mathf.Clamp01(reference.action.ReadValue<float>());
        }
    }
}
