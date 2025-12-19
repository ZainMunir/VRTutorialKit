using System;
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

        void Awake()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            audioSource = GetComponent<AudioSource>();
        }

        void OnEnable()
        {
            root = GetComponent<UIDocument>().rootVisualElement;
        }


        public void RegisterButton(string buttonName, Func<bool> action)
        {
            root = GetComponent<UIDocument>().rootVisualElement;
            Button btn = root.Q<Button>(buttonName);
            if (btn == null) return;

            btn.AddToClassList(BaseClass);

            btn.clicked += () =>
            {
                bool success = action.Invoke();
                ApplyFeedback(btn, success);
            };
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