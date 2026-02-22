using System.Collections;
using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public class TestBeforeSceneChangeFunc : MonoBehaviour, IBeforeSceneChange
    {
        public IEnumerator OnBeforeSceneChange()
        {
            Debug.Log("Running some logic before scene change...");
            yield return new WaitForSeconds(5f);
            Debug.Log("Finished logic before scene change!");
        }
    }
}