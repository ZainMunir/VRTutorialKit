using UnityEngine;
using UnityEngine.Events;

namespace ECDA.VRTutorialKit
{
    public class ActionOnGaze : ProgressProvider
    {
        [Header("Gaze Completion Settings")]
        [SerializeField] private float gazeDuration = 2f;
        [SerializeField] private float maxGazeAngle = 15f;

        private float gazeTimer = 0f;
        private Camera playerCamera;

        public override float Progress => Mathf.Clamp01(gazeTimer / gazeDuration);

        public UnityEvent onGazeComplete;

        private bool completed = false;
        [SerializeField] private bool reuseOnComplete = false;

        private void Awake()
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                Debug.LogError($"Main Camera not found in the scene for {name}.");
            }
        }

        private void Update()
        {
            if (completed) return;

            if (IsPlayerLookingAt())
            {

                gazeTimer += Time.deltaTime;
                if (gazeTimer >= gazeDuration)
                {
                    completed = true;
                    onGazeComplete?.Invoke();
                    if (reuseOnComplete) Reset();
                }
            }
            else
            {
                gazeTimer = 0f;
            }
        }

        private bool IsPlayerLookingAt()
        {
            Vector3 camPos = playerCamera.transform.position;
            Vector3 targetPos = transform.position;

            Vector3 directionToObject = (targetPos - camPos).normalized;
            Vector3 cameraForward = playerCamera.transform.forward;

            float angle = Vector3.Angle(cameraForward, directionToObject);

            return angle < maxGazeAngle;
        }

        public void Reset()
        {
            completed = false;
            gazeTimer = 0f;
        }
    }
}