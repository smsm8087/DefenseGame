using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharacterSelect;
using DataModels;
using NativeWebSocket.Models;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectSceneManager : MonoBehaviour
{
    public static CharacterSelectSceneManager Instance  { get; private set; }
    [SerializeField] private CharacterSelectUI characterSelectUI;
    [SerializeField] private Button OutButton;
    [SerializeField] private Button SelectButton;
    [SerializeField] private Button DeSelectButton;
    [SerializeField] private Button ChattingButton;
    [SerializeField] private Button ChattingSendButton;
    [SerializeField] private GameObject ChattingObj;
    [SerializeField] private TMP_InputField ChattingInput;
    [SerializeField] private GameObject ChatUI;
    [SerializeField] private Transform ChattingParent;
    [SerializeField] private Transform PlayerIconParent;
    [SerializeField] private GameObject PlayerIconPrefab;
    [SerializeField] private GameObject StartObj;
    [SerializeField] private Button StartButton;

    [SerializeField] private TMP_Text startCountdownText;
    private Coroutine _autoStartCo;
    private float _limitSeconds = 60f;
    private float _allReadyDelay = 5f;

    private bool ui_lock = false;
    private Dictionary<string, PlayerIcon> players;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
    }

    public void Initialize()
    {
        SoundManager.Instance.PlayBGM("characterSelect");
        DeSelectButton.onClick.AddListener(OnclickDeSelect);
        SelectButton.onClick.AddListener(OnclickSelect);
        OutButton.onClick.AddListener(OnClickOut);
        ChattingButton.onClick.AddListener(OnClickChatting);
        ChattingSendButton.onClick.AddListener(OnClickChattingSend);
        StartButton.onClick.AddListener(() =>
        {
            if (ui_lock) return;
            ui_lock = true;
            TryGameStart();
        });

        players = new Dictionary<string, PlayerIcon>();
        WebSocketClient.Instance.OnMessageReceived += Handle;

        // room info 요청
        var message = new
        {
            type = "get_room_info",
            playerId = UserSession.UserId,
            roomCode = RoomSession.RoomCode,
        };
        string json = JsonConvert.SerializeObject(message);
        WebSocketClient.Instance.Send(json);

        if (RoomSession.IsMatchmakingRoom)
        {
            // 빠른매칭 → 시작 버튼 숨김, 카운트다운은 서버 이벤트로 표시
            if (StartObj != null) StartObj.SetActive(false);
            if (startCountdownText != null)
            {
                startCountdownText.gameObject.SetActive(true);
                startCountdownText.text = "시작 준비 중...";
            }
        }
        else
        {
            // 커스텀 → 방장만 시작 버튼 보임, 카운트다운 숨김
            if (StartObj != null) StartObj.SetActive(UserSession.UserId == RoomSession.HostId);
            if (startCountdownText != null)
            {
                startCountdownText.gameObject.SetActive(false);
                startCountdownText.text = string.Empty;
            }
        }
    }

    void OnDisable()
    {
        if (WebSocketClient.Instance != null)
            WebSocketClient.Instance.OnMessageReceived -= Handle;
    }

    void Handle(string message)
    {
        NetMsg netMsg = JsonConvert.DeserializeObject<NetMsg>(message);
        switch (netMsg.type)
        {
            case "confirm":
            {
                var handler = new ConfirmPopupHandler();
                handler.Handle(netMsg);
            }
            break;
            case "notice":
            {
                var handler = new NoticePopupHandler();
                handler.Handle(netMsg);
            }
            break;
            case "started_game":
            {
                var handler = new StartGameHandler(()=>
                {
                    StartCoroutine(TryGameStartCoroutine());
                });
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
                var handler = new OutRoomHandler(() => { players.Clear();});
                handler.Handle(netMsg);
            }
            break;
            case "chat_room":
            {
                var handler = new ChatRoomHandler(ChatUI, ChattingParent);
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

            case "select_timer":
            {
                try
                {
                    var root = JsonConvert.DeserializeObject<Dictionary<string, object>>(message);
                    if (root != null && root.TryGetValue("remainSeconds", out var v) && startCountdownText != null)
                    {
                        int sec = Convert.ToInt32(v);
                        startCountdownText.gameObject.SetActive(true);
                        startCountdownText.text = $"시작까지 {sec}초";
                    }
                }
                catch { /* ignore */ }
            }
            break;

            case "select_all_ready":
            {
                // 전원 준비됨. 서버가 5초 카운트 다운을 select_timer로 보내므로 별도 처리 필요 없음.
            }
            break;

            case "select_phase_end":
            {
                // 선택 단계 종료(60초 만료 or 5초 단축 종료)
                if (RoomSession.IsMatchmakingRoom)
                {
                    TryGameStart();
                }
            }
            break;
        }
        ui_lock = false;
    }

    // 입장시에 기본 플레이어 아이콘 생성
    private void SetUpPlayerIcon()
    {
        // 방 모드 구분
        if (RoomSession.IsMatchmakingRoom)
        {
            // 매칭 방 → 방장 개념 없음
            if (StartObj != null) StartObj.SetActive(false);
        }
        else
        {
            // 커스텀 방 → Host만 시작 버튼 노출
            if (StartObj != null) StartObj.SetActive(UserSession.UserId == RoomSession.HostId);
        }

        // 기존으로 이미 생성되어 있는 아이콘들의 Host표식/강퇴버튼 갱신
        foreach (var kv in players)
        {
            if (kv.Value != null) kv.Value.UpdateHostIcon();
        }

        // PlayerIcon 리스트 싱크
        if (RoomSession.RoomInfos.Count != players.Count)
        {
            foreach (var playerId in players.Keys.ToList())
            {
                if (!RoomSession.RoomInfos.Any(x => x.playerId == playerId))
                {
                    Destroy(players[playerId].gameObject);
                    players.Remove(playerId);
                }
            }
        }

        foreach (var info in RoomSession.RoomInfos)
        {
            if (players.ContainsKey(info.playerId)) continue;

            var go = Instantiate(PlayerIconPrefab, PlayerIconParent);
            var icon = go.GetComponent<PlayerIcon>();
            if (icon != null)
            {
                icon.SetInfo(info);
                players[info.playerId] = icon;
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
        ui_lock = false;
    }

    private void SetLockState(bool locked)
    {
        characterSelectUI.SetInteractable(!locked);
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
        ui_lock = false;
        SetLockState(false);
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

        ui_lock = false;
        SetLockState(true);
    }

    void TryGameStart()
    {
        var message = new
        {
            type = "start_game",
            playerId = UserSession.UserId,
            roomCode = RoomSession.RoomCode,
        };
        string json = JsonConvert.SerializeObject(message);
        WebSocketClient.Instance.Send(json);
    }

    IEnumerator TryGameStartCoroutine()
    {
        // 매칭 방이면 웹 API(/room/status) 호출하지 않음
        if (RoomSession.IsMatchmakingRoom)
        {
            ui_lock = false;
            yield break;
        }

        if (string.IsNullOrEmpty(RoomSession.RoomCode))
        {
            Debug.LogWarning("[CSM] RoomCode empty at TryGameStartCoroutine");
            ui_lock = false;
            yield break;
        }

        var data = new Dictionary<string, string>
        {
            { "roomcode", RoomSession.RoomCode},
        };

        yield return ApiManager.Instance.Post(
            "room/status",
            data,
            onSuccess: (res) =>
            {
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
        ui_lock = false;
    }

    // 미선택자 자동 배정
    private void AutoAssignRandomForUnselected()
    {
        var all = new List<string> { "tank", "programmer", "sniper", "maid" };
        foreach (var info in RoomSession.RoomInfos)
        {
            if (info.isReady && !string.IsNullOrEmpty(info.jobType)) all.Remove(info.jobType);
        }
        var rand = new System.Random();
        foreach (var info in RoomSession.RoomInfos)
        {
            if (info.isReady) continue;
            if (all.Count <= 0) break;
            int idx = rand.Next(all.Count);
            string pick = all[idx];
            all.RemoveAt(idx);

            if (info.playerId == UserSession.UserId)
            {
                var message = new
                {
                    type = "select_character",
                    playerId = UserSession.UserId,
                    roomCode = RoomSession.RoomCode,
                    jobType = pick,
                };
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(message);
                WebSocketClient.Instance.Send(json);
            }
        }
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
