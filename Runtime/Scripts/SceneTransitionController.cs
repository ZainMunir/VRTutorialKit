using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ECDA.VRTutorialKit
{
    public class SceneTransitionController : MonoBehaviour
    {
        public static SceneTransitionController Instance { get; private set; }
        public FadeScreen fadeScreen;
        [SerializeField] private bool _isTransitioning = false;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (fadeScreen == null)
            {
                fadeScreen = FindAnyObjectByType<FadeScreen>();
            }
        }

        public void GoToScene(string sceneName)
        {
            if (_isTransitioning) return;
            StartCoroutine(GoToSceneRoutine(sceneName));
        }

        IEnumerator GoToSceneRoutine(string sceneName)
        {
            _isTransitioning = true;

            try
            {
                AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
                operation.allowSceneActivation = false;

                if (fadeScreen != null) yield return fadeScreen.FadeOut();


                var handlers = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                                    .OfType<IBeforeSceneChange>();

                foreach (var handler in handlers)
                {
                    if (handler is MonoBehaviour mb && !mb.isActiveAndEnabled) continue;
                    yield return handler.OnBeforeSceneChange();
                }

                while (operation.progress < 0.9f) yield return null;
                operation.allowSceneActivation = true;
                while (!operation.isDone) yield return null;
                yield return new WaitForEndOfFrame();

                fadeScreen = FindAnyObjectByType<FadeScreen>(FindObjectsInactive.Include);
                AlignPlayer();

                if (fadeScreen != null) yield return fadeScreen.FadeIn();
            }
            finally
            {
                _isTransitioning = false;
            }
        }

        private void AlignPlayer()
        {
            // 1. Find the new scene's components
            GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
            // Most XR Origins are tagged "Player" or have the script XR Origin
            GameObject xrOrigin = GameObject.FindWithTag("XRRig");

            if (spawnPoint == null || xrOrigin == null)
            {
                Debug.LogWarning("Alignment failed: SpawnPoint or XR Origin missing in new scene.");
                return;
            }

            Transform cameraTransform = Camera.main.transform;

            // 2. Position Alignment
            // Calculate the offset between the rig and the actual headset position
            Vector3 distanceOffset = xrOrigin.transform.position - cameraTransform.position;
            // Flatten y so we don't accidentally bury the player in the floor
            distanceOffset.y = 0;
            xrOrigin.transform.position = spawnPoint.transform.position + distanceOffset;

            // 3. Rotation Alignment
            float cameraYaw = cameraTransform.eulerAngles.y;
            float rigYaw = xrOrigin.transform.eulerAngles.y;
            float spawnYaw = spawnPoint.transform.eulerAngles.y;

            // Formula: Adjust rig rotation so camera matches spawn rotation
            float targetYaw = spawnYaw - (cameraYaw - rigYaw);
            xrOrigin.transform.rotation = Quaternion.Euler(0, targetYaw, 0);
        }
    }
}