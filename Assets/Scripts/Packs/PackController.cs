using System;
using System.Collections.Generic;
using Cards;
using Data.Cards;
using Data.Packs;
using DG.Tweening;
using Draggable;
using MatteoBenaissaLibrary.AudioManager;
using UnityEngine;

namespace Packs
{
    public class PackController : BoardDraggable
    {
        [SerializeField] private List<PackData> _packDatas = new List<PackData>();
        [SerializeField] private Transform _reflect;

        private Vector3 _baseReflectPosition;
        private int _packContentIndex;
        List<CardData> _packContent = new List<CardData>();

        private float _currentHeldTime;
        private PackData _packData;
        
        private void Start()
        {
            SoundManager.Instance.PlaySound(SoundEnum.Pack);
            
            _packData = _packDatas[UnityEngine.Random.Range(0, _packDatas.Count)];
            
            _baseReflectPosition = _reflect.localPosition;
            _packContentIndex = _packData.NumberOfCardsTotal - 1;
            _packContent = _packData.GetRandomCards();

            IsInitialized = true;
            
            _reflect.gameObject.SetActive(false);
        }

        protected override void Update()
        {
            if (IsHeld)
            {
                base.Update();
                _currentHeldTime += Time.deltaTime;
            }
        }

        private void OpenPack()
        {
            if (_packContentIndex < 0)
            {
                return;
            }
            
            float angle = (360f / (_packData.NumberOfCardsTotal)) * (_packContentIndex);
            float angleInRadians = angle * Mathf.Deg2Rad;
            float newX = 4f * Mathf.Cos(angleInRadians);
            float newY = 4f * Mathf.Sin(angleInRadians);

            Vector3 spawnPosition = new Vector3(transform.position.x + newX, transform.position.y, transform.position.z + newY);

            ReflectEffect();
            GameManager.Instance.Board.CreateCard(_packContent[_packContentIndex], transform.position, spawnPosition);
                
            _packContentIndex--;
            if (_packContentIndex < 0)
            {
                DestroyPack();;
            }
        }

        public void ReflectEffect()
        {
            _reflect.gameObject.SetActive(true);
            
            _reflect.DOComplete();
            _reflect.transform.localPosition = _baseReflectPosition;
            _reflect.transform.DOLocalMoveY(_baseReflectPosition.y + 16f, 0.25f).SetEase(Ease.InOutSine)
                .OnComplete(() => _reflect.gameObject.SetActive(false));

            transform.DOComplete();
            transform.DOPunchScale(Vector3.one * 0.1f, 0.25f);
            transform.DOPunchRotation(Vector3.one * 5f, 0.25f);
        }

        private void DestroyPack()
        {
            transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InCirc).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }

        private RaycastHit[] _raycastHits = new RaycastHit[16];
        
        public override void GetHeld(bool isHeld)
        {
            if (isHeld == IsHeld)
            {
                return;
            }
            IsHeld = isHeld;

            transform.DOComplete();

            SetShadow(isHeld ? ShadowHeldDistance : ShadowBaseDistance);
            SetSpritesSortingOrder(isHeld ? 100 : 0);

            if (isHeld == false)
            {
                if (_currentHeldTime < 0.2f)
                {
                    OpenPack();
                }
                _currentHeldTime = 0f;
                
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
    }
}