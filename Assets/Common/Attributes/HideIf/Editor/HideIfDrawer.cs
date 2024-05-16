using UnityEditor;
using UnityEngine;

namespace MatteoBenaissaLibrary.Attributes.HideIf.Editor
{
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    internal class HideIfDrawer : PropertyDrawer
    {
        private bool _hidden;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            HideIfAttribute hideIfAttribute = attribute as HideIfAttribute;
            _hidden = true;

            string attributeFieldName = hideIfAttribute.FieldName;
            if (string.IsNullOrEmpty(attributeFieldName) == false)
            {
                SerializedProperty serializedProperty = property.serializedObject.FindProperty(hideIfAttribute.FieldName);
                
                _hidden = serializedProperty.propertyType switch
                {
                    SerializedPropertyType.Boolean => serializedProperty.boolValue,
                    SerializedPropertyType.Enum => serializedProperty.enumValueIndex == hideIfAttribute.FieldValue,
                    SerializedPropertyType.ObjectReference => serializedProperty.objectReferenceValue != null,
                    _ => true,
                };
            }

            if (_hidden == false)
            {
                return base.GetPropertyHeight(property, label);
            }

            return -EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_hidden == false)
            {
                EditorGUI.BeginProperty(position, label, property);
                EditorGUI.PropertyField(position, property, label);
                EditorGUI.EndProperty();
            }
        }
    }
}