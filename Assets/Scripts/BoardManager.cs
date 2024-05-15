using System.Collections.Generic;
using Cards;
using Data.Cards;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public List<CardController> Cards { get; private set; } = new List<CardController>();
    
    [SerializeField] private CardController _cardPrefab;
    [Space]
    [SerializeField] private PlayZoneController _playZone;

    public void CreateCard(CardData data, Vector3 position)
    {
        CardController card = Instantiate(_cardPrefab, transform);
        position.y = _playZone.transform.position.y + 0.1f;
        card.Initialize(data, position);
        Cards.Add(card);
        card.gameObject.name = position.ToString();
    }
    
    public void RemoveCard(CardController card)
    {
        Cards.Remove(card);
        card.transform.SetParent(null);
        Destroy(card.gameObject);
    }
}