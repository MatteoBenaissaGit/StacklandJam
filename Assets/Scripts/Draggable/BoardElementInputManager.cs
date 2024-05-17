using System;
using Draggable;
using MatteoBenaissaLibrary.AudioManager;
using UnityEngine;

namespace Cards
{
    public class BoardElementInputManager : MonoBehaviour
    {
        public BoardDraggable CurrentHeldElement { get; set; }

        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _layerToCheck;

        private void Update()
        {
            CheckForHoverElement();
            CheckForElementOnClick();
            ManageHeldElement();
        }

        private RaycastHit[] _cardRaycasts = new RaycastHit[16];
        private void CheckForElementOnClick()
        {
            if (Input.GetMouseButtonUp(0) && CurrentHeldElement != null)
            {
                SoundManager.Instance?.PlaySound(SoundEnum.CardDrop, 0.05f);
            }
            
            if (Input.GetMouseButtonDown(0) == false || CurrentHeldElement != null)
            {
                return;
            }
            
            //raycast toward camera to see if there is a card on mouse position
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            int hits = Physics.RaycastNonAlloc(ray, _cardRaycasts, 1000f, _layerToCheck);
            for (int i = 0; i < hits; i++)
            {
                if (_cardRaycasts[i].collider.TryGetComponent(out BoardDraggable draggable) && draggable.IsHeld == false && draggable.IsInitialized)
                {
                    CurrentHeldElement = draggable;
                    CurrentHeldElement?.GetHeld(true);
                    SoundManager.Instance?.PlaySound(SoundEnum.CardHeld);
                    break;
                }
            }
        }

        private BoardDraggable _currentElementHovered;
        private void CheckForHoverElement()
        {
            if (CurrentHeldElement != null)
            {
                return;
            }
            
            BoardDraggable hoveredElement = null;
            
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            int hits = Physics.RaycastNonAlloc(ray, _cardRaycasts, 1000f, _layerToCheck);
            for (int i = 0; i < hits; i++)
            {
                if (_cardRaycasts[i].collider.TryGetComponent(out BoardDraggable draggable) && draggable.IsHeld == false && draggable.IsInitialized)
                {
                    if (draggable == _currentElementHovered && _currentElementHovered != null)
                    {
                        return;
                    }
                    hoveredElement = draggable;
                }
            }
            
            
            if (_currentElementHovered != null) 
            { 
                _currentElementHovered.SetHovered(false);
            }
            _currentElementHovered = hoveredElement;
            if (_currentElementHovered != null) 
            { 
                _currentElementHovered.SetHovered(true);
            }
            
        }
        
        private void ManageHeldElement()
        {
            if (CurrentHeldElement == null)
            {
                return;
            }
            
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            int hits = Physics.RaycastNonAlloc(ray, _cardRaycasts, 1000f, _layerToCheck);
            Vector3 targetPosition = CurrentHeldElement.transform.position;
            for (int i = 0; i < hits; i++)
            {
                if (_cardRaycasts[i].collider.TryGetComponent(out BoardDraggable card) == false)
                {
                    targetPosition = _cardRaycasts[i].point;
                    break;
                }
            }
            CurrentHeldElement.transform.position = Vector3.Lerp(CurrentHeldElement.transform.position, targetPosition, 0.1f);

            //drop card
            if (Input.GetMouseButtonUp(0))
            {
                CurrentHeldElement.GetHeld(false);

                if (CurrentHeldElement.TryGetComponent(out CardController heldCard))
                {
                    for (int i = 0; i < hits; i++)
                    {
                        if (_cardRaycasts[i].collider.TryGetComponent(out ICardHolder holder))
                        {
                            holder.HoldCard(heldCard);
                            break;
                        }
                        
                        if (_cardRaycasts[i].collider.TryGetComponent(out CardController card) && card != CurrentHeldElement && card.Child == null)
                        {
                            heldCard.CardUnderOnDrop(card);
                            break;
                        }
                    }
                }

                CurrentHeldElement = null;
            }
        }
    }
}