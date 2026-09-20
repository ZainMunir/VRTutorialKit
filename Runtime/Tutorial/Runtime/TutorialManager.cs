using UnityEngine;
using System;


namespace ECDA.VRTutorialKit
{

    public class TutorialManager : SingletonBehaviour<TutorialManager>
    {
        public TutorialConfig tutorialConfig;

        private int currentStepIndex = 0;

        private bool[] stepsCompleted;

        public Action<bool> OnStepCompleted;

        public Action<bool> OnTutorialFinished;

        public Action<bool> OnTutorialStepChanged;


        void Start()
        {
            if (tutorialConfig != null)
            {
                stepsCompleted = new bool[TotalSteps()];
                CheckImmediateCompletion();
            }
        }

        public int TotalSteps()
        {
            if (tutorialConfig == null)
                return 0;

            return tutorialConfig.tutorialSteps.Count;
        }

        bool CheckBounds(int index)
        {
            return index >= 0 && index < TotalSteps();
        }

        public TutorialStep GetCurrentStep()
        {
            if (tutorialConfig == null || TotalSteps() == 0)
                return null;

            return tutorialConfig.tutorialSteps[currentStepIndex];
        }

        public bool HasPreviousStep => currentStepIndex > 0;
        public bool HasNextStep => currentStepIndex < TotalSteps() - 1;
        public bool IsCurrentStepCompleted => stepsCompleted != null && CheckBounds(currentStepIndex) && stepsCompleted[currentStepIndex];

        void CheckImmediateCompletion()
        {
            var step = GetCurrentStep();
            if (step != null && step.immediateCompletion)
            {
                CompleteStep();
            }
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
                OnTutorialStepChanged?.Invoke(IsCurrentStepCompleted);
                CheckImmediateCompletion();
            }
        }

        [ContextMenu("Finish Tutorial")]
        public void FinishTutorial()
        {
            Debug.Log("Tutorial Finished!");
            var transitionController = SceneTransitionController.Instance;
            if (transitionController != null)
            {
                transitionController.GoToScene(tutorialConfig.startingScene);
            }
        }

        [ContextMenu("SkipToNextStep")]
        public void SkipToNextStep()
        {
            if (!HasNextStep)
                return;
            currentStepIndex++;
            if (CheckBounds(currentStepIndex))
            {
                OnTutorialStepChanged?.Invoke(IsCurrentStepCompleted);
                CheckImmediateCompletion();
            }
        }

        public void SetTutorialConfig(TutorialConfig config)
        {
            tutorialConfig = config;
            currentStepIndex = 0;
            if (tutorialConfig != null)
            {
                stepsCompleted = new bool[TotalSteps()];
                CheckImmediateCompletion();
                OnTutorialFinished?.Invoke(false);
                OnTutorialStepChanged?.Invoke(IsCurrentStepCompleted);
            }
        }
    }
}
