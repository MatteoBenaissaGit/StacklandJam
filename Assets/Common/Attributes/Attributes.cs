using MatteoBenaissaLibrary.Attributes.HideIf;
using MatteoBenaissaLibrary.Attributes.Randomize;
using MatteoBenaissaLibrary.Attributes.ReadOnly;
using MatteoBenaissaLibrary.Attributes.ShowIf;
using UnityEngine;

namespace MatteoBenaissaLibrary.Attributes
{
    public class Attributes : MonoBehaviour
    {
        [Header("ReadOnly") ,SerializeField, ReadOnly] private string _readOnlyString = "THIS IS READONLY";

        [Header("Randomize") ,SerializeField, Randomize] private float _randomize;
        [SerializeField, Randomize(0,5)] private float _randomizeFromIntValues;
        [SerializeField, Randomize(-4.5f,10.5f)] private float _randomizeFromFloatValues;

        private enum ShowIfChoice
        {
            ShowFirstProperty = 0,
            ShowSecondProperty = 1
        }
        [Header("ShowIf"), SerializeField] private ShowIfChoice _showPropertyChoice;
        [SerializeField, ShowIf("_showPropertyChoice", ShowIfChoice.ShowFirstProperty), ReadOnly] 
        private string _firstPropertyShow = "THIS IS THE FIRST PROPERTY";
        [SerializeField, ShowIf("_showPropertyChoice", ShowIfChoice.ShowSecondProperty), ReadOnly] 
        private string _secondPropertyShow = "THIS IS THE SECOND PROPERTY";
        
        private enum HideIfChoice
        {
            HideFirstProperty = 0,
            HideSecondProperty = 1,
            HideNone = 2
        }
        [Header("HideIf"), SerializeField] private HideIfChoice _hidePropertyChoice;
        [SerializeField, HideIf("_hidePropertyChoice", HideIfChoice.HideFirstProperty), ReadOnly] 
        private string _firstPropertyHide = "THIS IS THE FIRST PROPERTY";
        [SerializeField, HideIf("_hidePropertyChoice", HideIfChoice.HideSecondProperty), ReadOnly] 
        private string _secondPropertyHide = "THIS IS THE SECOND PROPERTY";
    }
}
