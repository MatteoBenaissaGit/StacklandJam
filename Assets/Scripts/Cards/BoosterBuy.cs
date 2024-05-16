using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Cards;
using DG.Tweening;
using Packs;
using TMPro;
using UnityEngine;

namespace Cards
{
    public class BoosterBuy : MonoBehaviour, ICardHolder
    {
        [SerializeField] private int _boosterPrice;
        [SerializeField] private TextMeshPro _priceText;
        [SerializeField] private Transform _boosterSpawnPoint;
        [SerializeField] private SpriteRenderer _highlight;
        [SerializeField] private PackController _boosterPackPrefab;

        public int CurrentNeededPrice { get; private set; }

        private void Awake()
        {
            Color color = _highlight.color;
            color.a = 0;
            _highlight.color = color;
            
            CurrentNeededPrice = _boosterPrice;
            _priceText.text = CurrentNeededPrice.ToString();
        }

        public void HoldCard(CardController card)
        {
            if (card.Data.Type != CardType.Money)
            {
                return;
            }

            List<CardController> cards = new List<CardController>();
            CardController currentChild = card;
            while (currentChild != null)
            {
                cards.Add(currentChild);
                currentChild = currentChild.Child;
            }
            
            transform.DOComplete();
            transform.DOPunchScale(Vector3.one * 0.2f, 0.3f).SetEase(Ease.OutElastic);
            transform.DOPunchRotation(new Vector3(0,10,0), 0.3f);

            CurrentNeededPrice -= cards.Count;
            _priceText.text = CurrentNeededPrice.ToString();
            cards.ForEach(x => GameManager.Instance.Board.RemoveCard(x));
            
            if (CurrentNeededPrice <= 0)
            {
                _priceText.text = "0";
                BuyBooster();
            }
        }

        private async void BuyBooster()
        {
            CurrentNeededPrice = _boosterPrice + CurrentNeededPrice;
            
            _priceText.text = CurrentNeededPrice.ToString();

            PackController pack = Instantiate(_boosterPackPrefab);
            pack.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            pack.Initialize(_boosterSpawnPoint.position);
            pack.IsInitialized = false;

            pack.transform.rotation = _boosterSpawnPoint.rotation;
            pack.transform.position = transform.position;
            pack.transform.DOMove(_boosterSpawnPoint.position, 0.3f).SetEase(Ease.OutExpo);
            
            Vector3 scale = pack.transform.localScale;
            pack.transform.localScale = Vector3.zero;
            pack.transform.DOScale(scale, 0.3f).SetEase(Ease.OutBack);

            await Task.Delay(300);
            
            if (CurrentNeededPrice <= 0)
            {
                BuyBooster();
            }
            
            pack.IsInitialized = true;
        }

        public void Highlight(bool doShow)
        {
            _highlight.DOFade(doShow ? 1 : 0, 0.2f);
        }
    }
}