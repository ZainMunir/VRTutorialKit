using System;

namespace ECDA.VRTutorialKit
{
    public enum FingerType
    {
        Index = 0,
        Middle = 1,
        Ring = 2,
        Pinky = 3,
        Thumb = 4
    }

    public static class FingerConstants
    {
        public static readonly int Count = Enum.GetValues(typeof(FingerType)).Length;
    }
}
