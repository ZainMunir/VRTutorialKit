using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ECDA.VRTutorialKit
{
    public class TutorialStepController : MonoBehaviour
    {
        public List<TutorialSubStep> subSteps = new List<TutorialSubStep>();
        public bool isOrdered = true;
        public UnityEvent onStepCompleted;
        private int m_CurrentSubStepIndex = 0;

        private void Start()
        {
            InitializeSubSteps();
        }

        private void InitializeSubSteps()
        {
            if (subSteps.Count == 0) return;

            foreach (var step in subSteps)
            {
                step.Initialize(this);
            }

            if (isOrdered)
            {
                ActivateSubStep(0);
            }
            else
            {
                foreach (var step in subSteps)
                {
                    step.SetActive(true);
                }
            }
        }

        private void ActivateSubStep(int index)
        {
            if (index >= 0 && index < subSteps.Count)
            {
                subSteps[index].SetActive(true);
            }
        }

        public void OnSubStepCompleted(TutorialSubStep subStep)
        {
            if (isOrdered)
            {
                if (subSteps.IndexOf(subStep) == m_CurrentSubStepIndex)
                {
                    m_CurrentSubStepIndex++;
                    if (m_CurrentSubStepIndex < subSteps.Count)
                    {
                        ActivateSubStep(m_CurrentSubStepIndex);
                    }
                    else
                    {
                        CompleteCurrentStep();
                    }
                }
            }
            else
            {
                bool allCompleted = true;
                foreach (var step in subSteps)
                {
                    if (!step.IsCompleted)
                    {
                        allCompleted = false;
                        break;
                    }
                }

                if (allCompleted)
                {
                    CompleteCurrentStep();
                }
            }
        }

        [ContextMenu("Complete Step")]
        public void CompleteCurrentStep()
        {
            onStepCompleted?.Invoke();
            if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.CompleteStep();
            }
        }
    }
}