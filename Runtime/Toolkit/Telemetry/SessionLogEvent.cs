using System;
using System.Globalization;
using UnityEngine;

namespace ECDA.VRTutorialKit
{
    // Inspector hook: wire Log() into any UnityEvent (select exited, beat complete, button press)
    // to record that moment in the session log without writing code.
    public class SessionLogEvent : MonoBehaviour
    {
        [SerializeField] private string eventName = "event";

        public void Log() => Log(string.Empty);

        // The string version lets a UnityEvent pass extra context, e.g. a dialogue option id.
        public void Log(string detail)
        {
            if (SessionLogger.Instance == null) return;
            SessionLogger.Instance.Log(eventName, new Payload { source = gameObject.name, detail = detail });
        }

        // Typed values go into detail as invariant text, so the payload shape never changes.
        public void Log(bool value) => Log(value ? "true" : "false");

        public void Log(int value) => Log(value.ToString(CultureInfo.InvariantCulture));

        public void Log(float value) => Log(value.ToString("R", CultureInfo.InvariantCulture));

        [Serializable]
        private struct Payload
        {
            public string source, detail;
        }
    }
}
