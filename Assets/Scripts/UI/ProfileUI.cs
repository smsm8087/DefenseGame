using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DataModels;
using System.Collections.Generic;
using System.Linq;

public class ProfileUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image iconBG;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Image hpBG;
    [SerializeField] private Slider ultSlider;
    [SerializeField] private Image ultBG;
    [SerializeField] private TextMeshProUGUI nicknameText;

    [Header("StatUI")]
    [SerializeField] private Button statUIButton;

    private float maxUlt = 100f;
    private PlayerInfo playerinfo;

    public void InitializeProfile(PlayerInfo playerinfo, GameObject player)
    {
        this.playerinfo = playerinfo;
        UpdateHp(playerinfo.currentHp, playerinfo.currentHp);
        UpdateUltGauge(0, maxUlt);

        var spriteRenderer = player.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            SetPlayerIcon(null, playerinfo.job_type);
        }

        statUIButton?.onClick.AddListener(() => OnShowStatPopup(this.playerinfo));
    }

    public void UpdatePlayerInfo(PlayerInfo playerinfo)
    {
        this.playerinfo = playerinfo;
    }

    public void OnShowStatPopup(PlayerInfo playerinfo)
    {
        UIManager.Instance.ShowStatPopup(playerinfo);
    }

    public void UpdateHp(int currentHp, int maxHp)
    {
        if (hpSlider == null)
        {
            Transform hpBarTransform = transform.Find("HPBAR");
            if (hpBarTransform != null)
            {
                hpSlider = hpBarTransform.GetComponentInChildren<Slider>();
            }
        }

        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHp;
            hpSlider.value = currentHp;
        }

        Transform hpTextTransform = transform.Find("HPBAR/hpBG/hp");
        if (hpTextTransform != null)
        {
            TextMeshProUGUI hpText = hpTextTransform.GetComponent<TextMeshProUGUI>();
            if (hpText != null)
            {
                hpText.text = $"{currentHp}/{maxHp}";
            }
        }
    }

    public void UpdateUltGauge(float currentUlt, float maxUlt)
    {
        if (ultSlider == null)
        {
            Transform ultBarTransform = transform.Find("ULTBAR");
            if (ultBarTransform != null)
            {
                ultSlider = ultBarTransform.GetComponentInChildren<Slider>();
            }
        }

        if (ultSlider != null)
        {
            ultSlider.maxValue = maxUlt;
            ultSlider.value = currentUlt;
        }
        else
        {
            Transform ultImageTransform = transform.Find("ULTBAR/ult");
            if (ultImageTransform != null)
            {
                Image ultImage = ultImageTransform.GetComponent<Image>();
                if (ultImage != null)
                {
                    ultImage.fillAmount = currentUlt / maxUlt;
                    ultImage.type = Image.Type.Filled;
                }
            }
        }

        Transform ultTextTransform = transform.Find("ULTBAR/ult");
        if (ultTextTransform != null)
        {
            TextMeshProUGUI ultText = ultTextTransform.GetComponent<TextMeshProUGUI>();
            if (ultText != null)
            {
                ultText.text = $"{currentUlt:F0}/{maxUlt:F0}";
            }
        }
    }

    public void SetNickname(string nickname)
    {
        // 닉네임 기능은 나중에 구현 예정
    }

    public bool IsUltReady()
    {
        return playerinfo.currentUlt >= maxUlt;
    }

    public void UseUlt()
    {
        if (IsUltReady())
        {
            UpdateUltGauge(0, maxUlt);
        }
    }

    public void SetPlayerIcon(Sprite playerSprite, string jobType = "")
    {
        Transform playerImgTransform = transform.Find("IconBG/mask/playerImg");
        if (playerImgTransform == null) return;

        Image playerImg = playerImgTransform.GetComponent<Image>();
        if (playerImg == null) return;

        // 항상 jobType 기반으로 로딩
        if (!string.IsNullOrEmpty(jobType))
        {
            string capitalJob = FirstCharToUpper(jobType);
            string spritePath = $"Character/{capitalJob}/PROFILE_{capitalJob}";

            Sprite overrideSprite = Resources.Load<Sprite>(spritePath);
            Debug.Log($"[ProfileUI] Try load sprite: {spritePath} => {(overrideSprite != null ? "Success" : "Fail")}");
            if (overrideSprite != null)
            {
                playerSprite = overrideSprite;
            }
            else
            {
                Debug.LogWarning($"[ProfileUI] Sprite '{spritePath}'을(를) Resources에서 찾지 못함");
                return;
            }
        }
        else if (playerSprite == null)
        {
            Debug.LogWarning("playerSprite도 없고 jobType도 없음");
            return;
        }

        playerImg.sprite = playerSprite;
        //나중에 이미지 각자 조절 필요할 때
        // playerImg.rectTransform.sizeDelta = new Vector2(1.23f, 1.19f);
        //
        // float scaleX = 1.12f;
        // float scaleY = 1.11f;
        //
        // // switch (playerSprite.name)
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
        // playerImg.rectTransform.anchoredPosition = new Vector2(-0.05f, -0.755f);
        // playerImg.rectTransform.localScale = new Vector3(scaleX, scaleY, 1);
    }


    private string FirstCharToUpper(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }

    // 게터 메서드들
    public string GetCurrentJobType() => playerinfo.job_type;
    public int GetCurrentHp() => playerinfo.currentHp;
    public float GetCurrentUlt() => playerinfo.currentUlt;
    public bool GetUltReady() => IsUltReady();
}
