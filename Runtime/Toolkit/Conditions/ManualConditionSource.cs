using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class ManualConditionSource : ConditionSourceBase
    {
        public void SetConditionMet(bool conditionMet)
        {
            SetConditionState(conditionMet);

            if (conditionMet)
                InvokeConditionAction();
            else
                ResetConditionActionGate();
        }

        [ContextMenu("Set Met")]
        public void SetMet() => SetConditionMet(true);

        [ContextMenu("Set Not Met")]
        public void SetNotMet() => SetConditionMet(false);
    }
}
