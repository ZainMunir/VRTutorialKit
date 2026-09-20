using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public interface IProgressProvider
    {
        public abstract float Progress { get; }
    }
}