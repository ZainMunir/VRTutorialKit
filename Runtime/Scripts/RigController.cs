using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using ECDA.VRTutorialKit;
using System;

public class RigController : MonoBehaviour
{
    [SerializeField] private ControllerInputActionManager leftControllerInputActionManager;
    [SerializeField] private ControllerInputActionManager rightControllerInputActionManager;

    [SerializeField] private XRRayInteractor leftTeleportRayInteractor;
    [SerializeField] private XRRayInteractor rightTeleportRayInteractor;


    [SerializeField] private SnapTurnProvider snapTurnProvider;
    [SerializeField] private DynamicMoveProvider dynamicMoveProvider;

    [SerializeField] private TutorialStep.LocomotionMode leftHandLocomotion;
    [SerializeField] private TutorialStep.LocomotionMode rightHandLocomotion;
    [SerializeField] private bool enableSnapTurn;

    private TutorialManager tutorialManager;
    void Start()
    {
        tutorialManager = TutorialManager.Instance;
        if (tutorialManager == null)
        {
            Debug.LogError("TutorialManager instance not found.");
            return;
        }
        tutorialManager.OnTutorialStepChanged += OnTutorialStepChanged;

        UpdateLocomotionControls();
    }
    void OnDestroy()
    {
        if (tutorialManager != null)
        {
            tutorialManager.OnTutorialStepChanged -= OnTutorialStepChanged;
        }
    }

    private void AllowMovement(bool enabled)
    {
        dynamicMoveProvider.enabled = enabled;
    }

    private void SetSmoothMotion(bool enable, ControllerInputActionManager manager)
    {
        manager.smoothMotionEnabled = enable;
    }

    private void SetSnapTurn(bool enabled)
    {
        snapTurnProvider.enabled = enabled;
    }

    private void SetTeleportation(bool enabled, ControllerInputActionManager manager, XRRayInteractor rayInteractor)
    {
        SetSmoothMotion(!enabled, manager);
        rayInteractor.enabled = enabled;
    }

    private void DisableAllControls()
    {
        SetSnapTurn(false);
        SetTeleportation(false, leftControllerInputActionManager, leftTeleportRayInteractor);
        SetTeleportation(false, rightControllerInputActionManager, rightTeleportRayInteractor);
        AllowMovement(false);
    }

    void OnTutorialStepChanged(bool stepCompleted)
    {
        UpdateLocomotionControls();
    }

    void UpdateLocomotionControls()
    {
        var step = tutorialManager.GetCurrentStep();
        if (step == null) return;

        DisableAllControls();

        SetSnapTurn(step.enableSnapTurn);

        HandleLocomotionMode(step.leftHandLocomotion, leftControllerInputActionManager, leftTeleportRayInteractor);
        HandleLocomotionMode(step.rightHandLocomotion, rightControllerInputActionManager, rightTeleportRayInteractor);
    }

    void HandleLocomotionMode(TutorialStep.LocomotionMode mode, ControllerInputActionManager manager, XRRayInteractor rayInteractor)
    {
        switch (mode)
        {
            case TutorialStep.LocomotionMode.None:
                SetSmoothMotion(false, manager);
                break;
            case TutorialStep.LocomotionMode.Smooth:
                SetTeleportation(false, manager, rayInteractor);
                AllowMovement(true);
                break;
            case TutorialStep.LocomotionMode.Teleport:
                SetTeleportation(true, manager, rayInteractor);
                break;
        }
    }
}