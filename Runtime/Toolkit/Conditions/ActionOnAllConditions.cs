using System.Collections.Generic;
using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class ActionOnAllConditions : ConditionSourceBase, IProgressProvider
    {
        [SerializeField] private List<ConditionSourceBase> conditionSources = new List<ConditionSourceBase>();

        public float Progress => CalculateProgress();

        private float CalculateProgress()
        {
            if (conditionSources.Count == 0)
                return 0f;

            int metCount = 0;
            for (int index = 0; index < conditionSources.Count; index++)
            {
                if (conditionSources[index] != null && conditionSources[index].IsConditionMet)
                {
                    metCount++;
                }
            }

            return (float)metCount / conditionSources.Count;
        }

        private void Awake()
        {
            ValidateSources();
        }

        private void OnEnable()
        {
            SubscribeToSources();
            Refresh();
        }

        private void OnDisable()
        {
            UnsubscribeFromSources();
        }

        public void Refresh()
        {
            bool allMet = AreAllConditionsMet();
            SetConditionState(allMet);

            if (allMet)
            {
                InvokeConditionAction();
            }
        }

        private bool AreAllConditionsMet()
        {
            if (conditionSources.Count == 0)
                return false;

            for (int index = 0; index < conditionSources.Count; index++)
            {
                if (conditionSources[index] == null || !conditionSources[index].IsConditionMet)
                {
                    return false;
                }
            }

            return true;
        }

        private void SubscribeToSources()
        {
            if (conditionSources == null)
            {
                return;
            }

            for (int index = 0; index < conditionSources.Count; index++)
            {
                ConditionSourceBase source = conditionSources[index];
                if (source != null)
                {
                    source.ConditionStateChanged += OnSourceConditionStateChanged;
                }
            }
        }

        private void UnsubscribeFromSources()
        {
            for (int index = 0; index < conditionSources.Count; index++)
            {
                ConditionSourceBase source = conditionSources[index];
                if (source != null)
                {
                    source.ConditionStateChanged -= OnSourceConditionStateChanged;
                }
            }
        }

        private void ValidateSources()
        {
            if (conditionSources.Count == 0)
                return;


            for (int index = 0; index < conditionSources.Count; index++)
            {
                if (conditionSources[index] == this)
                {
                    Debug.LogError($"{nameof(ActionOnAllConditions)} on {name} cannot include itself as a condition source.", this);
                }
            }
        }

        private void OnSourceConditionStateChanged(bool _)
        {
            Refresh();
        }
    }
}