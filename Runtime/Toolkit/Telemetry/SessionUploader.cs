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
    // With liveUpload on, the open session is also sent on a timer and left in place; the server
    // replaces a session's rows on every re-send, so the end-of-session upload has the last word.
    [RequireComponent(typeof(SessionLogger))]
    public class SessionUploader : MonoBehaviour, IBeforeSceneChange
    {
        private const float MinLiveUploadInterval = 10f;
        private const string ApiKeyResource = "SessionUploaderKey";


        [SerializeField] private string remoteEndpoint = "http(s)://PLACEHOLDER/api/ingest";
        [SerializeField] private string localEndpoint = "http://localhost:3000/api/ingest";
        [SerializeField] private bool useLocalEndpoint = false;

        [SerializeField] private int timeoutSeconds = 30;

        [Tooltip("Also upload the session while it is still running, so the dashboard can follow it. Off: sessions are only uploaded once they end.")]
        [SerializeField] private bool liveUpload;

        [Tooltip("Seconds between live uploads of the running session.")]
        [Min(MinLiveUploadInterval)]
        [SerializeField] private float liveUploadInterval = 60f;

        private SessionLogger logger;
        private string apiKey;
        private Coroutine liveRoutine;
        private bool uploading;
        private bool uploadRequested;
        private bool liveUploadRequested;

        private static string UploadedDirectory => Path.Combine(SessionLogger.SessionsDirectory, "uploaded");


        private string endpoint => useLocalEndpoint ? localEndpoint : remoteEndpoint;

        private void Awake()
        {
            logger = GetComponent<SessionLogger>();

            // Only read from a gitignored file in the consuming project, so the key never lands in a scene or prefab.
            var keyFile = Resources.Load<TextAsset>(ApiKeyResource);
            apiKey = keyFile != null ? keyFile.text.Trim() : string.Empty;
            if (apiKey.Length == 0)
                Debug.LogWarning($"[SessionUploader] No API key: add Resources/{ApiKeyResource}.txt containing the server's INGEST_KEY. Sessions stay on disk until then.");
        }

        private void OnEnable()
        {
            logger.SessionStarted += OnSessionStarted;
            logger.SessionEnded += OnSessionEnded;
            // startOnAwake opens the session before this component is listening.
            if (logger.IsLogging) OnSessionStarted(logger.CurrentSessionId);
        }

        private void OnDisable()
        {
            logger.SessionStarted -= OnSessionStarted;
            logger.SessionEnded -= OnSessionEnded;
            StopLiveUpload();
        }

        // Picks up anything left over from earlier runs (no network, app killed mid-upload).
        private void Start() => UploadPending();

        private void OnSessionStarted(string sessionId)
        {
            StopLiveUpload();
            if (liveUpload) liveRoutine = StartCoroutine(LiveUploadRoutine());
        }

        private void OnSessionEnded(string sessionId)
        {
            StopLiveUpload();
            UploadPending();
        }

        // Holds a SceneTransitionController scene change, at most timeoutSeconds, so an upload in
        // flight is not cut off with this scene. Anything unfinished is retried by the next uploader.
        public IEnumerator OnBeforeSceneChange()
        {
            // Under DontDestroyOnLoad the upload carries on in the next scene.
            if (gameObject.scene.buildIndex == -1) yield break;

            var deadline = Time.realtimeSinceStartup + timeoutSeconds;
            while (uploading && Time.realtimeSinceStartup < deadline) yield return null;
        }

        // Also callable from a UnityEvent, e.g. a "sync" button in the lobby.
        public void UploadPending()
        {
            uploadRequested = true;
            if (!uploading) StartCoroutine(UploadRoutine());
        }

        private void StopLiveUpload()
        {
            if (liveRoutine == null) return;
            StopCoroutine(liveRoutine);
            liveRoutine = null;
        }

        private IEnumerator LiveUploadRoutine()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(Mathf.Max(liveUploadInterval, MinLiveUploadInterval));
                liveUploadRequested = true;
                if (!uploading) StartCoroutine(UploadRoutine());
            }
        }

        // The only coroutine that talks to the server, so uploads never overlap. A session that ends
        // while its live upload is in flight is picked up by the pass after it, complete.
        private IEnumerator UploadRoutine()
        {
            if (apiKey.Length == 0) yield break;
            uploading = true;
            try
            {
                while (uploadRequested || liveUploadRequested)
                {
                    // A live pass sends only the open session and leaves the file where it is.
                    var live = liveUploadRequested;
                    if (live) liveUploadRequested = false;
                    else uploadRequested = false;
                    if (Application.internetReachability == NetworkReachability.NotReachable) continue;

                    foreach (var path in live ? OpenFile() : PendingFiles())
                    {
                        var sessionId = Path.GetFileNameWithoutExtension(path);

                        var operation = Send(path, sessionId);
                        if (operation == null) break;
                        using var request = operation.webRequest;
                        yield return operation;

                        if (request.result == UnityWebRequest.Result.Success)
                        {
                            if (live) continue;
                            MoveToUploaded(path);
                            Debug.Log($"[SessionUploader] Uploaded {sessionId}");
                            continue;
                        }

                        Debug.LogWarning($"[SessionUploader] {sessionId} failed: {request.error} {request.downloadHandler.text}");
                        // Server unreachable: the rest would fail the same way, try again next time.
                        if (request.result == UnityWebRequest.Result.ConnectionError) break;
                    }
                }
            }
            finally
            {
                uploading = false;
            }
        }

        // Builds and sends the request for both passes. Returns null when Unity refuses to send it,
        // which it reports by throwing from SendWebRequest rather than through request.result.
        private UnityWebRequestAsyncOperation Send(string path, string sessionId)
        {
            var request = new UnityWebRequest(endpoint, UnityWebRequest.kHttpVerbPOST)
            {
                uploadHandler = new UploadHandlerRaw(ReadShared(path)) { contentType = "application/x-ndjson" },
                downloadHandler = new DownloadHandlerBuffer(),
                timeout = timeoutSeconds,
            };

            try
            {
                request.SetRequestHeader("X-Api-Key", apiKey);
                request.SetRequestHeader("X-Session-Id", sessionId);
                return request.SendWebRequest();
            }
            catch (System.InvalidOperationException e)
            {
                request.Dispose();
                Debug.LogWarning(endpoint.StartsWith("http://", System.StringComparison.OrdinalIgnoreCase)
                    ? $"[SessionUploader] Upload skipped ({e.Message}): the endpoint is http:// and this build does not allow insecure connections. " +
                      "Set Player Settings > Other Settings > Allow downloads over HTTP > Always allowed, or use an https:// endpoint."
                    : $"[SessionUploader] Upload skipped: {e.Message}");
                return null;
            }
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

        private IEnumerable<string> OpenFile()
        {
            if (!logger.IsLogging) return new string[0];
            return new[] { Path.Combine(SessionLogger.SessionsDirectory, logger.CurrentSessionId + ".jsonl") };
        }

        // The logger still holds the open session's file for writing; File.ReadAllBytes would hit a sharing violation.
        private static byte[] ReadShared(string path)
        {
            using var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var buffer = new MemoryStream();
            stream.CopyTo(buffer);
            return buffer.ToArray();
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
