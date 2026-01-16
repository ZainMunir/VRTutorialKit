using UnityEngine;

public class VRPeripheralManager : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Transform currentTarget;

    [Header("Arrows")]
    [SerializeField] private GuidingArrow leftArrow;
    [SerializeField] private GuidingArrow rightArrow;

    [Header("Gaze Settings")]
    [Tooltip("Target is 'visible' within this cone angle in degrees")]
    [SerializeField] private float sensitivityAngle = 25f;

    void Start()
    {
        if (leftArrow == null || rightArrow == null)
        {
            Debug.LogError("GuidingArrow components not assigned in VRPeripheralManager");
            enabled = false;
            return;
        }
    }

    void Update()
    {
        if (currentTarget == null)
        {
            DeactivateBoth();
            return;
        }

        Vector3 targetDir = currentTarget.position - transform.position;

        float angle = Vector3.Angle(transform.forward, targetDir);
        float signedAngle = Vector3.SignedAngle(transform.forward, targetDir, Vector3.up);

        if (angle > sensitivityAngle)
        {
            if (signedAngle > 0) // Target is to the Right
            {
                rightArrow.Activate(targetDir);
                leftArrow.Deactivate();
            }
            else // Target is to the Left
            {
                leftArrow.Activate(targetDir);
                rightArrow.Deactivate();
            }
        }
        else
        {
            DeactivateBoth();
        }
    }

    void DeactivateBoth()
    {
        leftArrow.Deactivate();
        rightArrow.Deactivate();
    }

    public void SetNewTarget(Transform newTarget)
    {
        currentTarget = newTarget;
    }
}