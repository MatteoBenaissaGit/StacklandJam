using System;
using UnityEngine;

namespace MatteoBenaissaLibrary.Attributes.HideIf 
{
    
    [AttributeUsage(AttributeTargets.Field)]
    public class HideIfAttribute : PropertyAttribute 
    {
        public readonly string FieldName;
        public readonly int FieldValue;

        public HideIfAttribute() {}
        
        public HideIfAttribute(string fieldName)
        {
            FieldName = fieldName;
        }
        
        public HideIfAttribute(string fieldName, object fieldValue) 
        {
            FieldName = fieldName;
            FieldValue = (int)fieldValue;
        }
    }
}