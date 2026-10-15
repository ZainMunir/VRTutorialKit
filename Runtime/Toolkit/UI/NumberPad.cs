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
        // Set both to the same value to require an exact number of digits.
        [Min(1)]
        [SerializeField] private int minLength = 1;
        [Min(1)]
        [SerializeField] private int maxLength = 6;

        public UnityEvent<string> onSubmitted;

        private readonly List<(Button button, Action action)> bindings = new List<(Button, Action)>();
        private readonly List<Button> digitButtons = new List<Button>();
        private Label display;
        private Button backspaceButton;
        private Button submitButton;
        private string value = string.Empty;

        public string Value => value;

        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            display = root.Q<Label>("Display");

            for (var d = 0; d <= 9; d++)
            {
                var digit = (char)('0' + d);
                var button = Bind(root, $"Digit{d}", () => Append(digit));
                if (button != null) digitButtons.Add(button);
            }
            backspaceButton = Bind(root, "Backspace", Backspace);
            submitButton = Bind(root, "Submit", Submit);

            if (display == null) Debug.LogWarning($"[NumberPad] No Label named Display on {name}.");
            Refresh();
        }

        private void OnDisable()
        {
            foreach (var (button, action) in bindings) button.clicked -= action;
            bindings.Clear();
            digitButtons.Clear();
        }

        private void OnValidate()
        {
            maxLength = Mathf.Max(maxLength, minLength);
        }

        private Button Bind(VisualElement root, string buttonName, Action action)
        {
            var button = root.Q<Button>(buttonName);
            if (button == null)
            {
                Debug.LogWarning($"[NumberPad] No Button named {buttonName} on {name}.");
                return null;
            }
            button.clicked += action;
            bindings.Add((button, action));
            return button;
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
            if (!IsValidLength) return;
            onSubmitted?.Invoke(value);
        }

        private bool IsValidLength => value.Length >= minLength && value.Length <= maxLength;

        private void Refresh()
        {
            if (display != null) display.text = value;
            var canAppend = value.Length < maxLength;
            foreach (var button in digitButtons) button.SetEnabled(canAppend);
            backspaceButton?.SetEnabled(value.Length > 0);
            submitButton?.SetEnabled(IsValidLength);
        }
    }
}
