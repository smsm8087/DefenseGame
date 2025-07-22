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
                RoomSession.Set(parsed.roomCode, parsed.hostId);
                Debug.Log($"방 생성 성공! 코드: {parsed.roomCode}");
                SceneLoader.Instance.LoadScene("CharacterSelectScene");
            },
            onError: (err) =>
            {
                Debug.Log($"방 생성 실패: {err}");
            }
        );
        ui_lock = false;
    }
    IEnumerator JoinRoom()
    {
        if(ui_lock) yield break;
        ui_lock = true;
        var data = new Dictionary<string, string>
        {
            { "userId", UserSession.UserId },
            { "roomCode", roomCodeInput.text.Trim().ToUpper() }
        };

        yield return ApiManager.Instance.Post(
            "room/join",
            data,
            onSuccess: (res) =>
            {
                var parsed = JsonUtility.FromJson<ApiResponse.JoinRoomResponse>(res);
                RoomSession.Set(parsed.roomCode, parsed.hostId);
                Debug.Log($"입장 성공! 코드: {parsed.roomCode}");
                SceneLoader.Instance.LoadScene("CharacterSelectScene");
            },
            onError: (err) =>
            {
                Debug.Log($"입장 실패: {err}");
            }
        );
        ui_lock = false;
    }
}
