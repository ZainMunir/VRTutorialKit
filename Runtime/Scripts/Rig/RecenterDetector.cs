using UnityEngine;
using UnityEngine.Events;
using Unity.XR.CoreUtils;

public class RecenterDetector : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How far must the camera jump in one frame to trigger a recenter? (In meters)")]
    public float movementThreshold = 0.05f;

    [Header("Events")]
    public UnityEvent OnRecenterDetected;

    private XROrigin xrOrigin;
    private Vector3 lastLocalPos;
    private bool isInitialized = false;

    void Start()
    {
        xrOrigin = FindFirstObjectByType<XROrigin>();

        if (xrOrigin != null)
        {
            lastLocalPos = xrOrigin.Camera.transform.localPosition;
            isInitialized = true;
        }
        else
        {
            Debug.LogWarning("XRRecenterDetector: No XROrigin found in scene. Detection disabled.");
        }
    }

    void Update()
    {
        if (!isInitialized) return;

        Vector3 currentLocalPos = xrOrigin.Camera.transform.localPosition;

        float frameDelta = Vector3.Distance(currentLocalPos, lastLocalPos);

        // A player walking physically cannot move 1+ meters in 1/60th of a second.
        // If they did, it's a recenter or a teleport.
        if (frameDelta > movementThreshold)
        {
            OnRecenterDetected?.Invoke();
            Debug.Log("XR Recenter Detected!");
        }

        lastLocalPos = currentLocalPos;
    }
}