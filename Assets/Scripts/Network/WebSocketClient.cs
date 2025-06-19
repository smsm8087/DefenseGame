using UnityEngine;
using NativeWebSocket;
using System;
public class WebSocketClient : MonoBehaviour
{
    public static WebSocketClient Instance { get; private set; }

    private WebSocket websocket;

    public event Action<string> OnMessageReceived;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private string getUrl()
    {
        // 설정 불러오기
        var config = Resources.Load<ServerConfig>("ServerConfig");
        if (config == null)
        {
            Debug.LogError("ServerConfig.asset이 Resources 폴더에 없습니다!");
            return "";
        }
        string ip = config.GetServerIP();
        int port = config.port;
        return $"ws://{ip}:{port}/ws";
    }
    private async void Start()
    {
        websocket = new WebSocket(getUrl());

        websocket.OnOpen += () =>
        {
            Debug.Log("WebSocket connected.");
        };

        websocket.OnError += (e) =>
        {
            Debug.LogError($"WebSocket error: {e}");
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("WebSocket closed.");
        };

        websocket.OnMessage += (bytes) =>
        {
            string message = System.Text.Encoding.UTF8.GetString(bytes);
            OnMessageReceived?.Invoke(message);
        };

        await websocket.Connect();
    }

    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

    public async void Send(string message)
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            if (websocket.State == WebSocketState.Open)
            {
                await websocket.SendText(message);
            }
            else
            {
                Debug.LogWarning("WebSocket not connected.");
            }    
        }
    }
}