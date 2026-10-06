using UnityEngine;
using UnityEngine.Serialization;

namespace ECDA.VRTutorialKit
{
    public class OnTriggerEnterEventTag : OnTriggerEnterEventBase
    {
        [Tooltip("Unity tag (Tag Manager) the entering collider's GameObject must have. For TargetTag assets, use OnTriggerEnterEventTargetTag.")]
        [SerializeField, UnityTag, FormerlySerializedAs("targetTag")] private string unityTag = "Untagged";

        protected override GameObject Match(Collider other)
        {
            return other.CompareTag(unityTag) ? other.gameObject : null;
        }
    }
}
