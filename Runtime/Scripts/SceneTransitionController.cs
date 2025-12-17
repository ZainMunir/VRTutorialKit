using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ECDA.VRTutorialKit
{

    public class SceneTransitionController : MonoBehaviour
    {
        public FadeScreen fadeScreen;

        public void GoToScene(string sceneName)
        {
            StartCoroutine(GoToSceneRoutine(sceneName));
        }

        IEnumerator GoToSceneRoutine(string sceneName)
        {
            fadeScreen.FadeOut();

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;
            yield return new WaitForSeconds(fadeScreen.fadeDuration);
            while (operation.progress < 0.9f)
            {
                yield return null;
            }
            operation.allowSceneActivation = true;
        }
    }
}
