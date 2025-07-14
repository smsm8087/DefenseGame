using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PartyMemberIcon : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image hpBar;
    [SerializeField] private Image hpBg;
    [SerializeField] private Image ultBar;
    [SerializeField] private Image ultBg;
    [SerializeField] private Image playerIcon;
    [SerializeField] private Image mask;
    
    [Header("Revival System UI")]
    [SerializeField] private GameObject deadIndicator;      // 죽음 표시 오브젝트
    [SerializeField] private GameObject revivingIndicator;  // 부활 중 표시 오브젝트
    [SerializeField] private GameObject invulnerableIndicator; // 무적 표시 오브젝트
    [SerializeField] private TextMeshProUGUI statusText;    // 상태 텍스트
    [SerializeField] private TextMeshProUGUI reviverText;   // 부활시키는 사람 텍스트

    [Header("Visual Effects")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color deadColor = Color.gray;
    [SerializeField] private Color revivingColor = Color.yellow;
    [SerializeField] private Color invulnerableColor = Color.cyan;

    private string playerId;
    private string jobType;
    private float currentHealth;
    private float maxHealth;
    private float currentUlt;
    private float maxUlt;
    private bool isDead = false;
    private bool isBeingRevived = false;
    private bool isInvulnerable = false;
    private Vector2 deathPosition;
    private Coroutine blinkCoroutine;

    private void Awake()
    {
        Debug.Log($"PartyMemberIcon Awake 시작: {gameObject.name}");
        
        // 프리팹 구조에 맞춰 컴포넌트 자동 찾기
        if (hpBar == null)
        {
            Transform hpTransform = transform.Find("Canvas (Environment)/member/hp/hp");
            if (hpTransform != null)
            {
                hpBar = hpTransform.GetComponent<Image>();
                Debug.Log("HP Bar 찾음!");
            }
            else
            {
                Debug.LogError("HP Bar 못 찾음! 경로 확인 필요");
            }
        }

        if (hpBg == null)
        {
            Transform hpBgTransform = transform.Find("Canvas (Environment)/member/hp/hpBg");
            if (hpBgTransform != null)
            {
                hpBg = hpBgTransform.GetComponent<Image>();
                Debug.Log("HP Bg 찾음!");
            }
            else
            {
                Debug.LogError("HP Bg 못 찾음!");
            }
        }

        if (ultBar == null)
        {
            Transform ultTransform = transform.Find("Canvas (Environment)/member/ult/ult");
            if (ultTransform != null)
            {
                ultBar = ultTransform.GetComponent<Image>();
                Debug.Log("ULT Bar 찾음!");
            }
            else
            {
                Debug.LogError("ULT Bar 못 찾음! 경로 확인 필요");
            }
        }

        if (ultBg == null)
        {
            Transform ultBgTransform = transform.Find("Canvas (Environment)/member/ult/ultBg");
            if (ultBgTransform != null)
            {
                ultBg = ultBgTransform.GetComponent<Image>();
                Debug.Log("ULT Bg 찾음!");
            }
            else
            {
                Debug.LogError("ULT Bg 못 찾음!");
            }
        }

        if (playerIcon == null)
        {
            Transform iconTransform = transform.Find("Canvas (Environment)/member/IconBg/playerImg");
            if (iconTransform != null)
            {
                playerIcon = iconTransform.GetComponent<Image>();
                Debug.Log("Player Icon 찾음!");
            }
            else
            {
                Debug.LogError("Player Icon 못 찾음!");
            }
        }

        if (mask == null)
        {
            Transform maskTransform = transform.Find("Canvas (Environment)/member/IconBg/mask");
            if (maskTransform != null)
            {
                mask = maskTransform.GetComponent<Image>();
                Debug.Log("Mask 찾음!");
            }
            else
            {
                Debug.LogError("Mask 못 찾음!");
            }
        }
    }

    public void Initialize(string id, string job, Sprite icon = null)
    {
        playerId = id;
        jobType = job;

        if (playerIcon == null) return;

        // 항상 jobType 기반으로 로딩
        if (!string.IsNullOrEmpty(jobType))
        {
            string capitalJob = FirstCharToUpper(jobType);
            string spritePath = $"Character/{capitalJob}/PROFILE_{capitalJob}";

            Sprite overrideSprite = Resources.Load<Sprite>(spritePath);
            Debug.Log($"[PartyMemberIcon] Try load sprite: {spritePath} => {(overrideSprite != null ? "Success" : "Fail")}");
            if (overrideSprite != null)
            {
                icon = overrideSprite;
            }
            else
            {
                Debug.LogWarning($"[PartyMemberIcon] Sprite '{spritePath}'을(를) Resources에서 찾지 못함");
                icon = null; 
            }
        }
        else if (icon == null)
        {
            Debug.LogWarning("playerSprite도 없고 jobType도 없음");
            icon = null;
        }

        if (icon != null)
        {
            playerIcon.sprite = icon;
            // playerIcon.rectTransform.sizeDelta = new Vector2(100, 100);
            //
            // float scaleX = 1.25f;
            // float scaleY = 1.1f;
            //
            // // switch (playerIcon.sprite.name)
            // // {
            // //     case "PROFILE_Tank_0":
            // //         scaleX = 1.126f;
            // //         scaleY = 1.111f;
            // //         scaleZ = 0.06f; 
            // //         break;
            // //     case "PROFILE_Programmer_0":
            // //         scaleX = 1.2f;
            // //         scaleY = 1.3f;
            // //         scaleZ = 0.06f;
            // //         break;
            // //     case "PROFILE_Sniper_0":
            // //         scaleX = 1.0f;
            // //         scaleY = 1.0f;
            // //         scaleZ = 0.06f;
            // //         break;
            // //     default:
            // //         scaleX = 1.0f;
            // //         scaleY = 1.0f;
            // //         scaleZ = 0.06f;
            // //         break;
            // // }
            //
            // playerIcon.rectTransform.anchoredPosition = new Vector2(1.6f, -19.9f);
            // playerIcon.rectTransform.localScale = new Vector3(scaleX, scaleY, 1);
        }

        UpdateHealth(100f, 100f);
        UpdateUlt(0f, 100f);
        SetDeadState(false);
        SetRevivingState(false);
        SetInvulnerableState(false);
    }
    
    private string FirstCharToUpper(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }


    public void UpdateHealth(float current, float max)
    {
        currentHealth = current;
        maxHealth = max;

        Debug.Log($"UpdateHealth 호출: {current}/{max}");

        if (hpBar != null)
        {
            float healthPercent = maxHealth > 0 ? currentHealth / maxHealth : 0f;
            hpBar.fillAmount = healthPercent;
            Debug.Log($"HP Bar fillAmount 설정: {healthPercent}");
        }
        else
        {
            Debug.LogError("hpBar가 null입니다!");
        }
    }

    public void UpdateUlt(float current, float max)
    {
        currentUlt = current;
        maxUlt = max;

        Debug.Log($"UpdateUlt 호출: {current}/{max}");

        if (ultBar != null)
        {
            float ultPercent = maxUlt > 0 ? currentUlt / maxUlt : 0f;
            ultBar.fillAmount = ultPercent;
            Debug.Log($"ULT Bar fillAmount 설정: {ultPercent}");
        }
        else
        {
            Debug.LogError("ultBar가 null입니다!");
        }
    }

    public void SetStatus(string status)
    {
        switch (status)
        {
            case "dead":
                SetDeadState(true);
                break;
            case "being_revived":
                SetRevivingState(true);
                break;
            case "invulnerable":
                SetInvulnerableState(true);
                break;
            case "normal":
            default:
                SetDeadState(false);
                SetRevivingState(false);
                SetInvulnerableState(false);
                break;
        }
    }
    
    /// <summary>
    /// 죽음 상태 설정
    /// </summary>
    public void SetDeadState(bool dead)
    {
        isDead = dead;
        
        if (deadIndicator != null)
        {
            deadIndicator.SetActive(dead);
        }

        // 배경색 변경 (hpBg 또는 다른 UI 요소 사용)
        if (hpBg != null)
        {
            hpBg.color = dead ? deadColor : normalColor;
        }

        // 죽었을 때 다른 상태들 해제
        if (dead)
        {
            SetRevivingState(false);
            SetInvulnerableState(false);
        }

        UpdateVisualState();
    }

    /// <summary>
    /// 부활 중 상태 설정
    /// </summary>
    public void SetRevivingState(bool reviving)
    {
        isBeingRevived = reviving;
        
        if (revivingIndicator != null)
        {
            revivingIndicator.SetActive(reviving);
        }

        if (reviving)
        {
            // 부활 중일 때 깜빡임 효과
            StartBlinking(revivingColor);
        }
        else
        {
            StopBlinking();
            if (reviverText != null)
            {
                reviverText.text = "";
            }
        }

        UpdateVisualState();
    }

    /// <summary>
    /// 무적 상태 설정
    /// </summary>
    public void SetInvulnerableState(bool invulnerable)
    {
        isInvulnerable = invulnerable;
        
        if (invulnerableIndicator != null)
        {
            invulnerableIndicator.SetActive(invulnerable);
        }

        if (invulnerable)
        {
            // 무적 상태일 때 깜빡임 효과
            StartBlinking(invulnerableColor);
        }
        else if (!isBeingRevived)
        {
            StopBlinking();
        }

        UpdateVisualState();
    }

    /// <summary>
    /// 부활시키는 사람 정보 설정
    /// </summary>
    public void SetReviverInfo(string reviverPlayerId)
    {
        if (reviverText != null)
        {
            string reviverJob = GetPlayerJobType(reviverPlayerId);
            reviverText.text = $"{reviverJob}이 부활 중...";
        }
        else
        {
            // reviverText가 없으면 상태 텍스트 사용
            if (statusText != null)
            {
                string reviverJob = GetPlayerJobType(reviverPlayerId);
                statusText.text = $"{reviverJob}이 부활 중...";
            }
        }
    }

    /// <summary>
    /// 죽은 위치 설정
    /// </summary>
    public void SetDeathPosition(float x, float y)
    {
        deathPosition = new Vector2(x, y);
    }

    /// <summary>
    /// 시각적 상태 업데이트
    /// </summary>
    private void UpdateVisualState()
    {
        // hpBg를 배경으로 사용해서 색상 변경
        if (hpBg == null) return;

        Color targetColor = normalColor;

        if (isDead)
        {
            targetColor = deadColor;
        }
        else if (isBeingRevived)
        {
            targetColor = revivingColor;
        }
        else if (isInvulnerable)
        {
            targetColor = invulnerableColor;
        }

        if (blinkCoroutine == null)
        {
            hpBg.color = targetColor;
        }
    }

    /// <summary>
    /// 깜빡임 효과 시작
    /// </summary>
    private void StartBlinking(Color blinkColor)
    {
        StopBlinking();
        blinkCoroutine = StartCoroutine(BlinkCoroutine(blinkColor));
    }

    /// <summary>
    /// 깜빡임 효과 중지
    /// </summary>
    private void StopBlinking()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }
    }

    /// <summary>
    /// 깜빡임 코루틴
    /// </summary>
    private IEnumerator BlinkCoroutine(Color blinkColor)
    {
        while (true)
        {
            // 깜빡임 색상으로 변경
            if (hpBg != null)
            {
                hpBg.color = blinkColor;
            }
            yield return new WaitForSeconds(0.5f);

            // 원래 색상으로 변경
            if (hpBg != null)
            {
                Color baseColor = isDead ? deadColor : normalColor;
                hpBg.color = baseColor;
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// 플레이어 직업 타입 가져오기
    /// </summary>
    private string GetPlayerJobType(string playerId)
    {
        var players = NetworkManager.Instance.GetPlayers();
        if (players.TryGetValue(playerId, out GameObject playerObj))
        {
            BasePlayer player = playerObj.GetComponent<BasePlayer>();
            if (player != null)
            {
                return player.job_type;
            }
        }
        return "플레이어";
    }

    public string GetPlayerId() => playerId;
    public string GetJobType() => jobType;
    
    private void OnDestroy()
    {
        StopBlinking();
    }
}