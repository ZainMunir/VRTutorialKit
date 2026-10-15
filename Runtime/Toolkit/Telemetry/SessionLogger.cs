using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;

namespace ECDA.VRTutorialKit
{
    // Writes one JSON Lines file per participant session to persistentDataPath/sessions.
    // On Quest that is /sdcard/Android/data/<package>/files/sessions (adb pull-able).
    // Every line is flushed as it is written, so a crash loses at most the line in progress.
    public class SessionLogger : SingletonBehaviour<SessionLogger>
    {
        [Tooltip("Start a session as soon as the app loads. Off: call StartSession yourself, e.g. from a start button.")]
        [SerializeField] private bool startOnAwake;

        [Tooltip("Keep this logger, and its session, through scene loads. Off: it closes its session when its scene unloads, e.g. a tutorial that hands over to an experience with its own logger.")]
        [SerializeField] private bool persistAcrossScenes;

        public static string SessionsDirectory => Path.Combine(Application.persistentDataPath, "sessions");

        public string CurrentSessionId { get; private set; }
        public bool IsLogging => writer != null;

        // Raised with the session id once its file is open and the session_start line is written.
        public event Action<string> SessionStarted;

        // Raised with the session id after its file is closed. SessionUploader listens to both.
        public event Action<string> SessionEnded;

        private StreamWriter writer;
        private readonly StringBuilder line = new StringBuilder(256);

        protected override bool PersistAcrossScenes => persistAcrossScenes;

        protected override void OnSingletonAwake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
            if (startOnAwake) StartSession();
        }

        protected override void OnDuplicateDestroyed()
        {
            Debug.Log($"[SessionLogger] A logger from an earlier scene is still running; the one in {gameObject.scene.name} is ignored.");
        }

        protected override void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
            EndSession();
            base.OnDestroy();
        }

        // Closes any open session first, so calling this on experience reset is enough.
        public void StartSession()
        {
            EndSession();

            // The random part keeps two headsets that start in the same second apart.
            CurrentSessionId = $"{DateTime.UtcNow:yyyyMMdd_HHmmss}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";

            Directory.CreateDirectory(SessionsDirectory);
            var path = Path.Combine(SessionsDirectory, CurrentSessionId + ".jsonl");
            // No BOM: the first line must be plain JSON.
            writer = new StreamWriter(path, true, new UTF8Encoding(false)) { AutoFlush = true };
            if (Application.isEditor) Debug.Log($"[SessionLogger] Writing to {Path.GetFullPath(path)}");

            var profile = ParticipantProfile.Instance;
            Log("session_start", new SessionInfo
            {
                profile = profile != null ? profile.GetSnapshot() : default,
                project = Application.productName,
                appVersion = Application.version,
                device = SystemInfo.deviceModel,
                scene = SceneManager.GetActiveScene().name,
                locale = LocaleCode(LocalizationSettings.SelectedLocale),
                isEditor = Application.isEditor,
            });

            SessionStarted?.Invoke(CurrentSessionId);
        }

        public void EndSession()
        {
            if (writer == null) return;

            Log("session_end");
            writer.Dispose();
            writer = null;

            var endedId = CurrentSessionId;
            CurrentSessionId = null;
            SessionEnded?.Invoke(endedId);
        }

        // For UnityEvents: logs an event with no payload.
        public void Log(string eventName) => Log(eventName, null);

        // data must be a [Serializable] class or struct (JsonUtility rules); null for no payload.
        public void Log(string eventName, object data)
        {
            if (writer == null) return;

            line.Clear();
            line.Append("{\"t\":").Append(Time.realtimeSinceStartupAsDouble.ToString("F3", CultureInfo.InvariantCulture))
                .Append(",\"utc\":\"").Append(DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture))
                .Append("\",\"event\":\"").Append(Escape(eventName)).Append('"');
            if (data != null) line.Append(",\"data\":").Append(JsonUtility.ToJson(data));
            line.Append('}');

            writer.WriteLine(line.ToString());
        }

        private void OnApplicationPause(bool paused)
        {
            // Quest suspends the app when the headset comes off; useful to see in study data.
            Log(paused ? "app_paused" : "app_resumed");
        }

        private void OnApplicationQuit() => EndSession();

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Log("scene_loaded", new SceneInfo { scene = scene.name });
        }

        // Raised only when the language is switched, not for the startup choice; session_start carries that.
        private void OnLocaleChanged(Locale locale)
        {
            Log("locale_changed", new LocaleInfo { locale = LocaleCode(locale) });
        }

        private static string LocaleCode(Locale locale) => locale != null ? locale.Identifier.Code : string.Empty;

        private static string Escape(string s) => (s ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");

        [Serializable]
        private struct SessionInfo
        {
            public ParticipantProfile.Snapshot profile;
            public string project, appVersion, device, scene, locale;
            public bool isEditor;
        }

        [Serializable]
        private struct SceneInfo
        {
            public string scene;
        }

        [Serializable]
        private struct LocaleInfo
        {
            public string locale;
        }
    }
}
