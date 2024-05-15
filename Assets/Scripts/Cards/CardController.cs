using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Cards;
using DG.Tweening;
using Draggable;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Cards
{
    public class CardController : BoardDraggable
    {
        public CardData Data { get; private set; }
        public CardController Parent { get; private set; }
        public CardController Child { get; private set; }

        [SerializeField] private SpriteRenderer _iconSpriteRenderer;
        [SerializeField] private SpriteRenderer _cardSpriteRenderer;
        [SerializeField] private TextMeshPro _cardNameText;
        [SerializeField] private Transform _cardFront;
        [SerializeField] private Transform _cardBack;

        [Space(10)]
        [SerializeField] private float _parentingDistance = 1f;

        public void Initialize(CardData data, Vector3 position)
        {
            Data = data;
            
            _iconSpriteRenderer.sprite = Data.Sprite;
            _cardSpriteRenderer.color = Data.Type switch
            {
                CardType.Human => new Color(1f, 0.93f, 0.55f),
                CardType.Resource => new Color(0.42f, 0.42f, 0.42f),
                CardType.Usable => new Color(1f, 0.44f, 0.43f),
                _ => throw new ArgumentOutOfRangeException()
            };
            _cardNameText.text = Data.Name;
            
            base.Initialize(position);

            SpawnTween();
        }

        private async void SpawnTween()
        {
            SetSpritesSortingOrder(100);
            
            _cardFront.gameObject.SetActive(false);
            
            _cardBack.gameObject.SetActive(true);
            _cardBack.transform.localScale = new Vector3(1, 1, 1);
            _cardBack.transform.DOKill();
            _cardBack.transform.DOScaleY(0, 0.1f).SetEase(Ease.Linear);

            transform.DOKill();
            transform.localScale = Vector3.one * 0.5f;
            transform.DOScale(Vector3.one, 0.15f);
            
            await Task.Delay(100);
            
            _cardBack.gameObject.SetActive(false);
            
            _cardFront.gameObject.SetActive(true);
            _cardFront.transform.localScale = new Vector3(1, 0, 1);
            _cardFront.transform.DOKill();
            _cardFront.transform.DOScaleY(1, 0.3f).SetEase(Ease.OutBounce);

            transform.DOShakeRotation(0.5f, new Vector3(0, 0, 1) * 40);
                
            await Task.Delay(300);

            SetSpritesSortingOrder(0);
            IsInitialized = true;
        }

        private void Update()
        {
            if (IsInitialized == false)
            {
                return;
            }
            
            HandleParent();
            CheckCardCollision();
        }

        private RaycastHit[] _collisionHits = new RaycastHit[32];
        private Vector3 _collisionSpeed;
        private bool _wasColliding;
        private void CheckCardCollision()
        {
            if (IsHeld || Parent != null)
            {
                return;
            }

            bool isColliding = false;
            int hits = Physics.BoxCastNonAlloc(transform.position, CollisionRaycastSize / 2, Vector3.up, _collisionHits);
            for (int i = 0; i < hits; i++)
            {
                if (_collisionHits[i].collider.TryGetComponent(out CardController card) && card != this && card.IsHeld == false && IsCardAChild(Child) == false)
                {
                    Vector3 direction = (transform.position - card.transform.position).normalized;
                    _collisionSpeed = direction * (4f * Time.deltaTime);
                    isColliding = true;
                    SetSpritesSortingOrder(transform.position.y < card.transform.position.y ? 1 : -1);
                }
            }

            if (isColliding != _wasColliding)
            {
                SetSpritesSortingOrder();
            }
            _wasColliding = isColliding;
            
            transform.position += _collisionSpeed;
            _collisionSpeed = Vector3.Lerp(_collisionSpeed, Vector3.zero, 0.1f);
        }

        private bool IsCardAChild(CardController card)
        {
            if (Child == null)
            {
                return false;
            }
            if (Child == card)
            {
                return true;
            }
            return Child.IsCardAChild(card);
        }

        public void SetParent(CardController parentCard)
        {
            if (parentCard != null && (parentCard.Parent == this || parentCard.Child != null))
            {
                return;
            }
            
            if (Parent != null)
            {
                Parent.Child = null;
                StackOrder = 0;
            }
            
            Parent = parentCard;
            
            if (Parent != null)
            {
                Parent.Child = this;
                StackOrder = Parent.StackOrder + 1;
            }

            UpdateStackOrderOfChildren();
        }

        public void UpdateStackOrderOfChildren()
        {
            if (Child == null)
            {
                return;
            }
            Child.StackOrder = StackOrder + 1;
            Child.UpdateStackOrderOfChildren();
            SetSpritesSortingOrder();
        }

        private void HandleParent()
        {
            if (Parent == null)
            {
                return;
            }
            Vector3 targetPosition = Parent.transform.position - Parent.transform.up * _parentingDistance;
            transform.position = targetPosition;
        }

        public override void GetHeld(bool isHeld)
        {
            transform.DOKill();
            
            if (isHeld == IsHeld)
            {
                return;
            }
            IsHeld = isHeld;
            
            SetParent(isHeld ? null : Parent);
            SetShadow(isHeld ? ShadowHeldDistance : ShadowBaseDistance);
            
            SetSpritesSortingOrder(isHeld ? 10 : 0);
            
            Child?.SetHeldAsChild(isHeld);

            if (isHeld == false)
            {
                //raycast toward camera to see if there is a playzone on mouse position
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                int hits = Physics.RaycastNonAlloc(ray, _collisionHits);
                for (int i = 0; i < hits; i++)
                {
                    if (_collisionHits[i].collider.TryGetComponent(out PlayZoneController playZone))
                    {
                        LastPlayZonePosition = _collisionHits[i].point;
                        break;
                    }
                }
                transform.DOMove(LastPlayZonePosition, 0.2f);
            }
        }

        public override void CardUnderOnDrop(CardController cardUnder)
        {
            SetParent(cardUnder);
        }

        private void SetHeldAsChild(bool isHeld)
        {
            IsHeld = isHeld;
            
            SetSpritesSortingOrder(isHeld ? 10 : 0);

            SetShadow(isHeld ? ShadowHeldDistance : ShadowBaseDistance);
            
            Child?.SetHeldAsChild(isHeld);
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, CollisionRaycastSize);
        }

#endif
    }
}