using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace ECDA.VRTutorialKit
{
    // Digit-only entry for a world-space panel; the system keyboard is unreliable in headset.
    // Expects Buttons named Digit0-Digit9, Backspace and Submit, and a Label named Display.
    [RequireComponent(typeof(UIDocument))]
    public class NumberPad : MonoBehaviour
    {
        [Min(1)]
        [SerializeField] private int maxLength = 6;

        public UnityEvent<string> onSubmitted;

        private readonly List<(Button button, Action action)> bindings = new List<(Button, Action)>();
        private Label display;
        private Button submitButton;
        private string value = string.Empty;

        public string Value => value;

        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            display = root.Q<Label>("Display");
            submitButton = root.Q<Button>("Submit");

            for (var d = 0; d <= 9; d++)
            {
                var digit = (char)('0' + d);
                Bind(root, $"Digit{d}", () => Append(digit));
            }
            Bind(root, "Backspace", Backspace);
            Bind(root, "Submit", Submit);

            if (display == null) Debug.LogWarning($"[NumberPad] No Label named Display on {name}.");
            Refresh();
        }

        private void OnDisable()
        {
            foreach (var (button, action) in bindings) button.clicked -= action;
            bindings.Clear();
        }

        private void Bind(VisualElement root, string buttonName, Action action)
        {
            var button = root.Q<Button>(buttonName);
            if (button == null)
            {
                Debug.LogWarning($"[NumberPad] No Button named {buttonName} on {name}.");
                return;
            }
            button.clicked += action;
            bindings.Add((button, action));
        }

        public void Append(char digit)
        {
            if (digit < '0' || digit > '9' || value.Length >= maxLength) return;
            value += digit;
            Refresh();
        }

        public void Backspace()
        {
            if (value.Length == 0) return;
            value = value.Substring(0, value.Length - 1);
            Refresh();
        }

        public void Clear()
        {
            value = string.Empty;
            Refresh();
        }

        public void Submit()
        {
            if (value.Length == 0) return;
            onSubmitted?.Invoke(value);
        }

        private void Refresh()
        {
            if (display != null) display.text = value;
            submitButton?.SetEnabled(value.Length > 0);
        }
    }
}
