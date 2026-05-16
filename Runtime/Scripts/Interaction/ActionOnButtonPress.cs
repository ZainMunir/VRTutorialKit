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



        void OnEnable()
        {
            foreach (var button in buttonsToWatch)
            {
                button.action.performed += OnButtonPressed;
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