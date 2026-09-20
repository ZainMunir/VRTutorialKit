using System.Collections;
using UnityEngine;

namespace ECDA.VRTutorialKit
{
    public interface IBeforeSceneChange
    {
        IEnumerator OnBeforeSceneChange();
    }
}