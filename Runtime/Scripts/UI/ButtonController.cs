using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System;

namespace ECDA.VRTutorialKit
{
    [RequireComponent(typeof(UIDocument))]
    public class ButtonController : MonoBehaviour
    {
        public string buttonName = "Button";
        public LocalizedString buttonText;
        public UnityEvent onClick;

        private Button uiButton;
        private Action clickHandler;

        public void Start()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            VisualElement buttonContainer = root.Q<VisualElement>(buttonName);
            uiButton = buttonContainer.Q<Button>();
            if (uiButton != null)
            {
                uiButton.text = buttonText.GetLocalizedString();
                clickHandler = () => onClick?.Invoke();
                uiButton.clicked += clickHandler;
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
                if (clickHandler != null) uiButton.clicked -= clickHandler;
            }
            LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
        }

    }
}