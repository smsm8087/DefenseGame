using System;
using System.Collections;
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

    private Image hpImage;
    private TextMeshProUGUI hpText;
    private Image ultImage;
    private TextMeshProUGUI ultText;

    private void Start()
    {
        hpImage = transform.Find("HPBAR/hp").GetComponent<Image>();
        //hpText = transform.Find("HPBAR/hpBG/hp").GetComponent<TextMeshProUGUI>();
        ultImage =  transform.Find("ULTBAR/ult").GetComponent<Image>();
        //ultText = transform.Find("ULTBAR/ult").GetComponent<TextMeshProUGUI>();
    }

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
        float hpPct = Mathf.Clamp01((float)currentHp / (float)maxHp);
        StartCoroutine(LerpGaugeBar(hpPct, hpImage));
        // if (hpText != null)
        // {
        //     hpText.text = $"{currentHp}/{maxHp}";
        // }
    }
    public IEnumerator LerpGaugeBar(float targetPercent, Image targetImg)
    {
        if (targetImg == null) yield break;
        targetImg.type = Image.Type.Filled;
        
        float duration = 0.2f;
        float elapsed = 0f;
        float startFill = targetImg.fillAmount;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            targetImg.fillAmount = Mathf.Lerp(startFill, targetPercent, elapsed / duration);
            yield return null;
        }
        targetImg.fillAmount = targetPercent; 
    }

    public void UpdateUltGauge(float currentUlt, float maxUlt)
    {
        float ultPct = Mathf.Clamp01((float)currentUlt / (float)maxUlt);
        StartCoroutine(LerpGaugeBar(ultPct, ultImage));
        // if (ultText != null)
        // {
        //     ultText.text = $"{currentUlt:F0}/{maxUlt:F0}";
        // }
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
