using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

namespace ECDA.VRTutorialKit
{
    public class ConfigUIController : MonoBehaviour
    {
        TutorialManager tutorialManager;
        RadioButtonGroup radioButtonGroup;
        RadioButtonGroup languageGroup;

        private EventCallback<ChangeEvent<int>> valueChangedCallback;
        private EventCallback<ChangeEvent<int>> languageChangedCallback;

        public List<TutorialConfig> configOptions = new List<TutorialConfig>();

        void Start()
        {
            tutorialManager = TutorialManager.Instance;
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            // --- Tutorial Config Setup ---
            radioButtonGroup = root.Q<RadioButtonGroup>("ConfigOptions");
            SetupConfigOptions();

            // --- Language Options Setup ---
            languageGroup = root.Q<RadioButtonGroup>("LanguageOptions");
            SetupLanguageOptions();
        }

        private void SetupConfigOptions()
        {
            string currentConfigName = tutorialManager.tutorialConfig.name;
            var configNames = new List<string>();

            for (int i = 0; i < configOptions.Count; i++)
            {
                configNames.Add(configOptions[i].name);
                if (configOptions[i].name == currentConfigName)
                {
                    radioButtonGroup.value = i;
                }
            }

            radioButtonGroup.choices = configNames;

            valueChangedCallback = evt =>
            {
                int selectedIndex = evt.newValue;
                if (selectedIndex >= 0 && selectedIndex < configOptions.Count)
                {
                    tutorialManager.SetTutorialConfig(configOptions[selectedIndex]);
                }
            };

            radioButtonGroup.RegisterValueChangedCallback(valueChangedCallback);
        }

        private void SetupLanguageOptions()
        {
            if (languageGroup == null) return;

            // 1. Get available languages from Localization Settings
            var locales = LocalizationSettings.AvailableLocales.Locales;
            var languageNames = new List<string>();
            int currentLocaleIndex = 0;

            for (int i = 0; i < locales.Count; i++)
            {
                languageNames.Add(locales[i].Identifier.CultureInfo.NativeName);

                if (locales[i] == LocalizationSettings.SelectedLocale)
                {
                    currentLocaleIndex = i;
                }
            }

            languageGroup.choices = languageNames;
            languageGroup.value = currentLocaleIndex;

            // 2. Define the callback to change language
            languageChangedCallback = evt =>
            {
                int selectedIndex = evt.newValue;
                if (selectedIndex >= 0 && selectedIndex < locales.Count)
                {
                    // Start the async operation to swap locales
                    LocalizationSettings.SelectedLocale = locales[selectedIndex];
                }
            };

            languageGroup.RegisterValueChangedCallback(languageChangedCallback);
        }

        void OnDestroy()
        {
            if (radioButtonGroup != null && valueChangedCallback != null)
            {
                radioButtonGroup.UnregisterValueChangedCallback(valueChangedCallback);
            }

            if (languageGroup != null && languageChangedCallback != null)
            {
                languageGroup.UnregisterValueChangedCallback(languageChangedCallback);
            }
        }
    }
}