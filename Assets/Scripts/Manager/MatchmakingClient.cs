using System;
using Newtonsoft.Json;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class MatchmakingClient : MonoBehaviour
{
    public static MatchmakingClient Instance { get; private set; }

    public event Action OnSearchingStarted;
    public event Action OnSearchingCancelled;
    public event Action<MatchFoundPayload> OnMatchFound;
    public event Action<MatchStatusPayload> OnMatchStatus;
    public event Action<string> OnMatchCancelled;
    public event Action<MatchStartPayload> OnMatchStarted;

    private MatchFoundPayload _pendingFound;

    [Serializable] private class Head { public string type; }

    [Serializable] public class MatchFoundPayload { public string type; public string matchId; public int needPlayers; public int acceptedCount; public int secondsToExpire; }
    [Serializable] public class MatchStatusPayload { public string type; public string matchId; public int acceptedCount; public int totalPlayers; public int remainSeconds; }
    [Serializable] public class MatchCancelledPayload { public string type; public string reason; }
    [Serializable] public class MatchStartPayload { public string type; public string roomCode; public bool isMatchmakingRoom; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        WebSocketClient.Instance.OnMessageReceived += HandleMessage;
        Debug.Log("[MMClient] Awake & subscribed.");
    }

    private void OnDestroy()
    {
        if (WebSocketClient.Instance != null)
            WebSocketClient.Instance.OnMessageReceived -= HandleMessage;
    }

    public void SendJoinQuickMatch()
    {
        var msg = new { type = "quickmatch_join", playerId = UserSession.UserId, nickName = UserSession.Nickname };
        WebSocketClient.Instance.Send(JsonConvert.SerializeObject(msg));
        Debug.Log("[MMClient] -> quickmatch_join");
        OnSearchingStarted?.Invoke();
    }
    public void SendCancelQuickMatch()
    {
        var msg = new { type = "quickmatch_cancel", playerId = UserSession.UserId };
        WebSocketClient.Instance.Send(JsonConvert.SerializeObject(msg));
        Debug.Log("[MMClient] -> quickmatch_cancel");
        OnSearchingCancelled?.Invoke();
    }
    public void SendAccept(string matchId)
    {
        var msg = new { type = "match_accept", playerId = UserSession.UserId, matchId };
        WebSocketClient.Instance.Send(JsonConvert.SerializeObject(msg));
        Debug.Log($"[MMClient] -> match_accept ({matchId})");
    }
    public void SendDecline(string matchId)
    {
        var msg = new { type = "match_decline", playerId = UserSession.UserId, matchId };
        WebSocketClient.Instance.Send(JsonConvert.SerializeObject(msg));
        Debug.Log($"[MMClient] -> match_decline ({matchId})");
    }

    public MatchFoundPayload GetPendingMatchFound() => _pendingFound;

    private void HandleMessage(string json)
    {
        try
        {
            var head = JsonConvert.DeserializeObject<Head>(json);
            switch (head.type)
            {
                case "match_found":
                {
                    var p = JsonConvert.DeserializeObject<MatchFoundPayload>(json);
                    _pendingFound = p;
                    Debug.Log($"[MMClient] <= match_found (matchId={p.matchId})");

                    OnMatchFound?.Invoke(p);
                    StartCoroutine(EnsureMatchFoundUIShown(p));
                }
                break;

                case "match_status":
                {
                    var p = JsonConvert.DeserializeObject<MatchStatusPayload>(json);
                    Debug.Log($"[MMClient] <= match_status ({p.acceptedCount}/{p.totalPlayers})");
                    OnMatchStatus?.Invoke(p);
                }
                break;

                case "match_cancelled":
                {
                    var p = JsonConvert.DeserializeObject<MatchCancelledPayload>(json);
                    _pendingFound = null;
                    Debug.Log("[MMClient] <= match_cancelled");
                    OnMatchCancelled?.Invoke(p.reason);
                }
                break;

                case "match_start":
                {
                    var p = JsonConvert.DeserializeObject<MatchStartPayload>(json);
                    _pendingFound = null;
                    Debug.Log($"[MMClient] <= match_start (room={p.roomCode})");
                    OnMatchStarted?.Invoke(p);
                }
                break;
            }
        }
        catch (Exception e) { Debug.LogWarning($"[MatchmakingClient] parse fail: {e.Message}"); }
    }

    // UI 강제 표시 코루틴
    private System.Collections.IEnumerator EnsureMatchFoundUIShown(MatchFoundPayload payload)
    {
        // 패널들이 Awake/생성될 시간 확보
        yield return null;

        var ui = GameObject.FindObjectOfType<MatchFoundUI>(true);
        if (ui != null)
        {
            Debug.Log("[MMClient] Ensure: found MatchFoundUI in scene. ForceOpen.");
            ui.ForceOpen(payload);
            yield break;
        }

        // 씬에 없으면 Resources에서 프리팹 로드
        Debug.LogWarning("[MMClient] Ensure: MatchFoundUI not found. Trying to load prefab Resources/UI/MatchFoundPanel");
        var prefab = Resources.Load<GameObject>("UI/MatchFoundPanel");
        if (prefab != null)
        {
            var go = Instantiate(prefab);
            var comp = go.GetComponent<MatchFoundUI>();
            if (comp != null)
            {
                Debug.Log("[MMClient] Ensure: instantiated prefab and ForceOpen.");
                comp.ForceOpen(payload);
                yield break;
            }
        }
        Debug.LogError("[MMClient] Ensure failed: MatchFoundUI not found and prefab missing at Resources/UI/MatchFoundPanel");
    }
}
