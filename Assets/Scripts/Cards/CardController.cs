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

        [SerializeField] private SpriteRenderer _cardSpriteRenderer;
        [SerializeField] private Transform _cardFront;
        [SerializeField] private Transform _cardBack;
        [SerializeField] private SpriteRenderer _highlight;

        [Space(10)]
        [SerializeField] private float _parentingDistance = 1f;

        public void Initialize(CardData data, Vector3 position)
        {
            Color highlightColor = _highlight.color;
            highlightColor.a = 0;
            _highlight.color = highlightColor;
            
            Data = data;

            _cardSpriteRenderer.sprite = Data.Sprite;
            
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

            await Task.Delay(200);

            IsInitialized = true;
        }

        protected override void Update()
        {
            if (IsInitialized == false)
            {
                return;
            }

            if (IsHeld)
            {
                base.Update();
            }
            
            CheckCardCollision();
            HandleParent();
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
                    direction.y = 0;
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
            _collisionSpeed = Vector3.Lerp(_collisionSpeed, Vector3.zero, 0.2f * 60f * Time.deltaTime);
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
            SetSpritesSortingOrder();
        }

        public void UpdateStackOrderOfChildren()
        {
            if (Child == null)
            {
                return;
            }
            
            Child.StackOrder = StackOrder + 1;
            Child.SetSpritesSortingOrder();
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
            transform.localRotation = Parent.transform.localRotation;
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
            
            SetSpritesSortingOrder(isHeld ? 100 : 0);
            
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

                GameManager.Instance.Board.UnhighlightAll();
            }
            else
            {
                GameManager.Instance.Board.HighlightsPossiblesChildrenOf(this);
            }
        }

        public override void CardUnderOnDrop(CardController cardUnder)
        {
            if (cardUnder.CanHaveChild(this) == false)
            {
                return;
            }
            SetParent(cardUnder);
        }

        private void SetHeldAsChild(bool isHeld)
        {
            IsHeld = isHeld;
            
            SetSpritesSortingOrder(isHeld ? 100 : 0);

            SetShadow(isHeld ? ShadowHeldDistance : ShadowBaseDistance);
            
            Child?.SetHeldAsChild(isHeld);
        }

        public override void SetHovered(bool isHovered)
        {
            base.SetHovered(isHovered);
            
            GameManager.Instance.UI.SetDescription(isHovered, Data.Description);
        }

        public bool CanHaveChild(CardController wantToBeChildCard)
        {
            //if i already have a child
            if (Child != null 
                || Data.Type == CardType.Human
                || wantToBeChildCard == this 
                || wantToBeChildCard.Data.Type == CardType.Usable
                || (Data.IsFurnace && wantToBeChildCard.Data.CanBePutInFurnace == false)
                || (Data.Type == CardType.Money && wantToBeChildCard.Data.Type != CardType.Money)
                || Parent == wantToBeChildCard
                || (wantToBeChildCard.Data.Type == CardType.Money && Data.Type != CardType.Money))
            {
                return false;
            }

            //if a parent/child is the same card and its not stackable
            CardController currentWantToBeChildChild = wantToBeChildCard;
            while (currentWantToBeChildChild != null)
            {
                CardController currentParent = this;
                while (currentParent != null)
                {
                    if (currentWantToBeChildChild.Data == currentParent.Data && currentParent.Data.IsStackable == false)
                    {
                        return false;
                    }

                    if (currentParent.Data.IsWorkSpace && currentWantToBeChildChild.Data.CanBePutOnWorkSpace == false)
                    {
                        return false;
                    }

                    if (currentParent == wantToBeChildCard)
                    {
                        return false;
                    }

                    currentParent = currentParent.Parent;
                }
                currentWantToBeChildChild = currentWantToBeChildChild.Child;
            }

            return true;
        }

        public void Highlight(bool doHighlight)
        {
            _highlight.DOFade(doHighlight ? 1 : 0 , 0.2f);
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