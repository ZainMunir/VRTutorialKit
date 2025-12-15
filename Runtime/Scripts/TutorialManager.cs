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

        public TooltipController leftTooltipController;
        public TooltipController rightTooltipController;

        [SerializeField] private InputActionReference _leftActivateAction;
        [SerializeField] private InputActionReference _rightActivateAction;
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

            _leftActivateAction.action.performed += ctx => CycleStep();
            _rightActivateAction.action.performed += ctx => FinishTutorial();
        }

        void LoadCurrentStep()
        {
            TutorialStep step = GetCurrentStep();

            switch (step.tooltipHand)
            {
                case TutorialStep.TooltipHand.Left:
                    leftTooltipController.ReplaceTooltip(step.tooltipPrefabs);
                    rightTooltipController.RemoveAllTooltips();
                    break;
                case TutorialStep.TooltipHand.Right:
                    rightTooltipController.ReplaceTooltip(step.tooltipPrefabs);
                    leftTooltipController.RemoveAllTooltips();
                    break;
                case TutorialStep.TooltipHand.Both:
                    leftTooltipController.ReplaceTooltip(step.tooltipPrefabs);
                    rightTooltipController.ReplaceTooltip(step.tooltipPrefabs);
                    break;
            }
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
                if (stepsCompleted[currentStepIndex])
                    return;
                stepsCompleted[currentStepIndex] = true;
                OnStepCompleted?.Invoke(true);
                if (currentStepIndex == TotalSteps() - 1)
                {
                    OnTutorialFinished?.Invoke(true);
                }
            }
        }

        public void PreviousStep()
        {
            if (currentStepIndex <= 0)
                return;
            currentStepIndex--;
            if (CheckBounds(currentStepIndex))
            {
                LoadCurrentStep();
                OnTutorialStepChanged?.Invoke(stepsCompleted[currentStepIndex]);
            }
        }

        public void NextStep()
        {
            if (currentStepIndex >= TotalSteps() - 1)
                return;
            if (!stepsCompleted[currentStepIndex])
                return;
            currentStepIndex++;
            if (CheckBounds(currentStepIndex))
            {
                LoadCurrentStep();
                OnTutorialStepChanged?.Invoke(stepsCompleted[currentStepIndex]);
            }
        }


        [ContextMenu("Finish Tutorial")]
        public void FinishTutorial()
        {
            Debug.Log("Tutorial Finished!");
            SceneManager.LoadScene(tutorialConfig.startingScene);
        }

        void CycleStep()
        {
            currentStepIndex = (currentStepIndex + 1) % TotalSteps();
            LoadCurrentStep();
        }
    }
}
