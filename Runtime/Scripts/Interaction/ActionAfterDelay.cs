using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace ECDA.VRTutorialKit
{
    public class ActionAfterDelay : MonoBehaviour
    {
        public float delay = 5f;
        public bool startOnEnable = true;
        public UnityEvent actions;
        private Coroutine actionCoroutine;

        void OnEnable()
        {
            if (startOnEnable)
            {
                StartAction();
            }
        }

        IEnumerator DelayedAction()
        {
            yield return new WaitForSeconds(delay);
            actions?.Invoke();
            actionCoroutine = null;
        }

        void OnDestroy()
        {
            if (actionCoroutine != null)
            {
                StopCoroutine(actionCoroutine);
            }
        }

        public void CancelAction()
        {
            if (actionCoroutine != null)
            {
                StopCoroutine(actionCoroutine);
                actionCoroutine = null;
            }
        }

        public void RestartTimer()
        {
            CancelAction();
            actionCoroutine = StartCoroutine(DelayedAction());
        }

        public void StartAction()
        {
            if (actionCoroutine == null)
            {
                Debug.Log($"[ActionAfterDelay] Starting delayed action with {delay} seconds delay.");
                actionCoroutine = StartCoroutine(DelayedAction());
            }
        }
    }
}
