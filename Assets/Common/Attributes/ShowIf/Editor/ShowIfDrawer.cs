using UnityEditor;
using UnityEngine;

namespace MatteoBenaissaLibrary.Attributes.ShowIf.Editor
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        private bool _showed;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute hideIfAttribute = attribute as ShowIfAttribute;
            _showed = true;

            string attributeFieldName = hideIfAttribute.FieldName;
            if (string.IsNullOrEmpty(attributeFieldName) == false)
            {
                SerializedProperty serializedProperty = property.serializedObject.FindProperty(hideIfAttribute.FieldName);
                
                _showed = serializedProperty.propertyType switch
                {
                    SerializedPropertyType.Boolean => serializedProperty.boolValue,
                    SerializedPropertyType.Enum => serializedProperty.enumValueIndex == hideIfAttribute.FieldValue,
                    SerializedPropertyType.ObjectReference => serializedProperty.objectReferenceValue != null,
                    _ => true,
                };
            }

            if (_showed)
            {
                return base.GetPropertyHeight(property, label);
            }

            return -EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_showed)
            {
                EditorGUI.BeginProperty(position, label, property);
                EditorGUI.PropertyField(position, property, label);
                EditorGUI.EndProperty();
            }
        }
    }
}