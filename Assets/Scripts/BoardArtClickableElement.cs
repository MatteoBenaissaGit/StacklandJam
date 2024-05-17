using System;
using DG.Tweening;
using MatteoBenaissaLibrary.AudioManager;
using UnityEngine;
using Random = System.Random;

public enum ActionType
{
    PunchScale = 0,
    Rotate = 1,
    Jump = 2
}

public class BoardArtClickableElement : MonoBehaviour
{
    [SerializeField] private ActionType _action;
    
    RaycastHit[] _hits = new RaycastHit[16];
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) == false)
        {
            return;
        }
        
        //do a raycast non alloc to check if the mouse is over the object
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int hitsCount = Physics.RaycastNonAlloc(ray, _hits, 1000f);
        for (int i = 0; i < hitsCount; i++)
        {
            if (_hits[i].collider.gameObject == gameObject)
            {
                Action();
                break;
            }
        }
    }

    private void Action()
    {
        SoundManager.Instance?.PlaySound(SoundEnum.Human, 0.04f);
        
        transform.DOKill();

        switch (_action)
        {
            case ActionType.PunchScale:
                transform.DOPunchScale(Vector3.one, 0.2f);
                break;
            case ActionType.Rotate:
                Vector3 angle = transform.localRotation.eulerAngles;
                transform.DORotate(new Vector3(angle.x, angle.y + UnityEngine.Random.Range(-90,90), angle.z), 0.5f, RotateMode.FastBeyond360);
                break;
            case ActionType.Jump:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}