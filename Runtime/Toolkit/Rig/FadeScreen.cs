using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace ECDA.VRTutorialKit
{
    public class FadeScreen : MonoBehaviour
    {
        public bool fadeOnStart = true;
        public float fadeDuration = 2;
        public Color fadeColor;
        private Image img;
        private Coroutine current;

        void Awake()
        {
            img = GetComponent<Image>();
            img.raycastTarget = false;
        }

        void Start()
        {
            if (fadeOnStart)
                Fade(1, 0);
        }

        public Coroutine FadeIn() => FadeTo(0, fadeDuration);
        public Coroutine FadeOut() => FadeTo(1, fadeDuration);
        public Coroutine FadeIn(float duration, float delay = 0) => FadeTo(0, duration, delay);
        public Coroutine FadeOut(float duration, float delay = 0) => FadeTo(1, duration, delay);
        public Coroutine Fade(float alphaIn, float alphaOut)
        {
            SetAlpha(alphaIn);
            return FadeTo(alphaOut, fadeDuration);
        }
        public Coroutine FadeTo(float target, float duration, float delay = 0)
        {
            if (current != null) StopCoroutine(current);
            current = StartCoroutine(FadeRoutine(target, duration, delay));
            return current;
        }

        IEnumerator FadeRoutine(float target, float duration, float delay)
        {
            if (delay > 0) yield return new WaitForSeconds(delay);

            float start = img.color.a;
            float timer = 0;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                SetAlpha(Mathf.Lerp(start, target, timer / duration));
                yield return null;
            }
            SetAlpha(target);
            current = null;
        }

        private void SetAlpha(float alpha)
        {
            Color newColor = fadeColor;
            newColor.a = alpha;
            img.color = newColor;
            img.enabled = alpha > 0;
        }
    }
}
