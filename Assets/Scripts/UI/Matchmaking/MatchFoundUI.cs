using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DefaultExecutionOrder(-50)]
public class MatchFoundUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Image circleFill;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button declineButton;

    [Header("Ready Icons")]
    [SerializeField] private Image[] ReadyIcons;
    [SerializeField] private Sprite IconIdle;
    [SerializeField] private Sprite IconReady;

    private string matchId;
    private int total;
    private MatchmakingClient _mm;

    private void Awake()
    {
        _mm = MatchmakingClient.Instance ?? FindObjectOfType<MatchmakingClient>(true);
        if (_mm == null) { Debug.LogError("[MatchFoundUI] MatchmakingClient 없음"); enabled = false; return; }

        if (panel == null) panel = gameObject;

        if (acceptButton != null)
        {
            acceptButton.onClick.RemoveAllListeners();
            acceptButton.onClick.AddListener(() =>
            {
                if (!string.IsNullOrEmpty(matchId))
                {
                    // 수락 클릭 즉시 양쪽 버튼 잠금
                    acceptButton.interactable = false;
                    if (declineButton) declineButton.interactable = false;

                    _mm.SendAccept(matchId);
                    Debug.Log($"[MatchFoundUI] Click Accept ({matchId})");
                }
            });
        }
        if (declineButton != null)
        {
            declineButton.onClick.RemoveAllListeners();
            declineButton.onClick.AddListener(() =>
            {
                if (!string.IsNullOrEmpty(matchId)) _mm.SendDecline(matchId);
                Debug.Log("[MatchFoundUI] Click Decline");
                Close();
            });
        }

        _mm.OnMatchFound     += OnMatchFound;
        _mm.OnMatchStatus    += OnMatchStatus;
        _mm.OnMatchCancelled += OnMatchCancelled;
        _mm.OnMatchStarted   += OnMatchStart;
    }

    private void OnDestroy()
    {
        if (_mm != null)
        {
            _mm.OnMatchFound     -= OnMatchFound;
            _mm.OnMatchStatus    -= OnMatchStatus;
            _mm.OnMatchCancelled -= OnMatchCancelled;
            _mm.OnMatchStarted   -= OnMatchStart;
        }
    }

    public void ForceOpen(MatchmakingClient.MatchFoundPayload p)
    {
        OnMatchFound(p);
    }

    private void OnMatchFound(MatchmakingClient.MatchFoundPayload p)
    {
        matchId = p.matchId;
        total   = p.needPlayers;

        if (panel != null)
        {
            panel.SetActive(true);
            panel.transform.SetAsLastSibling();

            var c = panel.GetComponent<Canvas>();
            if (c == null) c = panel.AddComponent<Canvas>();
            c.overrideSorting = true;
            c.sortingOrder = 100;

            if (panel.GetComponent<GraphicRaycaster>() == null)
                panel.AddComponent<GraphicRaycaster>();
        }

        if (statusText != null) statusText.text = $"파티원들 대기중... (0/{total})";

        // 버튼 초기화
        if (acceptButton) acceptButton.interactable = true;
        if (declineButton) declineButton.interactable = true;

        ResetIcons(total);

        StopAllCoroutines();
        StartCoroutine(CoTimer(p.secondsToExpire));
    }

    private void OnMatchStatus(MatchmakingClient.MatchStatusPayload p)
    {
        if (p.matchId != matchId) return;
        if (statusText != null) statusText.text = $"파티원들 대기중... ({p.acceptedCount}/{total})";

        // 수락 인원 수만큼 왼쪽부터 초록색으로
        UpdateIcons(p.acceptedCount, total);
    }

    private void OnMatchCancelled(string _)
    {
        Close();
    }

    private System.Collections.IEnumerator CoTimer(int sec)
    {
        float t = sec;
        while (t > 0f && panel != null && panel.activeSelf)
        {
            t -= Time.deltaTime;
            if (circleFill != null) circleFill.fillAmount = Mathf.Clamp01(t / sec);
            yield return null;
        }
    }

    private void OnMatchStart(MatchmakingClient.MatchStartPayload p)
    {
        RoomSession.Init();
        RoomSession.Set(p.roomCode, "");
        RoomSession.SetMatchmaking(p.isMatchmakingRoom);
        Close();
        SceneLoader.Instance.LoadScene("CharacterSelectScene", () =>
        {
            CharacterSelectSceneManager.Instance.Initialize();
        });
    }

    private void Close()
    {
        StopAllCoroutines();
        if (panel != null) panel.SetActive(false);
        if (circleFill != null) circleFill.fillAmount = 0f;
        matchId = null;

        // 패널 닫힐 때도 아이콘 초기화
        ResetIcons(total);

        // 버튼 원복
        if (acceptButton) acceptButton.interactable = true;
        if (declineButton) declineButton.interactable = true;
    }

    // 아이콘 유틸
    private void ResetIcons(int totalPlayers)
    {
        if (ReadyIcons == null) return;
        for (int i = 0; i < ReadyIcons.Length; i++)
        {
            var img = ReadyIcons[i];
            if (!img) continue;

            // 총 인원 수 초과 슬롯은 숨김
            img.gameObject.SetActive(i < totalPlayers);

            if (IconIdle) img.sprite = IconIdle;
        }
    }

    private void UpdateIcons(int acceptedCount, int totalPlayers)
    {
        if (ReadyIcons == null) return;

        for (int i = 0; i < ReadyIcons.Length; i++)
        {
            var img = ReadyIcons[i];
            if (!img) continue;

            bool active = (i < totalPlayers);
            img.gameObject.SetActive(active);
            if (!active) continue;

            if (i < acceptedCount)
            {
                if (IconReady) img.sprite = IconReady;
            }
            else
            {
                if (IconIdle) img.sprite = IconIdle;
            }
        }
    }
}
