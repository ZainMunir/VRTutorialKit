using System.Collections.Generic;
using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class GripPoseProvider : MonoBehaviour
    {
        [Header("Grip")]
        [SerializeField] private HandPose defaultPose;
        [SerializeField] private HandPose leftPoseOverride;
        [SerializeField] private HandPose rightPoseOverride;
        private readonly List<HandPoseDriver> holders = new List<HandPoseDriver>();
        private HandPose activeActionPose;
        public bool IsHeld => holders.Count > 0;

        public HandPose GetPose(bool isLeftHand)
        {
            HandPose overridePose = isLeftHand ? leftPoseOverride : rightPoseOverride;
            return overridePose != null ? overridePose : defaultPose;
        }

        public void RegisterHolder(HandPoseDriver driver)
        {
            if (driver == null || holders.Contains(driver)) return;

            holders.Add(driver);
            ApplyPoses(driver);
        }

        public void UnregisterHolder(HandPoseDriver driver)
        {
            if (driver == null) return;

            holders.Remove(driver);
            driver.ClearGripPose();
        }

        public void SetActionPose(HandPose pose)
        {
            activeActionPose = pose;

            for (int i = 0; i < holders.Count; i++)
            {
                holders[i].SetActionPose(pose);
            }
        }

        public void ClearActionPose()
        {
            activeActionPose = null;

            for (int i = 0; i < holders.Count; i++)
            {
                holders[i].ClearActionPose();
            }
        }

        private void ApplyPoses(HandPoseDriver driver)
        {
            driver.SetGripPose(GetPose(driver.IsLeftHand));

            if (activeActionPose != null)
            {
                driver.SetActionPose(activeActionPose);
            }
        }

        private void OnEnable()
        {
            for (int i = holders.Count - 1; i >= 0; i--)
            {
                if (holders[i] == null) holders.RemoveAt(i);
                else ApplyPoses(holders[i]);
            }
        }

        private void OnDisable()
        {
            for (int i = holders.Count - 1; i >= 0; i--)
            {
                if (holders[i] == null) holders.RemoveAt(i);
                else holders[i].ClearGripPose();
            }
        }
    }
}
