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
    [SerializeField] private BoosterBuy _boosterBuy;

    public BoosterBuy BoosterBuyer => _boosterBuy;
    
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
        card.IsInitialized = false;
        card.transform.DOKill();
        card.transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.OutCirc).OnComplete(() => Destroy(card.gameObject));
    }

    public void HighlightsPossiblesChildrenOf(CardController cardToPlace)
    {
        foreach (CardController card in Cards)
        {
            if (card.CanHaveChild(cardToPlace) == false)
            {
                continue;
            }
            card.Highlight(true);
        }

        if (cardToPlace.Data.Type == CardType.Money)
        {
            _boosterBuy.Highlight(true);
        }

        if (cardToPlace.Data.IsPizza)
        {
            GameManager.Instance.CurrentClients.ForEach(x => x.Highlight(true));
        }
    }
    
    public void UnhighlightAll()
    {
        _boosterBuy.Highlight(false);
        Cards.ForEach(x => x.Highlight(false));
        GameManager.Instance.CurrentClients.ForEach(x => x.Highlight(false));
    }
}