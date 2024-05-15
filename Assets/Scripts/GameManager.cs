using System;
using System.Threading.Tasks;
using Cards;
using Data.Cards;
using MatteoBenaissaLibrary.SingletonClassBase;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [field:SerializeField] public BoardManager Board { get; private set; }
    [field:SerializeField] public UIManager UI { get; private set; }
    [field:SerializeField] public BoardElementInputManager BoardElementInput { get; private set; }
}