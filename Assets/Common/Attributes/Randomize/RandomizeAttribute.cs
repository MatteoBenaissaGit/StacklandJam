using System;
using UnityEngine;

namespace MatteoBenaissaLibrary.Attributes.Randomize
{
    [AttributeUsage(AttributeTargets.Field)]
    public class RandomizeAttribute : PropertyAttribute
    {
        public float MinimumValue { get; private set; }
        public float MaximumValue { get; private set; }

        public RandomizeAttribute()
        {
            MinimumValue = 0f;
            MaximumValue = 100f;
        }
        
        public RandomizeAttribute(int minimumValue, int maximumValue)
        {
            MinimumValue = minimumValue;
            MaximumValue = maximumValue;
        }
        
        public RandomizeAttribute(float minimumValue, float maximumValue)
        {
            MinimumValue = minimumValue;
            MaximumValue = maximumValue;
        }
    }
}
