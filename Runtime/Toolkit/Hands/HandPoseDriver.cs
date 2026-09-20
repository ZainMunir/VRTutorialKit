using UnityEngine;

namespace ECDA.VRTutorialKit
{

    [RequireComponent(typeof(Animator))]
    public class HandPoseDriver : MonoBehaviour
    {
        [SerializeField] private bool isLeftHand;
        [SerializeField][Min(0.01f)] private float defaultCurlSpeed = 12f;

        private Animator animator;
        private int[] paramHashes;

        private readonly float[] current = new float[FingerConstants.Count];
        private readonly float[] baseCurls = new float[FingerConstants.Count];
        private readonly float[] poseCurls = new float[FingerConstants.Count];

        private HandPose gripPose;
        private HandPose actionPose;

        public bool IsLeftHand => isLeftHand;

        private void Awake()
        {
            animator = GetComponent<Animator>();

            paramHashes = new int[FingerConstants.Count];
            foreach (FingerType finger in System.Enum.GetValues(typeof(FingerType)))
            {
                paramHashes[(int)finger] = Animator.StringToHash(finger.ToString());
            }
        }

        public void SetBaseCurl(FingerType finger, float value)
        {
            baseCurls[(int)finger] = Mathf.Clamp01(value);
        }

        public void SetGripPose(HandPose pose)
        {
            gripPose = pose;
        }

        public void ClearGripPose()
        {
            gripPose = null;
            actionPose = null;
        }

        public void SetActionPose(HandPose pose)
        {
            actionPose = pose;
        }

        public void ClearActionPose()
        {
            actionPose = null;
        }

        private void Update()
        {
            HandPose active = actionPose != null ? actionPose : gripPose;

            float[] target;
            float speed;

            if (active != null)
            {
                active.CopyTo(poseCurls);
                target = poseCurls;
                speed = active.TransitionSpeed > 0f ? active.TransitionSpeed : defaultCurlSpeed;
            }
            else
            {
                target = baseCurls;
                speed = defaultCurlSpeed;
            }

            float step = speed * Time.deltaTime;

            for (int i = 0; i < FingerConstants.Count; i++)
            {
                current[i] = Mathf.MoveTowards(current[i], target[i], step);
                animator.SetFloat(paramHashes[i], current[i]);
            }
        }
    }
}
