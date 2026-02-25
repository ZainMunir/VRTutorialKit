using System.Collections;
using UnityEngine;

namespace ECDA.VRTutorialKit
{

    public class BouncingArrow : MonoBehaviour
    {
        public float size = 1;
        [SerializeField] private float spinSpeed = 180f;
        [SerializeField] private Vector3 rotationAxis = Vector3.up;
        [SerializeField] private AnimationCurve bounceCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        private Vector3 initialLocalPosition;
        private Vector3 bounceDirectionLocal;
        [SerializeField] private float bounceHeight = 0.1f;
        private Coroutine bounceCoroutine;
        private bool isBouncing;

        private bool bounceOnEnable = false;

        private void Awake()
        {
            initialLocalPosition = transform.localPosition;
            bounceDirectionLocal = initialLocalPosition.sqrMagnitude > 0f ? initialLocalPosition.normalized : Vector3.up;
        }

        void Start()
        {
            transform.localScale = Vector3.one * size;
        }

        void OnEnable()
        {
            if (bounceOnEnable)
            {
                StartBouncing();
            }
        }

        [ContextMenu("Start Bouncing")]
        public void StartBouncing()
        {
            isBouncing = true;

            if (bounceCoroutine == null)
            {
                bounceCoroutine = StartCoroutine(BounceAndSpin());
            }
        }

        [ContextMenu("Stop Bouncing")]
        public void StopBouncing()
        {
            if (bounceCoroutine != null)
            {
                isBouncing = false;
            }
        }

        IEnumerator BounceAndSpin()
        {
            Vector3 spinAxis = rotationAxis.sqrMagnitude > 0f ? rotationAxis.normalized : Vector3.up;

            while (isBouncing)
            {
                float elapsedTime = 0f;
                Vector3 targetLocalPosition = initialLocalPosition + bounceDirectionLocal * bounceHeight;

                while (elapsedTime < 1f)
                {
                    float t = bounceCurve.Evaluate(elapsedTime);
                    transform.localPosition = Vector3.Lerp(initialLocalPosition, targetLocalPosition, t);
                    transform.Rotate(spinAxis, spinSpeed * Time.deltaTime, Space.Self);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                transform.localPosition = targetLocalPosition;

                elapsedTime = 0f;

                while (elapsedTime < 1f)
                {
                    float t = bounceCurve.Evaluate(elapsedTime);
                    transform.localPosition = Vector3.Lerp(targetLocalPosition, initialLocalPosition, t);
                    transform.Rotate(spinAxis, spinSpeed * Time.deltaTime, Space.Self);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                transform.localPosition = initialLocalPosition;
            }
            bounceCoroutine = null;
        }
    }
}
