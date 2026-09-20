using System;
using UnityEngine;
using UnityEngine.Events;

namespace ECDA.VRTutorialKit
{
    public abstract class IConditionSource : MonoBehaviour
    {
        [Header("Condition State")]
        [SerializeField] private bool isConditionMet = false;
        public bool IsConditionMet => isConditionMet;
        public event Action<bool> ConditionStateChanged;
        public UnityEvent onAction;
        [SerializeField] protected bool triggerOnce = false;
        protected bool hasTriggered = false;
        private bool previousConditionState = false;

        protected void SetConditionState(bool conditionMet)
        {
            if (isConditionMet == conditionMet)
            {
                return;
            }

            isConditionMet = conditionMet;
            NotifyIfConditionStateChanged();
        }

        protected void InvokeConditionAction()
        {
            if (triggerOnce)
            {
                if (hasTriggered) return;
                hasTriggered = true;
            }

            onAction?.Invoke();
        }

        protected void ResetConditionActionGate()
        {
            hasTriggered = false;
        }

        protected void NotifyIfConditionStateChanged()
        {
            bool current = IsConditionMet;
            if (current == previousConditionState) return;

            previousConditionState = current;
            ConditionStateChanged?.Invoke(current);
        }
    }
}