using System;
using System.Collections.Generic;
using UnityEngine;

namespace ECDA.VRTutorialKit
{
    // Records the tutorial's flow in the session log, and ends the session when the tutorial is exited.
    // Runs before the tutorial's components so the first step is logged before anything can complete it.
    [DefaultExecutionOrder(-100)]
    public class TutorialSessionLog : MonoBehaviour
    {
        private TutorialManager tutorialManager;
        private double tutorialStartedAt;
        private double stepStartedAt;
        private readonly Dictionary<TutorialSubStep, double> subStepActivatedAt = new Dictionary<TutorialSubStep, double>();

        private static double Now => Time.realtimeSinceStartupAsDouble;

        private void OnEnable()
        {
            TutorialSubStep.AnyActivated += OnSubStepActivated;
            TutorialSubStep.AnyCompleted += OnSubStepCompleted;
        }

        private void OnDisable()
        {
            TutorialSubStep.AnyActivated -= OnSubStepActivated;
            TutorialSubStep.AnyCompleted -= OnSubStepCompleted;
        }

        private void Start()
        {
            tutorialManager = TutorialManager.Instance;
            if (tutorialManager == null)
            {
                Debug.LogError("[TutorialSessionLog] TutorialManager instance not found.");
                return;
            }

            tutorialManager.OnTutorialStepChanged += OnStepChanged;
            tutorialManager.OnStepCompleted += OnStepCompleted;
            tutorialManager.OnTutorialFinished += OnAllStepsCompleted;
            tutorialManager.OnTutorialConfigChanged += OnConfigChanged;
            tutorialManager.OnTutorialExited += OnExited;

            tutorialStartedAt = Now;
            Log("tutorial_started", new TutorialInfo
            {
                config = ConfigName(tutorialManager.tutorialConfig),
                stepCount = tutorialManager.TotalSteps(),
            });
            LogStepStarted("first");
        }

        private void OnDestroy()
        {
            if (tutorialManager == null) return;

            tutorialManager.OnTutorialStepChanged -= OnStepChanged;
            tutorialManager.OnStepCompleted -= OnStepCompleted;
            tutorialManager.OnTutorialFinished -= OnAllStepsCompleted;
            tutorialManager.OnTutorialConfigChanged -= OnConfigChanged;
            tutorialManager.OnTutorialExited -= OnExited;
        }

        private void OnStepChanged(bool _)
        {
            switch (tutorialManager.LastStepChangeReason)
            {
                case TutorialManager.StepChangeReason.Next: LogStepStarted("next"); break;
                case TutorialManager.StepChangeReason.Previous: LogStepStarted("previous"); break;
                case TutorialManager.StepChangeReason.Skip: LogStepStarted("skip"); break;
                // ConfigChanged is logged by OnConfigChanged, which runs before the new first step can complete.
            }
        }

        private void LogStepStarted(string direction)
        {
            stepStartedAt = Now;
            subStepActivatedAt.Clear();
            Log("step_started", new StepStartedInfo
            {
                index = tutorialManager.CurrentStepIndex,
                step = StepName(tutorialManager.GetCurrentStep()),
                direction = direction,
            });
        }

        private void OnStepCompleted(bool _)
        {
            Log("step_completed", new StepCompletedInfo
            {
                index = tutorialManager.CurrentStepIndex,
                step = StepName(tutorialManager.GetCurrentStep()),
                seconds = (float)(Now - stepStartedAt),
            });
        }

        // SetTutorialConfig also raises OnTutorialFinished(false) to reset listeners; only true means done.
        private void OnAllStepsCompleted(bool finished)
        {
            if (!finished) return;
            Log("tutorial_all_steps_completed", new DurationInfo { seconds = (float)(Now - tutorialStartedAt) });
        }

        private void OnConfigChanged(TutorialConfig config)
        {
            Log("config_selected", new ConfigInfo { config = ConfigName(config), stepCount = tutorialManager.TotalSteps() });
            LogStepStarted("config_changed");
        }

        private void OnExited(TutorialManager.ExitReason reason)
        {
            Log("tutorial_exited", new ExitInfo
            {
                reason = reason == TutorialManager.ExitReason.Skipped ? "skipped" : "finished",
                lastStepIndex = tutorialManager.CurrentStepIndex,
                lastStep = StepName(tutorialManager.GetCurrentStep()),
                seconds = (float)(Now - tutorialStartedAt),
            });

            var logger = SessionLogger.Instance;
            if (logger != null) logger.EndSession();
        }

        private void OnSubStepActivated(TutorialSubStep subStep)
        {
            subStepActivatedAt[subStep] = Now;
            Log("substep_active", new SubStepInfo { stepIndex = CurrentStepIndex, subStep = subStep.name });
        }

        private void OnSubStepCompleted(TutorialSubStep subStep)
        {
            // -1 when the substep was completed without ever being activated.
            float seconds = subStepActivatedAt.TryGetValue(subStep, out double activatedAt) ? (float)(Now - activatedAt) : -1f;
            subStepActivatedAt.Remove(subStep);
            Log("substep_completed", new SubStepCompletedInfo { stepIndex = CurrentStepIndex, subStep = subStep.name, seconds = seconds });
        }

        private int CurrentStepIndex => tutorialManager != null ? tutorialManager.CurrentStepIndex : -1;

        private static string ConfigName(TutorialConfig config) => config != null ? config.name : string.Empty;

        private static string StepName(TutorialStep step) => step != null ? step.name : string.Empty;

        private static void Log(string eventName, object data)
        {
            var logger = SessionLogger.Instance;
            if (logger != null) logger.Log(eventName, data);
        }

        [Serializable]
        private struct TutorialInfo
        {
            public string config;
            public int stepCount;
        }

        [Serializable]
        private struct ConfigInfo
        {
            public string config;
            public int stepCount;
        }

        [Serializable]
        private struct StepStartedInfo
        {
            public int index;
            public string step, direction;
        }

        [Serializable]
        private struct StepCompletedInfo
        {
            public int index;
            public string step;
            public float seconds;
        }

        [Serializable]
        private struct SubStepInfo
        {
            public int stepIndex;
            public string subStep;
        }

        [Serializable]
        private struct SubStepCompletedInfo
        {
            public int stepIndex;
            public string subStep;
            public float seconds;
        }

        [Serializable]
        private struct DurationInfo
        {
            public float seconds;
        }

        [Serializable]
        private struct ExitInfo
        {
            public string reason;
            public int lastStepIndex;
            public string lastStep;
            public float seconds;
        }
    }
}
