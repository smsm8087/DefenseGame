using UnityEngine;
using UnityEngine.UI;

public class PartyMemberIcon : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image hpBar;
    [SerializeField] private Image hpBg;
    [SerializeField] private Image ultBar;
    [SerializeField] private Image ultBg;
    [SerializeField] private Image playerIcon;
    [SerializeField] private Image mask;

    private string playerId;
    private string jobType;
    private float currentHealth;
    private float maxHealth;
    private float currentUlt;
    private float maxUlt;

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
            Transform iconTransform = transform.Find("Canvas (Environment)/member/IconBg/playering");
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

        // 아이콘이 없으면 jobType 기반으로 Resources에서 자동 로딩
        if (icon == null && !string.IsNullOrEmpty(jobType))
        {
            string capitalJob = FirstCharToUpper(jobType);
            string spritePath = $"Character/{capitalJob}/profile_{capitalJob}";
            icon = Resources.Load<Sprite>(spritePath);

            if (icon == null)
            {
                Debug.LogWarning($"[PartyMemberIcon] Sprite '{spritePath}' 을(를) Resources에서 찾을 수 없습니다.");
            }
        }

        if (icon != null && playerIcon != null)
        {
            playerIcon.sprite = icon;
            playerIcon.type = Image.Type.Simple;
            playerIcon.preserveAspect = false;
            playerIcon.rectTransform.sizeDelta = new Vector2(100, 100);

            float scaleX = 1f;
            float scaleY = 1f;
            float scaleZ = 1f;

            switch (icon.name)
            {
                case "profile_Tank":
                    scaleX = 1.126f;
                    scaleY = 1.111f;
                    scaleZ = 0.06f;
                    break;
                case "profile_Programmer":
                    scaleX = 1.2f;
                    scaleY = 1.3f;
                    scaleZ = 0.06f;
                    break;
                case "profile_Sniper":
                    scaleX = 1.0f;
                    scaleY = 1.0f;
                    scaleZ = 0.06f;
                    break;
                default:
                    scaleX = 1.0f;
                    scaleY = 1.0f;
                    scaleZ = 0.06f;
                    break;
            }

            playerIcon.rectTransform.anchoredPosition = new Vector2(1.6f, -19.9f);
            playerIcon.rectTransform.localScale = new Vector3(scaleX, scaleY, scaleZ);
        }

        UpdateHealth(100f, 100f);
        UpdateUlt(0f, 100f);
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
        // 특별한 상태 처리가 필요한 경우에만 사용
        switch (status)
        {
            case "dead":
                // 필요시 죽음 상태 처리
                break;
            case "normal":
            default:
                // 정상 상태로 복원
                break;
        }
    }

    public string GetPlayerId() => playerId;
    public string GetJobType() => jobType;
}