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
        [SerializeField] private GameObject _useBar;
        [SerializeField] private Transform _useBarFill;

        [Space(10)]
        [SerializeField] private float _parentingDistance = 1f;

        private float _currentTimeToUse;
        private bool _isUsing;
        
        public void Initialize(CardData data, Vector3 position)
        {
            ActivateUse(false, null, null);
            
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

            ManageUsing();
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

                CardController currentChild = this;
                while (currentChild != null)
                {
                    currentChild.CheckForActivation();
                    currentChild = currentChild.Child;
                }
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

            if (Child != null)
            {
                Child.SetHeldAsChild(isHeld);
            }

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
            
            GameManager.Instance.UI.SetDescription(isHovered, Data);
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
                || (wantToBeChildCard.Data.Type == CardType.Money && Data.Type != CardType.Money)
                || wantToBeChildCard.Data.IsPizza && wantToBeChildCard.Data.IsCold == false
                || (wantToBeChildCard.Data.Type == CardType.Resource && Data.IsCold)
                || (Data.IsPizza && wantToBeChildCard.Data.Type == CardType.Resource))
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

        public List<CardController> GetParents()
        {
            List<CardController> parentList = new List<CardController>();

            CardController currentParent = Parent;
            while (currentParent != null)
            {
                parentList.Add(currentParent);
                currentParent = currentParent.Parent;
            }

            return parentList;
        }

        public void CheckForActivation()
        {
            if (Data.Type != CardType.Human)
            {
                return;
            }
            
            List<CardController> parents = GetParents();
            if (parents.Count == 0)
            {
                return;
            }

            CardController rootCard = parents[^1];
            if (rootCard.Data.Type == CardType.Usable)
            {
                parents.Remove(rootCard);
                rootCard.ActivateUse(this, parents.ToArray());
            }
        }

        public void ActivateUse(CardController cardHuman, params CardController[] elements)
        {
            _currentDoingElements = elements.ToList();
            if (Data.IsFurnace)
            {
                if (elements[0].Data.IsPizza && elements[0].Data.IsCold)
                {
                    ActivateUse(true, elements[0].Data.IsFull ? GameManager.Instance.PizzaHot : GameManager.Instance.PizzaHotNotFull, cardHuman);
                }
            }
            else if (Data.IsWorkSpace)
            {
                if (elements.Length >= 3)
                {
                    ActivateUse(true, GameManager.Instance.PizzaCold, cardHuman);
                }
                else if (elements.Length >= 1)
                {
                    ActivateUse(true, GameManager.Instance.PizzaColdNotFull, cardHuman);
                }
            }
        }

        private CardData _currentDoingData;
        private List<CardController> _currentDoingElements;
        private CardController _currentDoingHuman;
        private List<CardController> _childrenOnActivation = new List<CardController>();

        private void ActivateUse(bool doActivate, CardData cardData, CardController cardHuman)
        {
            _useBar.SetActive(doActivate);
            _isUsing = doActivate;

            if (cardData == null || cardHuman == null)
            {
                return;
            }

            _childrenOnActivation = GetChildren();
            
            _currentDoingHuman = cardHuman;
            
            _currentTimeToUse = Data.TimeToUse;
            _currentDoingData = cardData;
            
            _useBarFill.transform.localScale = new Vector3(0, 1, 1);
        }
        
        private void ManageUsing()
        {
            if (_isUsing == false || IsHeld)
            {
                return;
            }

            List<CardController> currentChildren = GetChildren();
            foreach (CardController child in _childrenOnActivation)
            {
                if (currentChildren.Contains(child) == false)
                {
                    CancelUsing();
                    return;
                }
            }

            _currentTimeToUse -= Time.deltaTime;
            _useBarFill.transform.localScale = new Vector3(_currentTimeToUse / Data.TimeToUse, 1, 1);
            
            if (_currentTimeToUse <= 0)
            {
                _isUsing = false;
                ActivateUse(false, null, null);
                
                _currentDoingElements.ForEach(x => GameManager.Instance.Board.RemoveCard(x));
                _currentDoingHuman.SetParent(null);
                
                GameManager.Instance.Board.CreateCard(_currentDoingData, transform.position, transform.position - transform.up * 1f + transform.right * 2f);
            }
        }

        public void CancelUsing()
        {
            _isUsing = false;
            ActivateUse(false, null, null);
        }

        public List<CardController> GetChildren()
        {
            List<CardController> children = new List<CardController>();

            CardController currentChild = Child;
            while (currentChild != null)
            {
                children.Add(currentChild);
                currentChild = currentChild.Child;
            }

            return children;
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