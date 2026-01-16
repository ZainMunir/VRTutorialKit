using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;

namespace ECDA.VRTutorialKit
{
    [RequireComponent(typeof(UIDocument))]
    public class ButtonController : MonoBehaviour
    {
        public string buttonName = "Button";
        public string buttonText;
        public UnityEvent onClick;

        private Button uiButton;

        public void Start()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            VisualElement buttonContainer = root.Q<VisualElement>(buttonName);
            uiButton = buttonContainer.Q<Button>();
            if (uiButton != null)
            {
                uiButton.text = buttonText;
                uiButton.clicked += () => onClick?.Invoke();
            }
        }

        public void OnDestroy()
        {
            if (uiButton != null)
            {
                uiButton.clicked -= () => onClick?.Invoke();
            }
        }

    }
}