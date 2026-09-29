using System;
using UnityEngine;

namespace ECDA.VRTutorialKit
{
    /// <summary>
    /// Draws a string (layer name) or int (layer index) field as a dropdown of the physics layers
    /// defined under Tags &amp; Layers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class UnityLayerAttribute : PropertyAttribute { }
}
