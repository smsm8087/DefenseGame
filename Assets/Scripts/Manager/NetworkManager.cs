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
    
    private Dictionary<string, GameObject> players = new();
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
    
    private Dictionary<string, INetworkMessageHandler> _handlers;

    private void RegisterHandlers()
    {
        //서버 리시브 처리 부분.
        AddHandler(new PlayerJoinHandler(playerPrefab,players, this));
        AddHandler(new PlayerMoveHandler(players));
        AddHandler(new PlayerListHandler(playerPrefab, players));
        AddHandler(new PlayerLeaveHandler(players));
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
