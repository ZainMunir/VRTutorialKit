using System.Diagnostics;
using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class InspectorDebug : MonoBehaviour
    {
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public void Log(string message) => UnityEngine.Debug.Log(message, this);

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public void LogWarning(string message) => UnityEngine.Debug.LogWarning(message, this);

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public void LogError(string message) => UnityEngine.Debug.LogError(message, this);
    }
}
