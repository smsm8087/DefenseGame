using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataModels;
using NativeWebSocket.Models;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectSceneManager : MonoBehaviour
{
    public static CharacterSelectSceneManager Instance  { get; private set; }
    [SerializeField] private Button OutButton;
	[SerializeField] private Button ChattingButton;
    [SerializeField] private Button ChattingSendButton;
    [SerializeField] private GameObject ChattingObj;
    [SerializeField] private TMP_InputField ChattingInput;
    [SerializeField] private Transform ChattingParent;
    [SerializeField] private Transform PlayerIconParent;
    [SerializeField] private GameObject PlayerIconPrefab;
    
    private bool ui_lock = false;
    private Dictionary<string, PlayerIcon> players = new Dictionary<string, PlayerIcon>();
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        OutButton.onClick.AddListener(OnClickOut);
        ChattingButton.onClick.AddListener(OnClickChatting);
        ChattingSendButton.onClick.AddListener(OnClickChattingSend);
        WebSocketClient.Instance.OnMessageReceived += Handle;
    }
    private void Start()
    {
        //room info 가져오기
        var message = new
        {
            type = "get_room_info",
            playerId = UserSession.UserId,
            roomCode = RoomSession.RoomCode,
        };
        string json = JsonConvert.SerializeObject(message);
        WebSocketClient.Instance.Send(json);
    }

    void DeleteHandler()
    {
        WebSocketClient.Instance.OnMessageReceived -=  Handle;
    }
    void Handle(string message)
    {
        NetMsg netMsg = JsonConvert.DeserializeObject<NetMsg>(message);
        switch (netMsg.type)
        {
            case "started_game":
            {
                var handler = new StartGameHandler(DeleteHandler);
                handler.Handle(netMsg);
            }
            break;
            case "room_info":
            {
                var handler = new RoomInfoHandler(SetUpPlayerIcon);
                handler.Handle(netMsg);
            }
            break;
            case "out_room":
            {
                var handler = new OutRoomHandler();
                handler.Handle(netMsg);
            }
            break;
            case "chat_room":
            {
                var handler = new ChatRoomHandler();
                handler.Handle(netMsg);
            } 
            break;
        }
    }

    //입장시에 기본 플레이어 아이콘 생성
    private void SetUpPlayerIcon()
    {
        for (int i = 0; i < RoomSession.RoomInfos.Count; i++)
        {
            if (players.ContainsKey(RoomSession.RoomInfos[i].playerId)) continue;
            GameObject playerIconObj = Instantiate(PlayerIconPrefab, PlayerIconParent);
            PlayerIcon icon = playerIconObj.GetComponent<PlayerIcon>();
            if (icon != null)
            {
                icon.SetInfo(RoomSession.RoomInfos[i].playerId, RoomSession.RoomInfos[i].nickName);
                players[RoomSession.RoomInfos[i].playerId] = icon;
            }
        }

        if (RoomSession.RoomInfos.Count != players.Count)
        {
            // 삭제된 유저 정리
            foreach (var playerId in players.Keys.ToList())
            {
                if (!RoomSession.RoomInfos.Any(x => x.playerId == playerId))
                {
                    Destroy(players[playerId].gameObject);
                    players.Remove(playerId);
                }
            }
        }
    }

    public void UpdatePlayerIcon(string playerId, PlayerData data)
    {
        if (players.TryGetValue(playerId, out PlayerIcon playerIcon))
        {
            playerIcon.SetJobIcon(data.job_type);
        }
    }
    private void OnClickChatting()
    {
        ChattingObj.SetActive(!ChattingObj.activeSelf);
        ChattingInput.text = null;
    }
    
    private void OnClickChattingSend()
    {
        ChattingInput.text = ChattingInput.text.Trim();
        var message = new
        {
            type = "chat_room",
            playerId = UserSession.UserId,
            roomCode = RoomSession.RoomCode,
            message = ChattingInput.text
        };
        string json = JsonConvert.SerializeObject(message);
        WebSocketClient.Instance.Send(json);
        
        ChattingInput.text = null;
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
    private void OnClickOut()
    {
        StartCoroutine(TryOutRoom());
    }
    IEnumerator TryOutRoom()
    {
        if (ui_lock) yield break;
        ui_lock = true;
        var data = new Dictionary<string, string>
        {
            { "userId", UserSession.UserId },
            { "roomcode", RoomSession.RoomCode},
        };

        yield return ApiManager.Instance.Post(
            "room/out",
            data,
            onSuccess: (res) =>
            {
                var roomStatusResponse = JsonUtility.FromJson<ApiResponse.RoomOutResponse>(res);
                var message = new
                {
                    type = "out_room",
                    playerId = UserSession.UserId,
                    roomCode = RoomSession.RoomCode,
                    hostId = roomStatusResponse.hostId ?? ""
                };
                string json = JsonConvert.SerializeObject(message);
                WebSocketClient.Instance.Send(json);
            },
            onError: (err) =>
            {
                Debug.Log($"방 나가기 실패: {err}");
            }
        );
        ui_lock = false;
    }
}
