﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cards;
using Data.Cards;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MatteoBenaissaLibrary.SingletonClassBase.Singleton<GameManager>
{
    [field:SerializeField] public BoardManager Board { get; private set; }
    [field:SerializeField] public CameraController CameraController { get; private set; }
    [field:SerializeField] public UIManager UI { get; private set; }
    [field:SerializeField] public BoardElementInputManager BoardElementInput { get; private set; }
    [field:SerializeField] public CardData PizzaColdNotFull { get; private set; }
    [field:SerializeField] public CardData PizzaCold { get; private set; }
    [field:SerializeField] public CardData PizzaHotNotFull { get; private set; }
    [field:SerializeField] public CardData PizzaHot { get; private set; }
    [field:SerializeField] public CardData Money { get; private set; }

    public List<Client> CurrentClients { get; private set; } = new List<Client>();
    
    [Space(10)] [SerializeField] private float _timePerDay;
    [SerializeField] private int _quotaPerDay;
    [SerializeField] private Vector2 _timeBetweenClients;
    [SerializeField] private Client _clientPrefab;
    [SerializeField] private Transform _clientSpawnPoint;
    
    private int _currentDay = 0;
    private float _currentTime;
    private bool _doDayTime = true;
    private bool _doSpawnClient = true;

    private float _currentClientsSpawnTime;
    private float _baseClientsSpawnTime;
    private int _currentQuota;

    private void Start()
    {
        _currentQuota = _quotaPerDay;
        
        _currentClientsSpawnTime = 5f;
        _baseClientsSpawnTime = 5f;
        
        SetNewDay();
    }

    private void Update()
    {
        ManageDayTime();
        ManageClientSpawn();
    }

    private void ManageDayTime()
    {
        if (_doDayTime == false)
        {
            return;
        }

        _currentTime -= Time.deltaTime;
        UI.UpdateFillDay(_currentTime / _timePerDay);
        if (_currentTime <= 0)
        {
            SetNewDay();
        }
    }

    private void SetNewDay()
    {
        if (_currentQuota < _quotaPerDay)
        {
            GameOver();
            return;
        }
        else if (_currentDay > 0)
        {
            CameraController.Confettis.Play();
        }
        
        _currentQuota = 0;
        UI.UpdateQuota(_currentQuota, _quotaPerDay);
        
        _currentDay++;
        _currentTime = _timePerDay;
        UI.SetNewDay(_currentDay);
    }

    private void AddClient()
    {
        Client client = Instantiate(_clientPrefab);
        client.Initialize(_clientSpawnPoint.position - _clientSpawnPoint.forward * UnityEngine.Random.Range(0,18));
        
        CurrentClients.Add(client);
    }

    public void ClientServed(Client client, int moneyGained)
    {
        CurrentClients.Remove(client);
        client.DestroyClient();
        
        _currentQuota++;
        UI.UpdateQuota(_currentQuota, _quotaPerDay);

        Vector3 offset = client.transform.right * 0;
        for (int i = 0; i < moneyGained; i++)
        {
            offset = client.transform.right * (i + (moneyGained > 1 ? -1 : 0));
            Board.CreateCard(Money, client.transform.position, client.transform.position - client.transform.up * 2 + offset);
        }
    }

    public void KillClient(Client client)
    {
        CurrentClients.Remove(client);
        client.DestroyClient(true);
    }

    private void ManageClientSpawn()
    {
        if (_doSpawnClient == false)
        {
            return;
        }
        
        _currentClientsSpawnTime -= Time.deltaTime;
        UI.UpdateFillQuota(_currentClientsSpawnTime/_baseClientsSpawnTime);
        
        if (_currentClientsSpawnTime <= 0)
        {
            _currentClientsSpawnTime = UnityEngine.Random.Range(_timeBetweenClients.x, _timeBetweenClients.y);
            _baseClientsSpawnTime = _currentClientsSpawnTime;
            AddClient();
        }
    }

    private void GameOver()
    {
        UI.ShowGameOver();
        
        _doSpawnClient = false;
        _doDayTime = false;
        Board.Cards.ForEach(x => x.IsInitialized = false);
        CurrentClients.ForEach(x => x.DestroyClient());
    }
}