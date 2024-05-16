using DG.Tweening;
using MatteoBenaissaLibrary.Attributes.ReadOnly;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace MatteoBenaissaLibrary.Menu
{
    public class ButtonManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Parameters")]
        
        [SerializeField, Range(0,2)]
        private float animationRange = .25f;
        
        [SerializeField, Range(0,2)]
        private float animationTime = .5f;

        [SerializeField] 
        private MenuType _menuType;

        [ReadOnly] 
        public bool CanAnimate = true;
        
        [Header("Events")]
        
        public UnityEvent OnButtonHover = new UnityEvent();
        public UnityEvent OnButtonExitHover = new UnityEvent();
        
        //privates
        
        private float startX;
        private Vector3 startSize;

        private void Start()
        {
            startX = transform.position.x;
            startSize = transform.localScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.DOKill();
            if (CanAnimate == false) return;
            
            switch (_menuType)
            {
                case MenuType.Simple:
                    transform.DOScale(startSize + Vector3.one * animationRange, animationTime);
                    break;
                case MenuType.SideSlide:
                    transform.DOMoveX(startX + animationRange, animationTime);
                    break;
            }
            
            OnButtonHover.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.DOKill();
            if (CanAnimate == false) return;
            
            switch (_menuType)
            {
                case MenuType.Simple:
                    transform.DOScale(startSize, animationTime);
                    break;
                case MenuType.SideSlide:
                    transform.DOMoveX(startX, animationTime);
                    break;
            }
            
            OnButtonExitHover.Invoke();
        }
        
        
    }
}

