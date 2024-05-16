using System.Collections.Generic;
using Data.Cards;
using UnityEngine;

namespace Data.Packs
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Pack", order = 2)]
    public class PackData : ScriptableObject
    {
        [field:SerializeField] public int NumberOfCardsUsable { get; private set; }
        [field:SerializeField] public int NumberOfCardsElements{ get; private set; }
        [field:SerializeField] public int NumberOfCardsHuman { get; private set; }
        [field:SerializeField] public int NumberOfCardsMoney { get; private set; }
        [field: SerializeField] public List<CardData> UsableCards { get; private set; } = new List<CardData>();
        [field: SerializeField] public List<CardData> ElementsCards { get; private set; } = new List<CardData>();
        [field: SerializeField] public List<CardData> HumanCards { get; private set; } = new List<CardData>();
        [field: SerializeField] public List<CardData> MoneyCards { get; private set; } = new List<CardData>();
        [field: SerializeField] public List<CardData> CustomAddedCards { get; private set; } = new List<CardData>();
        
        public int NumberOfCardsTotal => NumberOfCardsUsable + NumberOfCardsElements + NumberOfCardsHuman + NumberOfCardsMoney + CustomAddedCards.Count;
        
        public List<CardData> GetRandomCards()
        {
            List<CardData> randomCards = new List<CardData>();
            CustomAddedCards.ForEach(x => randomCards.Add(x));
            
            for (var i = 0; i < NumberOfCardsUsable; i++)
            {
                int randomIndex = Random.Range(0, UsableCards.Count);
                randomCards.Add(UsableCards[randomIndex]);
            }
            for (var i = 0; i < NumberOfCardsElements; i++)
            {
                int randomIndex = Random.Range(0, ElementsCards.Count);
                randomCards.Add(ElementsCards[randomIndex]);
            }
            for (var i = 0; i < NumberOfCardsHuman; i++)
            {
                int randomIndex = Random.Range(0, HumanCards.Count);
                randomCards.Add(HumanCards[randomIndex]);
            }
            for (var i = 0; i < NumberOfCardsMoney; i++)
            {
                int randomIndex = Random.Range(0, MoneyCards.Count);
                randomCards.Add(MoneyCards[randomIndex]);
            }

            return randomCards;
        }
    }
}