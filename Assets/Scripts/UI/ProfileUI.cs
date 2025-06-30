using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DataModels;
using System.Collections.Generic;
using System.Linq;

public class ProfileUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image iconBG; // IconBG 하위의 실제 아이콘 이미지
    [SerializeField] private Slider hpSlider; // HPBAR 하위의 슬라이더
    [SerializeField] private Image hpBG; // hp_outline 등
    [SerializeField] private Slider ultSlider; // ULTBAR 하위의 슬라이더  
    [SerializeField] private Image ultBG; // ult_outline 등
    [SerializeField] private TextMeshProUGUI nicknameText;
    
    [Header("StatUI")]
    [SerializeField] private Button statUIButton;

    // 현재 플레이어 정보
    private float maxUlt = 100f; // 모든 직업 동일한 최대값
    private PlayerInfo playerinfo;
    public void InitializeProfile(PlayerInfo playerinfo, GameObject player)
    {
        //서버에서 받은 플레이어 데이터 세팅
        this.playerinfo = playerinfo;
        UpdateHp(playerinfo.currentHp, playerinfo.currentHp);
        UpdateUltGauge(0, maxUlt);
        var spriteRenderer = player.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            SetPlayerIcon(spriteRenderer.sprite);
        }
        statUIButton?.onClick.AddListener(OnShowStatPopup);
    }

    public void OnShowStatPopup()
    {
        UIManager.Instance.ShowStatPopup();
    }
    public void UpdateHp(int currentHp, int maxHp)
    {
        // HPBAR 하위에서 슬라이더 찾기
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

        // HP 텍스트 업데이트 (있다면)
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
        // ULTBAR 하위에서 슬라이더 찾기
        if (ultSlider == null)
        {
            Transform ultBarTransform = transform.Find("ULTBAR");
            if (ultBarTransform != null)
            {
                ultSlider = ultBarTransform.GetComponentInChildren<Slider>();
            }
        }

        // Slider가 있으면 Slider 사용
        if (ultSlider != null)
        {
            ultSlider.maxValue = maxUlt;
            ultSlider.value = currentUlt;
        }
        else
        {
            // Slider가 없으면 Image fillAmount 사용
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

        // ULT 텍스트 업데이트
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
    
    // 플레이어의 실제 스프라이트를 설정하는 메서드
    public void SetPlayerIcon(Sprite playerSprite)
    {
        // IconBG/mask/playerImg에서 플레이어 아이콘 설정
        Transform playerImgTransform = transform.Find("IconBG/mask/playerImg");
        if (playerImgTransform != null)
        {
            Image playerImgComponent = playerImgTransform.GetComponent<Image>();
            if (playerImgComponent != null)
            {
                // Image Type을 Simple로 설정하여 일관성 유지
                playerImgComponent.sprite = playerSprite;
                playerImgComponent.type = Image.Type.Simple;
                playerImgComponent.preserveAspect = false;
            }
        }
    }

    // 게터 메서드들
    public string GetCurrentJobType() => playerinfo.job_type;
    public int GetCurrentHp() => playerinfo.currentHp;
    public float GetCurrentUlt() => playerinfo.currentUlt;
    public bool GetUltReady() => IsUltReady();
}