using System.Collections;
using System.Collections.Generic;
using NativeWebSocket.Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectSceneManager : MonoBehaviour
{
    [SerializeField] private Button StartButton;
	[SerializeField] private Button ChattingButton;
	[SerializeField] private GameObject ChattingObj;
    
    private bool ui_lock = false;
    void Awake()
    {
        StartButton.onClick.AddListener(OnClickStart);
        ChattingButton.onClick.AddListener(OnClickChatting);
        WebSocketClient.Instance.OnMessageReceived += Handle;
    }

    void Handle(string message)
    {
        NetMsg netMsg = JsonConvert.DeserializeObject<NetMsg>(message);
        switch (netMsg.type)
        {
            case "started_game":
            {
                var handler = new StartGameHandler();
                handler.Handle(netMsg);
            }
            break;
        }
        WebSocketClient.Instance.OnMessageReceived -=  Handle;
    }

    private void OnClickChatting()
    {
        ChattingObj.SetActive(!ChattingObj.activeSelf);
    }

    private void OnClickStart()
    {
        //방장만 할수 있는 메서드.
        StartCoroutine(TryGameStart());
    }
    IEnumerator TryGameStart()
    {
        if (ui_lock) yield break;
        ui_lock = true;
        var data = new Dictionary<string, string>
        {
            { "roomcode", RoomSession.RoomCode},
        };

        yield return ApiManager.Instance.Post(
            "room/status",
            data,
            onSuccess: (res) =>
            {
                var roomStatusResponse = JsonUtility.FromJson<ApiResponse.RoomStatusResponse>(res);
                var message = new
                {
                    type = "start_game",
                    playerId = UserSession.UserId,
                    players = roomStatusResponse.playerIds,
                    roomCode = RoomSession.RoomCode,
                };
                string json = JsonConvert.SerializeObject(message);
                WebSocketClient.Instance.Send(json);
            },
            onError: (err) =>
            {
                Debug.Log($"방 상태확인 실패: {err}");
            }
        );
        ui_lock = false;
    }
}
