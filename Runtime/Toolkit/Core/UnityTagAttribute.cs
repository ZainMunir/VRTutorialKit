using System;
using UnityEngine;

namespace ECDA.VRTutorialKit
{
    /// <summary>
    /// Draws a string field as a dropdown of the tags defined in the Tag Manager.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class UnityTagAttribute : PropertyAttribute { }
}
