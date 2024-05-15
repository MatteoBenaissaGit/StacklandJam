using UnityEngine;

namespace Data.Cards
{
    public enum CardType
    {
        Human = 0,
        Resource = 1,
        
    }
    
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Card", order = 1)]
    public class CardData : ScriptableObject
    {
    
    }
}
