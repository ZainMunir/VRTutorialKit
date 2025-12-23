using UnityEngine;
using UnityEngine.UIElements;

namespace ECDA.VRTutorialKit
{
    [RequireComponent(typeof(UIDocument))]
    public class DynamicUIText : MonoBehaviour
    {
        public string title;
        [TextArea(3, 10)]
        public string description;
        void Start()
        {
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;

            Label titleLabel = root.Q<Label>("Title");
            Label descriptionLabel = root.Q<Label>("Description");

            if (titleLabel != null)
            {
                titleLabel.text = title;
            }

            if (descriptionLabel != null)
            {
                descriptionLabel.text = description;
            }
        }
    }
}