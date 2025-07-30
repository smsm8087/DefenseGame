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
    [SerializeField] private Button SelectButton;
    [SerializeField] private Button DeSelectButton;
	[SerializeField] private Button ChattingButton;
    [SerializeField] private Button ChattingSendButton;
    [SerializeField] private GameObject ChattingObj;
    [SerializeField] private TMP_InputField ChattingInput;
    [SerializeField] private Transform ChattingParent;
    [SerializeField] private Transform PlayerIconParent;
    [SerializeField] private GameObject PlayerIconPrefab;
    [SerializeField] private GameObject AllReadyObject;
    [SerializeField] private Transform AllReadyStartPos;
    [SerializeField] private Transform AllReadyDestPos;
    
    private bool ui_lock = false;
    private Dictionary<string, PlayerIcon> players = new Dictionary<string, PlayerIcon>();
    private Coroutine moveCoroutine;
    private Vector3 startPos;
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DeSelectButton.onClick.AddListener(OnclickDeSelect);
        SelectButton.onClick.AddListener(OnclickSelect);
        OutButton.onClick.AddListener(OnClickOut);
        ChattingButton.onClick.AddListener(OnClickChatting);
        ChattingSendButton.onClick.AddListener(OnClickChattingSend);
        WebSocketClient.Instance.OnMessageReceived += Handle;
        startPos = AllReadyStartPos.localPosition;
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
            case "selected_character":
            {
                var handler = new SelectedCharacterHandler();
                handler.Handle(netMsg);
            } 
            break;
            case "deselected_character":
            {
                var handler = new DeSelectedCharacterHandler();
                handler.Handle(netMsg);
            } 
            break;
        }
    }

    public void setUiLock(bool locked)
    {
        ui_lock = locked;
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
    public void UpdatePlayerIcon(string playerId, string job_type)
    {
        if (players.TryGetValue(playerId, out PlayerIcon playerIcon))
        {
            playerIcon.SetJobIcon(job_type);
        }
    }

    public void StopMoveReadyTextCoroutine()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            AllReadyObject.SetActive(false);
            AllReadyStartPos.localPosition = startPos;
        }
    }
    public void MoveReadyText()
    {
        moveCoroutine = StartCoroutine(MoveReadyTextCoroutine());
    }
    public IEnumerator MoveReadyTextCoroutine()
    {
        AllReadyObject.SetActive(true);
        float duration = 3f;
        float time = 0f;

        Vector3 startPos = AllReadyStartPos.localPosition;
        Vector3 to = AllReadyDestPos.localPosition;

        while (time < duration)
        {
            time += Time.deltaTime;
            AllReadyStartPos.localPosition = Vector3.Lerp(startPos, to, time / duration);
            yield return null;
        }

        AllReadyStartPos.localPosition = to;
        AllReadyObject.SetActive(false);
        if (RoomSession.HostId == UserSession.UserId)
        {
            yield return StartCoroutine(TryGameStartCoroutine());
        }
    }
    public void SetReady(string playerId, bool isReady)
    {
        if (players.TryGetValue(playerId, out PlayerIcon playerIcon))
        {
            playerIcon.SetReady(isReady);
        }
    }
    private void OnClickChatting()
    {
        ChattingObj.SetActive(!ChattingObj.activeSelf);
        ChattingInput.text = null;
    }
    
    private void OnClickChattingSend()
    {
        if (ui_lock) return;
        ui_lock = true;
        ChattingInput.text = ChattingInput.text.Trim();
        if (ChattingInput.text != null & ChattingInput.text.Length > 0)
        {
            var message = new
            {
                type = "chat_room",
                playerId = UserSession.UserId,
                nickName = UserSession.Nickname,
                roomCode = RoomSession.RoomCode,
                message = ChattingInput.text
            };
            string json = JsonConvert.SerializeObject(message);
            WebSocketClient.Instance.Send(json);
        
            ChattingInput.text = null;
        }
        else
        {
            // 메시지를 스페이스바 혹은 공백으로 입력한 경우
            ChattingInput.text = null;
        }
    }

    private void OnclickDeSelect()
    {
        if (ui_lock) return;
        ui_lock = true;
        var message = new
        {
            type = "deselect_character",
            playerId = UserSession.UserId,
            roomCode = RoomSession.RoomCode,
        };
        string json = JsonConvert.SerializeObject(message);
        WebSocketClient.Instance.Send(json);
        DeSelectButton.gameObject.SetActive(false);
        SelectButton.gameObject.SetActive(true);
    }
    
    private void OnclickSelect()
    {
        if (ui_lock) return;
        ui_lock = true;
        if (players.TryGetValue(UserSession.UserId, out PlayerIcon playerIcon))
        {
            var message = new
            {
                type = "select_character",
                playerId = UserSession.UserId,
                roomCode = RoomSession.RoomCode,
                jobType = playerIcon.job_type,
            };
            string json = JsonConvert.SerializeObject(message);
            WebSocketClient.Instance.Send(json);
            DeSelectButton.gameObject.SetActive(true);
            SelectButton.gameObject.SetActive(false);
        }
    }
    
    IEnumerator TryGameStartCoroutine()
    {
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
        if(ui_lock) return;
        ui_lock = true;
        StartCoroutine(TryOutRoom());
    }
    IEnumerator TryOutRoom()
    {
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
                string hostId = roomStatusResponse != null ?  roomStatusResponse.hostId : null;
                var message = new
                {
                    type = "out_room",
                    playerId = UserSession.UserId,
                    roomCode = RoomSession.RoomCode,
                    hostId = hostId ?? ""
                };
                string json = JsonConvert.SerializeObject(message);
                WebSocketClient.Instance.Send(json);
            },
            onError: (err) =>
            {
                Debug.Log($"방 나가기 실패: {err}");
            }
        );
    }
}
