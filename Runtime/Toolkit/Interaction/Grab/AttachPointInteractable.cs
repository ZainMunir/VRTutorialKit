using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

namespace ECDA.VRTutorialKit
{
    /// <summary>
    /// Simple interactable with an assignable attach transform, so the interactor line connects
    /// to that point while selected instead of to the object's pivot.
    /// </summary>
    public class AttachPointInteractable : XRSimpleInteractable
    {
        public Transform attachTransform;

        /// <inheritdoc />
        public override Transform GetAttachTransform(IXRInteractor interactor)
        {
            return attachTransform != null ? attachTransform : base.GetAttachTransform(interactor);
        }
    }
}
