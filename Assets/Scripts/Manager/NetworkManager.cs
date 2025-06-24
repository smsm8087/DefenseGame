using System;
using System.Collections.Generic;
using System.Text;
using NativeWebSocket;
using NativeWebSocket.MessageHandlers;
using UnityEngine;
using Newtonsoft.Json;
using UI;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;
    public List<GameObject> playerPrefabs;
    
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private SharedHpManager sharedHpManager;
    [SerializeField] private CenterText centerText;
    [SerializeField] private GameObject DamageTextPrefab;
    
    private Dictionary<string, GameObject> players = new();
    private Dictionary<string, GameObject> enemies = new();
    private Dictionary<string, INetworkMessageHandler> handlers = new();
    private event Action onGameOver;
    public string MyGUID { get; private set; }
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        RegisterHandlers();
        WebSocketClient.Instance.OnMessageReceived += HandleMessage;
    }

    public Dictionary<string, GameObject> GetEnemies()
    {
        return  enemies;
    }
    

    public void SetOnGamveOverAction(Action onGameOver)
    {
        this.onGameOver += onGameOver;
    }

    public void TriggerGameOver()
    {
        this.onGameOver?.Invoke();
    }
    public void SetMyGUID(string guid)
    {
        MyGUID = guid;
    }

    public void RemovePlayer(string guid)
    {
        if (!players.ContainsKey(guid)) return;
        Destroy(players[guid]);
        players.Remove(guid);
    }

    public void RemoveEnemy(string guid)
    {
        if (!enemies.ContainsKey(guid)) return;
        Destroy(enemies[guid]);
        enemies.Remove(guid);
    }

    public void RemoveAllEnemies()
    {
        foreach (var enemyData in enemies)
        {
            GameObject enemyObj = enemyData.Value;
            Destroy(enemyObj);
        }
        enemies.Clear();
    }

    public void ResetHp()
    {
        //임시 초기화
        sharedHpManager.UpdateHPBar(100,100);
    }
    private Dictionary<string, INetworkMessageHandler> _handlers;

    private void RegisterHandlers()
    {
        //서버 리시브 처리 부분.
        AddHandler(new PlayerJoinHandler(playerPrefabs,players, this));
        AddHandler(new PlayerMoveHandler(players));
        AddHandler(new PlayerListHandler(playerPrefabs, players));
        AddHandler(new PlayerLeaveHandler());
        AddHandler(new SpawnEnemyHandler(enemies,waveManager));
        AddHandler(new EnemySyncHandler(enemies));
        AddHandler(new EnemyDieHandler(enemies));
        AddHandler(new SharedHpUpdateHandler(sharedHpManager));
        AddHandler(new CountDownHandler(centerText));
        AddHandler(new GameOverHandler(centerText));
        AddHandler(new RestartHandler());
        AddHandler(new PlayerAnimationHandler(players));
        AddHandler(new EnemyDamagedHandler(enemies,DamageTextPrefab));
        AddHandler(new EnemyAttackHandler(enemies));
        AddHandler(new SettlementStartHandler(centerText));
    }

    private void AddHandler(INetworkMessageHandler handler)
    {
        handlers[handler.Type] = handler;
    }

    public void HandleMessage(string msg)
    {
        NetMsg netMsg = JsonConvert.DeserializeObject<NetMsg>(msg);
        if (handlers.TryGetValue(netMsg.type, out var handler))
        {
            handler.Handle(netMsg);
        }
        else
        {
            Debug.LogWarning($"Unhandled message type: {netMsg.type}");
        }
    }
    
    public void SendMsg(object msg)
    {
        string json = JsonConvert.SerializeObject(msg);
        WebSocketClient.Instance.Send(json);
    }
}
