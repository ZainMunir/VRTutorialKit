using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class HideOnButtonPress : MonoBehaviour
{
    [SerializeField] private List<GameObject> objectsToHide = new List<GameObject>();

    [SerializeField] private List<InputActionReference> buttonsToWatch = new List<InputActionReference>();

    [SerializeField] private bool isHidden = false;

    private Dictionary<GameObject, Vector3> originalPositions;

    void Start()
    {
        foreach (var button in buttonsToWatch)
        {
            button.action.performed += OnButtonPressed;
        }
        originalPositions = new Dictionary<GameObject, Vector3>();
        foreach (var obj in objectsToHide)
        {
            if (obj != null)
            {
                originalPositions[obj] = obj.transform.position;
            }
        }
        UpdateObjectVisibility();
    }

    void UpdateObjectVisibility()
    {
        foreach (var obj in objectsToHide)
        {
            if (obj != null)
            {
                if (isHidden)
                {
                    obj.transform.position = originalPositions[obj] + Vector3.down * 10;
                }
                else
                {
                    obj.transform.position = originalPositions[obj];
                }
            }
        }
    }

    void OnButtonPressed(InputAction.CallbackContext context)
    {
        isHidden = !isHidden;
        UpdateObjectVisibility();
    }

    private void OnDestroy()
    {
        foreach (var button in buttonsToWatch)
        {
            button.action.performed -= OnButtonPressed;
        }
    }
}
