using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;
using UnityEngine.Localization.Settings;

namespace ECDA.VRTutorialKit
{
    [RequireComponent(typeof(UIDocument))]
    public class DynamicUIText : MonoBehaviour
    {
        public LocalizedString d_title;
        public LocalizedString d_description;
        VisualElement root;
        void Start()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;

            UpdateUI();
        }

        void OnSelectedLocaleChanged(Locale newLocale)
        {
            UpdateUI();
        }

        void UpdateUI()
        {
            Label titleLabel = root.Q<Label>("Title");
            Label descriptionLabel = root.Q<Label>("Description");

            if (titleLabel != null)
            {
                titleLabel.text = d_title.GetLocalizedString();
            }

            if (descriptionLabel != null)
            {
                descriptionLabel.text = d_description.GetLocalizedString();
            }
        }
    }
}