using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using NativeWebSocket.Models;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class LobbySceneManager : MonoBehaviour
{
    public Button createRoomButton;
    public TMP_InputField roomCodeInput;
    public Button joinRoomButton;
    private bool ui_lock = false;

    private void Start()
    {
        createRoomButton.onClick.AddListener(() => StartCoroutine(CreateRoom()));
        joinRoomButton.onClick.AddListener(() => StartCoroutine(JoinRoom()));
        WebSocketClient.Instance.OnMessageReceived += Handle;
    }

    void Handle(string message)
    {
        NetMsg netMsg = JsonConvert.DeserializeObject<NetMsg>(message);
        switch (netMsg.type)
        {
            case "room_created":
            {
                var handler = new CreateRoomHandler();
                handler.Handle(netMsg);
                WebSocketClient.Instance.OnMessageReceived -=  Handle;
            }
            break;
            case "room_joined":
            {
                var handler = new JoinRoomHandler();
                handler.Handle(netMsg);
                WebSocketClient.Instance.OnMessageReceived -=  Handle;
            }
            break;
        }
    }

    IEnumerator CreateRoom()
    {
        if(ui_lock) yield break;
        ui_lock = true;
        var data = new Dictionary<string, string>
        {
            { "userId", UserSession.UserId }
        };

        yield return ApiManager.Instance.Post(
            "room/create",
            data,
            onSuccess: (res) =>
            {
                var parsed = JsonUtility.FromJson<ApiResponse.CreateRoomResponse>(res);
                RoomSession.Set(parsed.roomCode);
                Debug.Log($"방 생성 성공! 코드: {parsed.roomCode}");
                TryConnectAndEnterLobby(true);
            },
            onError: (err) =>
            {
                Debug.Log($"방 생성 실패: {err}");
            }
        );
        ui_lock = false;
    }
    void TryConnectAndEnterLobby(bool isCreateRoom = false)
    {
        if (isCreateRoom)
        {
            var message = new
            {
                type = "create_room",         // 첫 메시지 타입
                playerId = UserSession.UserId,
                roomCode = RoomSession.RoomCode
            };
            string json = JsonConvert.SerializeObject(message);
            WebSocketClient.Instance.Send(json);
        }
        else
        {
            var message = new
            {
                type = "join_room",         // 첫 메시지 타입
                playerId = UserSession.UserId,
                roomCode = RoomSession.RoomCode
            };
            string json = JsonConvert.SerializeObject(message);
            WebSocketClient.Instance.Send(json);
        }
    }
    IEnumerator JoinRoom()
    {
        if(ui_lock) yield break;
        ui_lock = true;
        var data = new Dictionary<string, string>
        {
            { "userId", UserSession.UserId.ToString() },
            { "roomCode", roomCodeInput.text.Trim().ToUpper() }
        };

        yield return ApiManager.Instance.Post(
            "room/join",
            data,
            onSuccess: (res) =>
            {
                var parsed = JsonUtility.FromJson<ApiResponse.JoinRoomResponse>(res);
                RoomSession.Set(parsed.roomCode);
                Debug.Log($"입장 성공! 코드: {parsed.roomCode}");
                TryConnectAndEnterLobby(false);
            },
            onError: (err) =>
            {
                Debug.Log($"입장 실패: {err}");
            }
        );
        ui_lock = false;
    }
}
