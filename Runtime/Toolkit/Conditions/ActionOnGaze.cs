using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class ActionOnGaze : IConditionSource, ProgressProvider
    {
        [Header("Gaze Completion Settings")]
        [SerializeField] private float gazeDuration = 2f;
        [SerializeField] private float maxGazeAngle = 15f;
        private float gazeTimer = 0f;
        private Camera playerCamera;
        private bool isLookingAt;
        private bool hasCompleted;
        public float Progress => gazeDuration <= 0f ? 1f : Mathf.Clamp01(gazeTimer / gazeDuration);

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
            if (triggerOnce && hasTriggered) return;
            isLookingAt = IsPlayerLookingAt();

            if (isLookingAt)
            {
                gazeTimer += Time.deltaTime;
                if (!hasCompleted && gazeTimer >= gazeDuration)
                {
                    hasCompleted = true;
                    InvokeConditionAction();
                }

                SetConditionState(gazeTimer >= gazeDuration);
            }
            else
            {
                gazeTimer = 0f;
                hasCompleted = false;
                SetConditionState(false);
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
    }
}