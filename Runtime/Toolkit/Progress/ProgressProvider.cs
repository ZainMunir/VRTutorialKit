using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public interface ProgressProvider
    {
        public abstract float Progress { get; }
    }
}