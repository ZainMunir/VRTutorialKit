using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace ECDA.VRTutorialKit
{
    // Sends finished session files to the telemetry server, then moves them to sessions/uploaded.
    // Failed uploads stay where they are and are retried on next launch or the next session end,
    // so the files on the headset remain the source of truth.
    [RequireComponent(typeof(SessionLogger))]
    public class SessionUploader : MonoBehaviour
    {

        [SerializeField] private string endpoint = "http(s)://PLACEHOLDER/api/ingest";

        [Tooltip("Must match INGEST_KEY on the server. Ships in the APK, so it only keeps random traffic out.")]
        [SerializeField] private string apiKey = "change-me";

        [SerializeField] private int timeoutSeconds = 30;

        private SessionLogger logger;
        private bool uploading;
        private bool uploadRequested;

        private static string UploadedDirectory => Path.Combine(SessionLogger.SessionsDirectory, "uploaded");

        private void Awake() => logger = GetComponent<SessionLogger>();
        private void OnEnable() => logger.SessionEnded += OnSessionEnded;
        private void OnDisable() => logger.SessionEnded -= OnSessionEnded;

        // Picks up anything left over from earlier runs (no network, app killed mid-upload).
        private void Start() => UploadPending();

        private void OnSessionEnded(string sessionId) => UploadPending();

        // Also callable from a UnityEvent, e.g. a "sync" button in the lobby.
        public void UploadPending()
        {
            if (uploading)
            {
                uploadRequested = true;
                return;
            }
            StartCoroutine(UploadRoutine());
        }

        private IEnumerator UploadRoutine()
        {
            uploading = true;
            do
            {
                uploadRequested = false;
                if (Application.internetReachability == NetworkReachability.NotReachable) break;

                foreach (var path in PendingFiles())
                {
                    var sessionId = Path.GetFileNameWithoutExtension(path);

                    using var request = new UnityWebRequest(endpoint, UnityWebRequest.kHttpVerbPOST)
                    {
                        uploadHandler = new UploadHandlerRaw(File.ReadAllBytes(path)) { contentType = "application/x-ndjson" },
                        downloadHandler = new DownloadHandlerBuffer(),
                        timeout = timeoutSeconds,
                    };
                    request.SetRequestHeader("X-Api-Key", apiKey);
                    request.SetRequestHeader("X-Session-Id", sessionId);

                    yield return request.SendWebRequest();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        MoveToUploaded(path);
                        Debug.Log($"[SessionUploader] Uploaded {sessionId}");
                        continue;
                    }

                    Debug.LogWarning($"[SessionUploader] {sessionId} failed: {request.error} {request.downloadHandler.text}");
                    // Server unreachable: the rest would fail the same way, try again next time.
                    if (request.result == UnityWebRequest.Result.ConnectionError) break;
                }
            } while (uploadRequested);
            uploading = false;
        }

        // Oldest first (file names start with a UTC timestamp); skips the session still being written.
        private IEnumerable<string> PendingFiles()
        {
            var dir = SessionLogger.SessionsDirectory;
            if (!Directory.Exists(dir)) return new string[0];

            var files = new List<string>(Directory.GetFiles(dir, "*.jsonl"));
            files.RemoveAll(f => Path.GetFileNameWithoutExtension(f) == logger.CurrentSessionId);
            files.Sort(System.StringComparer.Ordinal);
            return files;
        }

        private static void MoveToUploaded(string path)
        {
            Directory.CreateDirectory(UploadedDirectory);
            var destination = Path.Combine(UploadedDirectory, Path.GetFileName(path));
            if (File.Exists(destination)) File.Delete(destination);
            File.Move(path, destination);
        }
    }
}
