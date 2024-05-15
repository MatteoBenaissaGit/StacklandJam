using System.Collections.Generic;
using Data.Cards;
using UnityEngine;

namespace Data.Packs
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Pack", order = 2)]
    public class PackData : ScriptableObject
    {
        [field:SerializeField] public int NumberOfCards { get; private set; }
        [field: SerializeField] public List<CardData> PossibleCards { get; private set; } = new List<CardData>();
        
        public List<CardData> GetRandomCards()
        {
            List<CardData> randomCards = new List<CardData>();
            for (var i = 0; i < NumberOfCards; i++)
            {
                int randomIndex = Random.Range(0, PossibleCards.Count);
                randomCards.Add(PossibleCards[randomIndex]);
            }

            return randomCards;
        }
    }
}