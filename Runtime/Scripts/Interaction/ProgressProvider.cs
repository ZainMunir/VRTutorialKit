using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public abstract class ProgressProvider : MonoBehaviour
    {
        public abstract float Progress { get; }
    }
}