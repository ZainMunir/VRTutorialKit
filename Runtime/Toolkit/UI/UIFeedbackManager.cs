using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace ECDA.VRTutorialKit
{

    [RequireComponent(typeof(UIDocument), typeof(AudioSource))]
    public class UIFeedbackManager : MonoBehaviour
    {
        [Header("Audio")]
        public AudioClip successSound;
        public AudioClip failSound;
        private VisualElement root;
        private AudioSource audioSource;

        private const string BaseClass = "btn-feedback";
        private const string SuccessClass = "btn-feedback--success";
        private const string FailClass = "btn-feedback--fail";

        public int durationMs = 500;

        private readonly Dictionary<string, (Button button, Action handler)> handlers =
            new Dictionary<string, (Button, Action)>();

        void Awake()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            audioSource = GetComponent<AudioSource>();
        }

        void OnEnable()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
        }

        void OnDisable()
        {
            UnregisterAll();
        }


        public void RegisterButton(string buttonName, Func<bool> action)
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            Button btn = root.Q<Button>(buttonName);
            if (btn == null) return;

            UnregisterButton(buttonName);

            btn.AddToClassList(BaseClass);

            Action handler = () =>
            {
                bool success = action.Invoke();
                ApplyFeedback(btn, success);
            };

            btn.clicked += handler;
            handlers[buttonName] = (btn, handler);
        }

        public void UnregisterButton(string buttonName)
        {
            if (!handlers.TryGetValue(buttonName, out var entry)) return;

            if (entry.button != null) entry.button.clicked -= entry.handler;
            handlers.Remove(buttonName);
        }

        private void UnregisterAll()
        {
            foreach (var entry in handlers.Values)
            {
                if (entry.button != null) entry.button.clicked -= entry.handler;
            }
            handlers.Clear();
        }

        private void ApplyFeedback(Button btn, bool success)
        {
            string classToAdd = success ? SuccessClass : FailClass;
            AudioClip clip = success ? successSound : failSound;

            if (clip != null) audioSource.PlayOneShot(clip);

            btn.AddToClassList(classToAdd);

            btn.schedule.Execute(() =>
            {
                btn.RemoveFromClassList(classToAdd);
            }).ExecuteLater(durationMs);
        }
    }
}