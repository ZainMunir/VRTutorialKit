using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace ECDA.VRTutorialKit
{
    public class TransitionTrigger : MonoBehaviour
    {
        [Scene] public string targetSceneName;

        public void ExecuteTransition()
        {
            if (SceneTransitionController.Instance != null)
            {
                SceneTransitionController.Instance.GoToScene(targetSceneName);
            }
            else
            {
                Debug.LogError("No SceneTransitionManager found in the world!");
            }
        }
    }
}