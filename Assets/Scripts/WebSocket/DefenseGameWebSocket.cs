using UnityEngine;
using NativeWebSocket;
using System.Text;
using TMPro;
using UnityEngine.UI;

public class DefenseGameWebSocket : MonoBehaviour
{
    public static DefenseGameWebSocket Instance { get; private set; }
    public WebSocket websocket { get; private set; }
    async void Start()
    {
        Instance = this;
        // 설정 불러오기
        var config = Resources.Load<ServerConfig>("ServerConfig");
        if (config == null)
        {
            Debug.LogError("ServerConfig.asset이 Resources 폴더에 없습니다!");
            return;
        }

        string ip = config.GetServerIP();
        int port = config.port;

        string url = $"ws://{ip}:{port}/ws";
        
        websocket = new WebSocket(url);

        websocket.OnOpen += () =>
        {
            Debug.Log("서버에 연결됨!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("에러 발생: " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("연결 끊김");
        };

        websocket.OnMessage += (bytes) =>
        {
            var msg = Encoding.UTF8.GetString(bytes);
            NetworkManger.Instance.HandleMessage(msg);
        };
        await websocket.Connect();
    }

    void Update()
    {
        websocket?.DispatchMessageQueue();
    }

    private async void OnApplicationQuit()
    {
        await websocket?.Close();
    }
}