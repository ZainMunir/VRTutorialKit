using System;
using UnityEngine;

namespace ECDA.VRTutorialKit
{
    /// <summary>
    /// Constrains an object reference field to components implementing a given interface.
    ///
    /// Unity cannot serialize interface references directly, so fields are typically typed as
    /// <see cref="MonoBehaviour"/>. Dragging a GameObject onto such a field makes Unity pick the
    /// first MonoBehaviour on it, which is rarely the one that implements the interface.
    /// The editor drawer for this attribute resolves the drop to a component that actually
    /// implements <see cref="InterfaceType"/>, and prompts for a choice when several qualify.
    /// </summary>
    /// <example>
    /// [SerializeField, RequireInterface(typeof(ProgressProvider))]
    /// private MonoBehaviour progressProvider;
    /// </example>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class RequireInterfaceAttribute : PropertyAttribute
    {
        public Type InterfaceType { get; }

        public RequireInterfaceAttribute(Type interfaceType)
        {
            InterfaceType = interfaceType;
        }
    }
}
