using UnityEngine;
using NativeWebSocket;
using System.Text;
using TMPro;
using UnityEngine.UI;

public class DefenseGameWebSocket : MonoBehaviour
{
    WebSocket websocket;
    public TMP_InputField inputField;
    public Button sendButton;
    async void Start()
    {
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
            Debug.Log("서버로부터 메시지: " + msg);
        };
        sendButton.onClick.AddListener(SendInputText);
        await websocket.Connect();
    }

    public async void SendInputText()
    {
        if (websocket.State == WebSocketState.Open)
        {
            string msg = inputField.text;
            if (!string.IsNullOrEmpty(msg))
            {
                await websocket.SendText(msg);
                inputField.text = ""; // 전송 후 입력창 비움
            }
        }
        else
        {
            Debug.Log("서버에 연결되어 있지 않음");
        }
    }

    private void Update()
    {
        // 필수! 메인스레드에서 이벤트 처리
        websocket?.DispatchMessageQueue();
    }

    private async void OnApplicationQuit()
    {
        await websocket?.Close();
    }
}