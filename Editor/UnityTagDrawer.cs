using UnityEditor;
using UnityEngine;

namespace ECDA.VRTutorialKit.EditorTools
{
    [CustomPropertyDrawer(typeof(UnityTagAttribute))]
    internal class UnityTagDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label, new GUIContent("[UnityTag] only works on string fields."));
                return;
            }

            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            string tag = EditorGUI.TagField(position, label, property.stringValue);
            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = tag;
            }
            EditorGUI.EndProperty();
        }
    }
}
