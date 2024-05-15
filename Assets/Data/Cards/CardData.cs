using System.Collections.Generic;
using UnityEngine;

namespace Data.Cards
{
    public enum CardType
    {
        Human = 0,
        Resource = 1,
        Usable = 2,
        Money = 3
    }
    
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Card", order = 1)]
    public class CardData : ScriptableObject
    {
        [field:SerializeField] public string Name { get; private set; }
        [field:SerializeField] public CardType Type { get; private set; }
        [field:SerializeField] public Sprite Sprite { get; private set; }
        [field:SerializeField] public string Description { get; private set; }
        [field:SerializeField] public int Value { get; private set; }
        [field:SerializeField] public bool IsNotSellable { get; private set; }
        [field:SerializeField] public bool IsStackable { get; private set; }
    }
}
