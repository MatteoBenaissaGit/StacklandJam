using System.Collections.Generic;
using Cards;
using Data.Cards;
using DG.Tweening;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public List<CardController> Cards { get; private set; } = new List<CardController>();
    
    [SerializeField] private CardController _cardPrefab;
    [Space]
    [SerializeField] private PlayZoneController _playZone;

    public void CreateCard(CardData data, Vector3 startPosition, Vector3 endPosition)
    {
        CardController card = Instantiate(_cardPrefab, transform);
        startPosition.y = _playZone.transform.position.y + 0.1f;
        card.Initialize(data, startPosition);
        Cards.Add(card);
        card.gameObject.name = startPosition.ToString();
        
        card.transform.DOMove(endPosition, 0.5f).SetEase(Ease.OutQuint);
    }
    
    public void RemoveCard(CardController card)
    {
        Cards.Remove(card);
        card.transform.SetParent(null);
        Destroy(card.gameObject);
    }
}