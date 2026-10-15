using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace ECDA.VRTutorialKit
{
    public class ActionOnButtonPress : MonoBehaviour
    {
        [SerializeField] private List<InputActionReference> buttonsToWatch = new List<InputActionReference>();
        public UnityEvent onButtonPressed;

        public static event Action<ActionOnButtonPress, string> AnyButtonPressed;

        void OnEnable()
        {
            foreach (var button in buttonsToWatch)
            {
                button.action.performed += OnButtonPressed;
                button.action.Enable();
            }
        }

        void OnDisable()
        {
            foreach (var button in buttonsToWatch)
            {
                button.action.performed -= OnButtonPressed;
            }
        }

        void OnButtonPressed(InputAction.CallbackContext context)
        {
            onButtonPressed.Invoke();
            AnyButtonPressed?.Invoke(this, context.action.name);
        }

        void OnDestroy()
        {
            foreach (var button in buttonsToWatch)
            {
                button.action.performed -= OnButtonPressed;
            }
        }

    }
}