using System;
using System.Threading.Tasks;
using DG.Tweening;
using Draggable;
using UnityEngine;

namespace Cards
{
    public class Client : BoardDraggable, ICardHolder
    {
        [field:SerializeField] public float TotalTime { get; private set; }
        
        public float CurrentTime { get; private set; }

        [SerializeField] private SpriteRenderer _highlight;
        [SerializeField] private GameObject _fillGauge;
        [SerializeField] private ParticleSystem _angryParticle;
        [SerializeField] private ParticleSystem _loveParticle;
        
        private RaycastHit[] _raycastHits = new RaycastHit[16];

        private void Start()
        {
            CurrentTime = TotalTime;
        }

        public override void Initialize(Vector3 position)
        {
            base.Initialize(position);
            _highlight.gameObject.SetActive(false);
            StartTween();
        }

        private async void StartTween()
        {
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, 0.5f);

            await Task.Delay(200);
            
            transform.DOPunchRotation(new Vector3(0,15,0), 0.3f).SetEase(Ease.InOutBack);
            
            await Task.Delay(300);
            
            IsInitialized = true;
        }

        protected override void Update()
        {
            base.Update();
            
            CurrentTime -= Time.deltaTime;

            if (CurrentTime <= 0)
            {
                GameManager.Instance.KillClient(this);
                return;
            }

            float percentage = CurrentTime / TotalTime;
            _fillGauge.transform.localScale = new Vector3(percentage <= 0.01f ? 0f : percentage, 1, 1);
        }

        public override void GetHeld(bool isHeld)
        {
            if (isHeld == IsHeld || IsInitialized == false)
            {
                return;
            }
            IsHeld = isHeld;

            transform.DOComplete();

            SetShadow(isHeld ? ShadowHeldDistance : ShadowBaseDistance);
            SetSpritesSortingOrder(isHeld ? 100 : 0);

            if (isHeld == false)
            {
                //raycast toward camera to see if there is a playzone on mouse position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                int hits = Physics.RaycastNonAlloc(ray, _raycastHits);
                for (int i = 0; i < hits; i++)
                {
                    if (_raycastHits[i].collider.TryGetComponent(out PlayZoneController playZone))
                    {
                        LastPlayZonePosition = _raycastHits[i].point;
                        break;
                    }
                }
                transform.DOMove(LastPlayZonePosition, 0.2f);
            }
        }

        public override void CardUnderOnDrop(CardController cardUnder)
        {
            
        }

        public void GetPizza(CardController pizza)
        {
            GameManager.Instance.Board.RemoveCard(pizza);

            int money = pizza.Data.Value;
            GameManager.Instance.ClientServed(this, money);
        }

        public void HoldCard(CardController card)
        {
            if (card.Data.IsPizza == false)
            {
                return;
            }
            GetPizza(card);
        }

        public void DestroyClient(bool isAngry = false)
        {
            if (GameManager.Instance.BoardElementInput.CurrentHeldElement == this)
            {
                GameManager.Instance.BoardElementInput.CurrentHeldElement = null;
            }
            
            IsInitialized = false;

            if (isAngry)
            {
                _angryParticle.transform.parent = null;
                _angryParticle.Play();
            }
            else
            {
                _loveParticle.transform.parent = null;
                _loveParticle.Play();
            }
            
            transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InCirc).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }

        public void Highlight(bool doShow)
        {
            _highlight.gameObject.SetActive(doShow);
        }
    }
}