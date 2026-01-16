using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ECDA.VRTutorialKit
{
    public class ConfigUIController : MonoBehaviour
    {
        TutorialManager tutorialManager;
        RadioButtonGroup radioButtonGroup;
        private EventCallback<ChangeEvent<int>> valueChangedCallback;

        public List<TutorialConfig> configOptions = new List<TutorialConfig>();

        void Start()
        {
            tutorialManager = TutorialManager.Instance;
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            radioButtonGroup = root.Q<RadioButtonGroup>("ConfigOptions");

            int currentIndex = -1;
            string currentConfigName = tutorialManager.tutorialConfig.name;

            var configNames = new List<string>();
            foreach (var config in configOptions)
            {
                currentIndex++;
                configNames.Add(config.name);
                if (config.name == currentConfigName)
                {
                    radioButtonGroup.value = currentIndex;
                }
            }

            radioButtonGroup.choices = configNames;

            valueChangedCallback = evt =>
            {
                int selectedIndex = evt.newValue;
                if (selectedIndex >= 0 && selectedIndex < configOptions.Count)
                {
                    tutorialManager.tutorialConfig = configOptions[selectedIndex];
                }
            };

            radioButtonGroup.RegisterValueChangedCallback(valueChangedCallback);
        }

        void OnDestroy()
        {
            if (radioButtonGroup != null && valueChangedCallback != null)
            {
                radioButtonGroup.UnregisterValueChangedCallback(valueChangedCallback);
            }
        }
    }
}