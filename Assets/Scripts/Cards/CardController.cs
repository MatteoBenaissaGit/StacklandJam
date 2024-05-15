using System;
using System.Collections.Generic;
using System.Linq;
using Data.Cards;
using DG.Tweening;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Cards
{
    public class CardController : MonoBehaviour
    {
        public CardData Data { get; private set; }
        public CardController Parent { get; private set; }
        public CardController Child { get; private set; }
        public int StackOrder { get; private set; }
        public bool IsHeld { get; set; }

        [SerializeField] private SpriteRenderer _iconSpriteRenderer;
        [SerializeField] private SpriteRenderer _cardSpriteRenderer;
        [SerializeField] private TextMeshPro _cardNameText;
        
        [Space(10)]
        [SerializeField] private Transform _shadow;
        [SerializeField] private Vector2 _shadowDirection = new Vector2(-1f, -1f);
        [SerializeField] private float _shadowBaseDistance = 0.1f;
        [SerializeField] private float _shadowHoverDistance = 0.3f;
        [SerializeField] private float _shadowHeldDistance = 1f;
        
        [Space(10)]
        [SerializeField] private float _parentingDistance = 1f;
        
        [Space(10)]
        [SerializeField] private Vector3 _collisionRaycastSize = Vector3.one;

        private Vector3 _lastPlayZonePosition;
        private Dictionary<SpriteRenderer, int> _spriteToBaseOrder = new Dictionary<SpriteRenderer, int>();
        private int _textBaseOrder;

        private void Awake()
        {
            foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
            {
                _spriteToBaseOrder.Add(sprite,sprite.sortingOrder);
            }
            _textBaseOrder = _cardNameText.sortingOrder;
        }

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

            _lastPlayZonePosition = position;
            transform.position = position;
            
            SetShadow(_shadowBaseDistance);
        }

        private void Update()
        {
            HandleParent();
            CheckCardCollision();
        }

        private RaycastHit[] _collisionHits = new RaycastHit[16];
        private Vector3 _collisionSpeed;
        private void CheckCardCollision()
        {
            if (IsHeld || Parent != null)
            {
                return;
            }

            bool isColliding = false;
            int hits = Physics.BoxCastNonAlloc(transform.position, _collisionRaycastSize / 2, Vector3.up, _collisionHits);
            for (int i = 0; i < hits; i++)
            {
                if (_collisionHits[i].collider.TryGetComponent(out CardController card) && card != this && card.IsHeld == false && card != Child)
                {
                    Vector3 direction = (transform.position - card.transform.position).normalized;
                    _collisionSpeed = direction * (4f * Time.deltaTime);
                    isColliding = true;
                    SetSpritesSortingOrder(transform.position.y < card.transform.position.y ? 1 : -1);
                }
            }

            if (isColliding == false)
            {
                SetSpritesSortingOrder();
            }
            
            transform.position += _collisionSpeed;
            _collisionSpeed = Vector3.Lerp(_collisionSpeed, Vector3.zero, 0.1f);
        }

        public void SetParent(CardController parentCard)
        {
            if (parentCard != null && parentCard.Parent == this)
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

        public void GetHeld(bool isHeld)
        {
            transform.DOKill();
            
            if (isHeld == IsHeld)
            {
                return;
            }
            IsHeld = isHeld;
            
            SetParent(isHeld ? null : Parent);
            SetShadow(isHeld ? _shadowHeldDistance : _shadowBaseDistance);

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
                        _lastPlayZonePosition = _collisionHits[i].point;
                        break;
                    }
                }
                transform.DOMove(_lastPlayZonePosition, 0.2f);
            }
        }

        private void SetHeldAsChild(bool isHeld)
        {
            IsHeld = isHeld;
            
            SetSpritesSortingOrder(isHeld ? 10 : 0);

            SetShadow(isHeld ? _shadowHeldDistance : _shadowBaseDistance);
            
            Child?.SetHeldAsChild(isHeld);
        }

        private void SetSpritesSortingOrder(int orderOffset = 0)
        {
            foreach(KeyValuePair<SpriteRenderer, int> pair in _spriteToBaseOrder)
            {
                pair.Key.sortingOrder = pair.Value + StackOrder + orderOffset;
            }
            _cardNameText.sortingOrder = _textBaseOrder + StackOrder + orderOffset;
        }

        private void SetShadow(float distance)
        {
            Vector3 desiredPosition = _shadowDirection * distance;
            
            _shadow.transform.DOKill();
            _shadow.transform.DOLocalMove(desiredPosition, 0.1f);
        }
        
#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, _collisionRaycastSize);
        }

#endif
    }
}