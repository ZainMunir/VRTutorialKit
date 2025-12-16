using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System;


namespace ECDA.VRTutorialKit
{

    public class TutorialManager : MonoBehaviour
    {
        private static TutorialManager _instance;
        public static TutorialManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<TutorialManager>();
                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject("TutorialManager");
                        _instance = singletonObject.AddComponent<TutorialManager>();
                        DontDestroyOnLoad(singletonObject);
                    }
                }
                return _instance;
            }
        }

        public TutorialConfig tutorialConfig;

        private int currentStepIndex = 0;

        private bool[] stepsCompleted;

        public Action<bool> OnStepCompleted;

        public Action<bool> OnTutorialFinished;

        public Action<bool> OnTutorialStepChanged;

        public bool HasPreviousStep => currentStepIndex > 0;
        public bool HasNextStep => currentStepIndex < TotalSteps() - 1;
        public bool IsCurrentStepCompleted => stepsCompleted != null && CheckBounds(currentStepIndex) && stepsCompleted[currentStepIndex];

        public TutorialStep GetCurrentStep()
        {
            if (tutorialConfig == null || TotalSteps() == 0)
                return null;

            return tutorialConfig.tutorialSteps[currentStepIndex];
        }

        public int TotalSteps()
        {
            if (tutorialConfig == null)
                return 0;

            return tutorialConfig.tutorialSteps.Count;
        }


        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            if (tutorialConfig != null)
            {
                stepsCompleted = new bool[TotalSteps()];
            }

            LoadCurrentStep();

        }

        void LoadCurrentStep()
        {
            // Tooltips are now handled by TooltipController listening to events
        }

        bool CheckBounds(int index)
        {
            return index >= 0 && index < TotalSteps();
        }

        [ContextMenu("Complete Step")]
        public void CompleteStep()
        {
            if (CheckBounds(currentStepIndex))
            {
                if (IsCurrentStepCompleted)
                    return;
                stepsCompleted[currentStepIndex] = true;
                OnStepCompleted?.Invoke(true);
                if (!HasNextStep)
                {
                    OnTutorialFinished?.Invoke(true);
                }
            }
        }

        public void PreviousStep()
        {
            if (!HasPreviousStep)
                return;
            currentStepIndex--;
            if (CheckBounds(currentStepIndex))
            {
                LoadCurrentStep();
                OnTutorialStepChanged?.Invoke(IsCurrentStepCompleted);
            }
        }

        public void NextStep()
        {
            if (!HasNextStep)
                return;
            if (!IsCurrentStepCompleted)
                return;
            currentStepIndex++;
            if (CheckBounds(currentStepIndex))
            {
                LoadCurrentStep();
                OnTutorialStepChanged?.Invoke(IsCurrentStepCompleted);
            }
        }


        [ContextMenu("Finish Tutorial")]
        public void FinishTutorial()
        {
            Debug.Log("Tutorial Finished!");
            SceneManager.LoadScene(tutorialConfig.startingScene);
        }
    }
}
