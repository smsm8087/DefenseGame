using System;
using System.Collections.Generic;
using System.Text;
using NativeWebSocket;
using UnityEngine;
using Newtonsoft.Json;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;
    public GameObject playerPrefab;
    
    [SerializeField] private WaveManager waveManager;
    
    private Dictionary<string, GameObject> players = new();
    private Dictionary<string, GameObject> enemies = new();
    private Dictionary<string, INetworkMessageHandler> handlers = new();
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
    
    private Dictionary<string, INetworkMessageHandler> _handlers;

    private void RegisterHandlers()
    {
        //서버 리시브 처리 부분.
        AddHandler(new PlayerJoinHandler(playerPrefab,players, this));
        AddHandler(new PlayerMoveHandler(players));
        AddHandler(new PlayerListHandler(playerPrefab, players));
        AddHandler(new PlayerLeaveHandler());
        AddHandler(new SpawnEnemyHandler(enemies,waveManager));
        AddHandler(new EnemySyncHandler(enemies));
        AddHandler(new EnemyDieHandler(enemies));
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
