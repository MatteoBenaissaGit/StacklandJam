using System;
using Cards;
using Data.Cards;
using MatteoBenaissaLibrary.SingletonClassBase;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [field:SerializeField] public BoardManager Board { get; private set; }
    [field:SerializeField] public CardInputManager CardInput { get; private set; }
    
    [SerializeField] private CardData _testCardData;

    private void Start()
    {
        Board.CreateCard(_testCardData, Board.transform.position);
        Board.CreateCard(_testCardData, Board.transform.position + Vector3.left * 3);
    }
}