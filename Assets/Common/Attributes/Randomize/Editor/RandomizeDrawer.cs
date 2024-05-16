using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MatteoBenaissaLibrary.Attributes.Randomize.Editor
{
    [CustomPropertyDrawer(typeof(RandomizeAttribute))]
    public class RandomizeDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return 16f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            if (property.propertyType != SerializedPropertyType.Float)
            {
                GUI.contentColor = new Color(1f, 0.47f, 0.51f);
                EditorGUI.LabelField(position,$"'{property.name}' property can't be randomized ! ");
                GUI.contentColor = Color.white;
                return;
            }

            Rect labelPosition = new Rect(position.x, position.y, position.width/2, 16f);
            Rect buttonPosition = new Rect(position.x + labelPosition.width + 20, position.y, position.width/4, 16f);
            
            EditorGUI.LabelField(labelPosition,label,new GUIContent(property.floatValue.ToString()));
            if (GUI.Button(buttonPosition, "Randomize"))
            {
                RandomizeAttribute randomizeAttribute = attribute as RandomizeAttribute;
                float randomNumber =  Random.Range(randomizeAttribute.MinimumValue, randomizeAttribute.MaximumValue);
                property.floatValue = (float)Math.Round(randomNumber, 3);
            }
            EditorGUI.EndProperty();
        }
    }
}
