using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace ECDA.VRTutorialKit
{
    public class HandedAttachPoint : XRGrabInteractable
    {
        public Transform leftAttach;
        public Transform rightAttach;

        protected override void OnSelectEntering(SelectEnterEventArgs args)
        {
            if (args.interactorObject.transform.CompareTag("LeftHand"))
            {
                attachTransform = leftAttach;
            }
            else if (args.interactorObject.transform.CompareTag("RightHand"))
            {
                attachTransform = rightAttach;
            }
            Debug.Log($"Interactor Tag: {args.interactorObject.transform.tag}");
            base.OnSelectEntering(args);
        }
    }
}