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
        websocket = new WebSocket("ws://59.12.167.192:5215/ws");

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