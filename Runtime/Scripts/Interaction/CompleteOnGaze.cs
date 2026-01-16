using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class CompleteOnGaze : MonoBehaviour
    {
        [Header("Gaze Completion Settings")]
        [SerializeField] private TutorialSubStep subStep;
        [SerializeField] private float gazeDuration = 2f;
        [SerializeField] private float maxGazeAngle = 15f;

        private float gazeTimer = 0f;
        private Camera playerCamera;

        private void Awake()
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
            {
                Debug.LogError($"Main Camera not found in the scene for {name}.");
            }
            if (subStep == null)
            {
                Debug.LogError($"{nameof(subStep)} reference is not set on {name}.");
            }
        }

        private void Update()
        {
            if (subStep.IsCompleted) return;

            if (IsPlayerLookingAt())
            {

                gazeTimer += Time.deltaTime;
                if (gazeTimer >= gazeDuration)
                    subStep.Complete();
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
    }
}