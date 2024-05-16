using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cards;
using Data.Cards;
using MatteoBenaissaLibrary.SingletonClassBase;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [field:SerializeField] public BoardManager Board { get; private set; }
    [field:SerializeField] public UIManager UI { get; private set; }
    [field:SerializeField] public BoardElementInputManager BoardElementInput { get; private set; }
    [field:SerializeField] public CardData PizzaColdNotFull { get; private set; }
    [field:SerializeField] public CardData PizzaCold { get; private set; }
    [field:SerializeField] public CardData PizzaHotNotFull { get; private set; }
    [field:SerializeField] public CardData PizzaHot { get; private set; }

    public List<Client> CurrentClients { get; private set; } = new List<Client>();
    
    [Space(10)] [SerializeField] private float _timePerDay;
    [SerializeField] private int _quotaPerDay;
    [SerializeField] private Vector2 _timeBetweenClients;
    [SerializeField] private Client _clientPrefab;
    [SerializeField] private Transform _clientSpawnPoint;
    
    private int _currentDay = 0;
    private float _currentTime;

    private float _currentClientsSpawnTime;
    private int _currentQuota;

    private void Start()
    {
        _currentQuota = _quotaPerDay;
        SetNewDay();
    }

    private void Update()
    {
        _currentTime -= Time.deltaTime;
        UI.UpdateFillDay(_currentTime / _timePerDay);
        if (_currentTime <= 0)
        {
            SetNewDay();
        }

        float quotaFill = (CurrentClients.Count <= 0 ? 0 : CurrentClients[0].CurrentTime) /
                          (CurrentClients.Count <= 0 ? 0 : CurrentClients[0].TotalTime);
        UI.UpdateFillQuota(quotaFill);
    }

    private void SetNewDay()
    {
        if (_currentQuota < _quotaPerDay)
        {
            //TODO: Game Over
            SceneManager.LoadScene(0);
            return;
        }
        
        _currentQuota = 0;
        _currentClientsSpawnTime = UnityEngine.Random.Range(_timeBetweenClients.x, _timeBetweenClients.y);
        UI.UpdateQuota(_currentQuota, _quotaPerDay);
        
        _currentDay++;
        _currentTime = _timePerDay;
        UI.SetNewDay(_currentDay);
        
        //TODO random time
        AddClient();
    }

    private void AddClient()
    {
        Client client = Instantiate(_clientPrefab);
        client.Initialize(_clientSpawnPoint.position);
        
        CurrentClients.Add(client);
    }

    public void ClientServed(Client client)
    {
        CurrentClients.Remove(client);
        client.DestroyClient();
        
        _currentQuota++;
        UI.UpdateQuota(_currentQuota, _quotaPerDay);
    }

    public void KillClient(Client client)
    {
        CurrentClients.Remove(client);
        client.DestroyClient();
    }
}