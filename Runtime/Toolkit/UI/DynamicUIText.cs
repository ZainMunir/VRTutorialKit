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

            UpdateUI(d_title, d_description);
        }

        void OnSelectedLocaleChanged(Locale newLocale)
        {
            UpdateUI(d_title, d_description);
        }

        public void UpdateUI(LocalizedString title, LocalizedString description)
        {
            Label titleLabel = root.Q<Label>("Title");
            Label descriptionLabel = root.Q<Label>("Description");

            if (titleLabel != null)
            {
                titleLabel.text = title.GetLocalizedString();
            }

            if (descriptionLabel != null)
            {
                descriptionLabel.text = description.GetLocalizedString();
            }
        }

        public void ResetUI()
        {
            UpdateUI(d_title, d_description);
        }
    }
}