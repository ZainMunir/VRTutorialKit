using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace ECDA.VRTutorialKit
{
    [RequireComponent(typeof(UIDocument))]
    public class ButtonController : MonoBehaviour
    {
        public string buttonName = "Button";
        public LocalizedString buttonText;
        public UnityEvent onClick;

        private Button uiButton;

        public void Start()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            VisualElement buttonContainer = root.Q<VisualElement>(buttonName);
            uiButton = buttonContainer.Q<Button>();
            if (uiButton != null)
            {
                uiButton.text = buttonText.GetLocalizedString();
                uiButton.clicked += () => onClick?.Invoke();
            }

            LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;
        }

        private void OnSelectedLocaleChanged(Locale newLocale)
        {
            UpdateUI();
        }

        void UpdateUI()
        {
            if (uiButton != null)
            {
                uiButton.text = buttonText.GetLocalizedString();
            }
        }

        public void OnDestroy()
        {
            if (uiButton != null)
            {
                uiButton.clicked -= () => onClick?.Invoke();
            }
            LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
        }

    }
}