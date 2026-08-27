using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ECDA.VRTutorialKit.EditorTools
{
    /// <summary>
    /// Drawer for <see cref="RequireInterfaceAttribute"/>.
    ///
    /// Keeps the default object field, but post-processes whatever the user drops on it:
    /// a GameObject (or a component that does not implement the interface) is resolved to a
    /// sibling component that does. When more than one component qualifies, a menu asks which,
    /// instead of silently taking the first one.
    /// </summary>
    [CustomPropertyDrawer(typeof(RequireInterfaceAttribute))]
    internal class RequireInterfaceDrawer : PropertyDrawer
    {
        const float HelpBoxHeight = 30f;

        Type RequiredType => ((RequireInterfaceAttribute)attribute).InterfaceType;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            if (NeedsWarning(property))
            {
                height += EditorGUIUtility.standardVerticalSpacing + HelpBoxHeight;
            }
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.ObjectReference)
            {
                EditorGUI.LabelField(position, label,
                    new GUIContent($"[RequireInterface] only works on object reference fields."));
                return;
            }

            var fieldRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            label = EditorGUI.BeginProperty(fieldRect, label, property);
            label.tooltip = string.IsNullOrEmpty(label.tooltip)
                ? $"Requires a component implementing {RequiredType.Name}."
                : label.tooltip;

            EditorGUI.BeginChangeCheck();
            var picked = EditorGUI.ObjectField(fieldRect, label, property.objectReferenceValue, ObjectFieldType, true);
            if (EditorGUI.EndChangeCheck())
            {
                Assign(property, picked);
            }

            EditorGUI.EndProperty();

            if (NeedsWarning(property))
            {
                var helpRect = new Rect(
                    position.x,
                    fieldRect.yMax + EditorGUIUtility.standardVerticalSpacing,
                    position.width,
                    HelpBoxHeight);
                EditorGUI.HelpBox(helpRect,
                    $"{property.objectReferenceValue.GetType().Name} does not implement {RequiredType.Name}.",
                    MessageType.Warning);
            }
        }

        bool NeedsWarning(SerializedProperty property)
        {
            return property.propertyType == SerializedPropertyType.ObjectReference
                   && property.objectReferenceValue != null
                   && !RequiredType.IsInstanceOfType(property.objectReferenceValue);
        }

        /// <summary>
        /// Type the object field itself accepts, taken from the declared field so array and list
        /// elements resolve to their element type rather than the collection type.
        /// </summary>
        Type ObjectFieldType
        {
            get
            {
                var type = fieldInfo != null ? fieldInfo.FieldType : typeof(UnityEngine.Object);
                if (type.IsArray)
                {
                    type = type.GetElementType();
                }
                else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
                {
                    type = type.GetGenericArguments()[0];
                }
                return type;
            }
        }

        void Assign(SerializedProperty property, UnityEngine.Object picked)
        {
            if (picked == null)
            {
                property.objectReferenceValue = null;
                return;
            }

            if (RequiredType.IsInstanceOfType(picked))
            {
                property.objectReferenceValue = picked;
                return;
            }

            // Unity resolved a GameObject drop to the first component of the field type, or the
            // user picked an unrelated component - search the whole GameObject instead.
            var source = picked as GameObject ?? (picked as Component)?.gameObject;
            if (source == null)
            {
                Debug.LogWarning($"{picked.name} does not implement {RequiredType.Name}.", picked);
                return;
            }

            var candidates = new List<Component>();
            foreach (var component in source.GetComponents<Component>())
            {
                if (component != null
                    && RequiredType.IsInstanceOfType(component)
                    && ObjectFieldType.IsInstanceOfType(component))
                {
                    candidates.Add(component);
                }
            }

            switch (candidates.Count)
            {
                case 0:
                    Debug.LogWarning($"{source.name} has no component implementing {RequiredType.Name}.", source);
                    break;
                case 1:
                    property.objectReferenceValue = candidates[0];
                    break;
                default:
                    ShowCandidateMenu(property, candidates);
                    break;
            }
        }

        void ShowCandidateMenu(SerializedProperty property, List<Component> candidates)
        {
            var serializedObject = property.serializedObject;
            string path = property.propertyPath;
            var current = property.objectReferenceValue;

            var menu = new GenericMenu();
            menu.AddDisabledItem(new GUIContent($"Component implementing {RequiredType.Name}"));
            menu.AddSeparator(string.Empty);

            foreach (var candidate in candidates)
            {
                var captured = candidate;
                menu.AddItem(new GUIContent(captured.GetType().Name), captured == current, () =>
                {
                    serializedObject.Update();
                    var target = serializedObject.FindProperty(path);
                    if (target == null) return;

                    target.objectReferenceValue = captured;
                    serializedObject.ApplyModifiedProperties();
                });
            }

            menu.ShowAsContext();
        }
    }
}
