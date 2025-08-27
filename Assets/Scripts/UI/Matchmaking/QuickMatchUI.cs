using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuickMatchUI : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private Button cancelButton;

    private float _elapsed;
    private bool _running;

    private MatchmakingClient _mm;

    private void Awake()
    {
        _mm = MatchmakingClient.Instance ?? FindObjectOfType<MatchmakingClient>(true);
        if (_mm == null)
        {
            Debug.LogError("[QuickMatchUI] MatchmakingClient가 없습니다.");
            enabled = false;
            return;
        }

        _mm.OnSearchingStarted   += OnSearchingStarted;
        _mm.OnSearchingCancelled += OnSearchingCancelled;
        _mm.OnMatchFound         += OnMatchFound;
    }

    private void OnDestroy()
    {
        if (_mm != null)
        {
            _mm.OnSearchingStarted   -= OnSearchingStarted;
            _mm.OnSearchingCancelled -= OnSearchingCancelled;
            _mm.OnMatchFound         -= OnMatchFound;
        }
    }

    private void Start()
    {
        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(() =>
            {
                _mm.SendCancelQuickMatch();
                Hide();
            });
        }
        else
        {
            Debug.LogError("[QuickMatchUI] cancelButton 미할당");
        }
    }

    private void Update()
    {
        if (!_running || panel == null || !panel.activeSelf) return;

        _elapsed += Time.deltaTime;
        if (timerText != null) timerText.text = FormatElapsed(_elapsed);
    }

    // 이벤트 핸들러
    private void OnSearchingStarted()
    {
        _elapsed = 0f;
        _running = true;
        if (timerText != null) timerText.text = "00:00";
        Show();

        if (title != null && string.IsNullOrEmpty(title.text))
            title.text = "게임 찾는 중";
    }

    private void OnSearchingCancelled() => Hide();

    private void OnMatchFound(MatchmakingClient.MatchFoundPayload _)
    {
        Hide();
    }

    public void Show()
    {
        if (panel != null) panel.SetActive(true);
        if (!_running)
        {
            _elapsed = 0f;
            _running = true;
            if (timerText != null) timerText.text = "00:00";
        }
    }

    public void Hide()
    {
        _running = false;
        if (panel != null) panel.SetActive(false);
    }

    private string FormatElapsed(float seconds)
    {
        int total = Mathf.FloorToInt(Mathf.Max(0f, seconds));
        int mm = total / 60;
        int ss = total % 60;
        return $"{mm:00}:{ss:00}";
    }
}
