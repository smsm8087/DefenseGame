using System;
using System.Collections;
using System.Collections.Generic;
using NativeWebSocket.Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectSceneManager : MonoBehaviour
{
    [SerializeField] private Button StartButton;
    private bool ui_lock = false;
    private int playerCount;
    void Awake()
    {
        StartButton.onClick.AddListener(OnClickStart);
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
        bool isSuccess = false;
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
                    playerCount = roomStatusResponse.playerCount,
                    roomCode = RoomSession.RoomCode,
                };
                NetworkManager.Instance.SendMsg(message);
            },
            onError: (err) =>
            {
                Debug.Log($"방 상태확인 실패: {err}");
            }
        );
        ui_lock = false;
    }
}
