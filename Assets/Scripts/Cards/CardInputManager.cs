using System;
using UnityEngine;

namespace Cards
{
    public class CardInputManager : MonoBehaviour
    {
        public CardController CurrentHeldCard { get; private set; }

        [SerializeField] private Camera _camera;

        private void Update()
        {
            CheckForCardClick();
            ManageHeldCard();
        }

        private RaycastHit[] _cardRaycasts = new RaycastHit[16];
        private void CheckForCardClick()
        {
            if (Input.GetMouseButtonDown(0) == false || CurrentHeldCard != null)
            {
                return;
            }
            
            //raycast toward camera to see if there is a card on mouse position
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            int hits = Physics.RaycastNonAlloc(ray, _cardRaycasts);
            for (int i = 0; i < hits; i++)
            {
                if (_cardRaycasts[i].collider.TryGetComponent(out CardController card))
                {
                    CurrentHeldCard = card;
                    CurrentHeldCard.GetHeld(true);
                    break;
                }
            }
        }
        
        private void ManageHeldCard()
        {
            if (CurrentHeldCard == null)
            {
                return;
            }
            
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            int hits = Physics.RaycastNonAlloc(ray, _cardRaycasts);
            Vector3 targetPosition = CurrentHeldCard.transform.position;
            for (int i = 0; i < hits; i++)
            {
                if (_cardRaycasts[i].collider.TryGetComponent(out CardController card) == false)
                {
                    targetPosition = _cardRaycasts[i].point;
                    break;
                }
            }
            CurrentHeldCard.transform.position = Vector3.Lerp(CurrentHeldCard.transform.position, targetPosition, 0.1f);

            if (Input.GetMouseButtonUp(0))
            {
                CurrentHeldCard.GetHeld(false);
                
                for (int i = 0; i < hits; i++)
                {
                    if (_cardRaycasts[i].collider.TryGetComponent(out CardController card) && card != CurrentHeldCard)
                    {
                        CurrentHeldCard.SetParent(card);
                        break;
                    }
                }
                
                CurrentHeldCard = null;
            }
        }
    }
}