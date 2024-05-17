using System;
using System.Collections.Generic;
using Cards;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Draggable
{
    public abstract class BoardDraggable : MonoBehaviour
    {
        public int StackOrder { get; set; }
        public bool IsHeld { get; set; }
        public bool IsInitialized { get; set; }

        
        [Space(10)]
        [SerializeField] protected Vector3 CollisionRaycastSize = Vector3.one;
        
        [field:Space(10)]
        [field:SerializeField] protected Transform Shadow { get; set; }
        [field:SerializeField] protected Vector2 ShadowDirection { get; set; } = new Vector2(-1f, -1f);
        [field:SerializeField] protected float ShadowBaseDistance { get; set; } = 0.1f;
        [field:SerializeField] protected float ShadowHoverDistance { get; set; } = 0.25f;
        [field:SerializeField] protected float ShadowHeldDistance { get; set; } = 0.5f;

        protected Vector3 LastPlayZonePosition { get; set; }
        
        private Dictionary<SpriteRenderer, int> _spriteToBaseOrder = new Dictionary<SpriteRenderer, int>();
        private Dictionary<TextMeshPro, int> _textsBaseOrder = new Dictionary<TextMeshPro, int>();
        
        private Vector3 _previousPosition;
        private Vector3 _baseLocalRotation;

        private void Awake()
        {
            _baseLocalRotation = transform.localRotation.eulerAngles;
            
            foreach (SpriteRenderer sprite in GetComponentsInChildren<SpriteRenderer>())
            {
                _spriteToBaseOrder.Add(sprite,sprite.sortingOrder);
            }
            foreach (TextMeshPro text in GetComponentsInChildren<TextMeshPro>())
            {
                _textsBaseOrder.Add(text,text.sortingOrder);
            }
        }

        protected virtual void Update()
        {
            //TODO rotation ?
        }

        public virtual void Initialize(Vector3 position)
        {
            LastPlayZonePosition = position;
            transform.position = position;
            
            SetShadow(ShadowBaseDistance);
        }

        public abstract void GetHeld(bool isHeld);
        public abstract void CardUnderOnDrop(CardController cardUnder);

        protected void SetSpritesSortingOrder(int orderOffset = 0)
        {
            foreach(KeyValuePair<SpriteRenderer, int> pair in _spriteToBaseOrder)
            {
                if (pair.Key == null)
                {
                    continue;
                }
                pair.Key.sortingOrder = pair.Value + (StackOrder * 5) + orderOffset;
            }
            foreach(KeyValuePair<TextMeshPro, int> pair in _textsBaseOrder)
            {
                if (pair.Key == null)
                {
                    continue;
                }
                pair.Key.sortingOrder = pair.Value + (StackOrder * 5) + orderOffset;
            }
        }

        protected virtual void SetShadow(float distance)
        {
            if (Shadow.transform == null)
            {
                return;
            }
            
            Vector3 direction = new Vector3(ShadowDirection.x, ShadowDirection.y, 0);
            Vector3 desiredPosition = direction * distance;

            Shadow.transform.DOKill();
            Shadow.transform.DOLocalMove(desiredPosition, 0.1f);
        }

        public virtual void SetHovered(bool isHovered)
        {
            SetShadow(isHovered ? ShadowHoverDistance : IsHeld ? ShadowHeldDistance : ShadowBaseDistance);
        }
    }
}