using UnityEditor;
using UnityEngine;

namespace ECDA.VRTutorialKit.EditorTools
{
    [CustomPropertyDrawer(typeof(UnityLayerAttribute))]
    internal class UnityLayerDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool isString = property.propertyType == SerializedPropertyType.String;
            if (!isString && property.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.LabelField(position, label, new GUIContent("[UnityLayer] only works on string or int fields."));
                return;
            }

            int current = isString ? LayerMask.NameToLayer(property.stringValue) : property.intValue;

            label = EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            int layer = EditorGUI.LayerField(position, label, current);
            if (EditorGUI.EndChangeCheck())
            {
                if (isString)
                    property.stringValue = LayerMask.LayerToName(layer);
                else
                    property.intValue = layer;
            }
            EditorGUI.EndProperty();
        }
    }
}
