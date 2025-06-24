using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DataModels;
using System.Collections.Generic;

public class ProfileUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image iconBG; // IconBG 하위의 실제 아이콘 이미지
    [SerializeField] private Slider hpSlider; // HPBAR 하위의 슬라이더
    [SerializeField] private Image hpBG; // hp_outline 등
    [SerializeField] private Slider ultSlider; // ULTBAR 하위의 슬라이더  
    [SerializeField] private Image ultBG; // ult_outline 등
    [SerializeField] private TextMeshProUGUI nicknameText;

    // 현재 플레이어 정보
    private string currentJobType;
    private int currentPlayerId;
    private int maxHp = 100;
    private int currentHp = 100;
    private float maxUlt = 100f; // 모든 직업 동일한 최대값
    private float currentUlt = 0f; // 시작값 0
    private float ultIncrement = 0f; // 테이블에서 가져올 증가량

    private void Start()
    {
        InitializeProfile();
    }

    private void InitializeProfile()
    {
        // 초기 설정
        UpdateHp(currentHp, maxHp);
        UpdateUltGauge(currentUlt, maxUlt);
        
        // 플레이어 데이터 로드 시도
        LoadPlayerData();
    }

    private void LoadPlayerData()
    {
        // NetworkManager에서 내 플레이어 ID 가져오기
        string myPlayerId = NetworkManager.Instance.MyGUID;
        
        if (string.IsNullOrEmpty(myPlayerId))
        {
            Debug.LogWarning("[ProfileUI] 플레이어 ID를 찾을 수 없습니다.");
            return;
        }

        // 로컬 테이블에서 먼저 데이터 로드 시도
        LoadPlayerDataFromTable();
        
        // 서버에서 최신 데이터 요청 (직업 타입 등)
        RequestPlayerDataFromServer();
    }

    private void LoadPlayerDataFromTable()
    {
        // 기본값으로 Player 직업(ID=1) 데이터 로드
        var playerTable = GameDataManager.Instance.GetTable<PlayerData>("player_data");
        if (playerTable != null)
        {
            // ID 1: Player, ID 2: Programmer 등으로 설정되어 있다고 가정
            var defaultPlayerData = GameDataManager.Instance.GetData<PlayerData>("player_data", 1);
            if (defaultPlayerData != null)
            {
                SetJobType(defaultPlayerData.job_type);
                UpdateHp(defaultPlayerData.hp, defaultPlayerData.hp);
                UpdateUltGauge(0f, maxUlt); // 초기값은 항상 0
                ultIncrement = defaultPlayerData.ult_gauge; // 테이블의 ult_gauge는 증가량
                
                Debug.Log($"[ProfileUI] 테이블에서 기본 데이터 로드: {defaultPlayerData.job_type}, HP: {defaultPlayerData.hp}, ULT 증가량: {ultIncrement}");
            }
        }
    }

    private void RequestPlayerDataFromServer()
    {
        // 서버에 플레이어 데이터 요청 메시지 전송
        var request = new
        {
            type = "request_player_data",
            playerId = NetworkManager.Instance.MyGUID
        };
        
        NetworkManager.Instance.SendMsg(request);
    }

    // PlayerJoinHandler에서 호출할 깔끔한 메서드
    public void OnMyPlayerCreated(string jobType, GameObject playerObj)
    {
        Debug.Log($"[ProfileUI] 내 플레이어 생성 알림: {jobType}");
        
        SetJobType(jobType);
        
        var spriteRenderer = playerObj.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            SetPlayerIcon(spriteRenderer.sprite);
        }
        
        OnJobTypeReceived(jobType);
    }

    public void SetJobType(string jobType)
    {
        currentJobType = jobType;
        
        // 직업 변경 시 해당 직업의 ULT 증가량도 업데이트
        LoadUltIncrementFromTable(jobType);
        
        Debug.Log($"[ProfileUI] 직업 설정: {jobType}, ULT 증가량: {ultIncrement}");
    }

    private void LoadUltIncrementFromTable(string jobType)
    {
        var playerTable = GameDataManager.Instance.GetTable<PlayerData>("player_data");
        if (playerTable != null)
        {
            foreach (var kvp in playerTable)
            {
                var data = kvp.Value;
                if (data.job_type == jobType)
                {
                    ultIncrement = data.ult_gauge; // 테이블의 ult_gauge가 증가량
                    Debug.Log($"[ProfileUI] {jobType} 직업의 ULT 증가량 로드: {ultIncrement}");
                    break;
                }
            }
        }
    }

    public void UpdateHp(int currentHp, int maxHp)
    {
        this.currentHp = currentHp;
        this.maxHp = maxHp;

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

        Debug.Log($"[ProfileUI] HP 업데이트: {currentHp}/{maxHp}");
    }

    public void UpdateUltGauge(float currentUlt, float maxUlt)
    {
        this.currentUlt = Mathf.Clamp(currentUlt, 0, maxUlt);
        this.maxUlt = maxUlt;

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
            ultSlider.value = this.currentUlt;
            Debug.Log($"[ProfileUI] ULT 슬라이더 업데이트: {this.currentUlt}/{maxUlt}");
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
                    ultImage.fillAmount = this.currentUlt / maxUlt;
                    ultImage.type = Image.Type.Filled;
                    Debug.Log($"[ProfileUI] ULT 이미지 fillAmount 업데이트: {ultImage.fillAmount} ({this.currentUlt}/{maxUlt})");
                }
                else
                {
                    Debug.LogError("[ProfileUI] ult 오브젝트에 Image 컴포넌트가 없습니다!");
                }
            }
            else
            {
                Debug.LogError("[ProfileUI] ULTBAR/ult을 찾을 수 없습니다!");
            }
        }

        // ULT 텍스트 업데이트
        Transform ultTextTransform = transform.Find("ULTBAR/ult");
        if (ultTextTransform != null)
        {
            TextMeshProUGUI ultText = ultTextTransform.GetComponent<TextMeshProUGUI>();
            if (ultText != null)
            {
                ultText.text = $"{this.currentUlt:F0}/{maxUlt:F0}";
            }
        }

        Debug.Log($"[ProfileUI] ULT 게이지 업데이트 완료: {this.currentUlt}/{maxUlt}");
    }

    public void IncreaseUltGauge()
    {
        if (ultIncrement <= 0)
        {
            Debug.LogWarning($"[ProfileUI] ULT 증가량이 설정되지 않았습니다. 현재값: {ultIncrement}, 직업: {currentJobType}");
            
            // 백업: 직업별 기본값 사용
            if (!string.IsNullOrEmpty(currentJobType))
            {
                LoadUltIncrementFromTable(currentJobType);
                if (ultIncrement <= 0)
                {
                    // 여전히 0이면 기본값 사용
                    ultIncrement = currentJobType == "tank" ? 0.2f : 0.1f;
                    Debug.Log($"[ProfileUI] 기본값 사용: {currentJobType} → {ultIncrement}");
                }
            }
            else
            {
                Debug.LogWarning("[ProfileUI] 직업도 설정되지 않았습니다.");
                return;
            }
        }

        float newUlt = currentUlt + ultIncrement;
        UpdateUltGauge(newUlt, maxUlt);
        
        Debug.Log($"[ProfileUI] {currentJobType} 직업으로 ULT 게이지 {ultIncrement} 증가 (현재: {currentUlt}/{maxUlt})");
    }

    public void SetNickname(string nickname)
    {
        // 닉네임 기능은 나중에 구현 예정
    }

    // 서버에서 플레이어 데이터를 받았을 때 호출
    public void OnPlayerDataReceived(PlayerData playerData)
    {
        if (playerData == null) return;

        currentPlayerId = playerData.id;
        SetJobType(playerData.job_type);
        UpdateHp(playerData.hp, playerData.hp); // maxHp는 현재 hp와 동일하게 설정
        UpdateUltGauge(0f, maxUlt); // 시작 시 ULT는 항상 0
        ultIncrement = playerData.ult_gauge; // 테이블의 ult_gauge는 증가량
        
        Debug.Log($"[ProfileUI] 서버에서 플레이어 데이터 수신 완료 - ID: {playerData.id}, 직업: {playerData.job_type}, HP: {playerData.hp}, ULT 증가량: {playerData.ult_gauge}");
    }

    // 서버에서 직업 타입을 받았을 때 테이블에서 해당 직업 데이터 로드
    public void OnJobTypeReceived(string jobType)
    {
        var playerTable = GameDataManager.Instance.GetTable<PlayerData>("player_data");
        if (playerTable != null)
        {
            // 테이블에서 해당 직업 타입의 데이터 찾기
            foreach (var kvp in playerTable)
            {
                var data = kvp.Value;
                if (data.job_type == jobType)
                {
                    currentPlayerId = data.id;
                    SetJobType(data.job_type);
                    UpdateHp(data.hp, data.hp);
                    UpdateUltGauge(0f, maxUlt); // 시작 시 ULT는 항상 0
                    ultIncrement = data.ult_gauge; // 테이블의 ult_gauge는 증가량
                    
                    Debug.Log($"[ProfileUI] 테이블에서 직업 데이터 로드: ID {data.id}, {data.job_type}, HP: {data.hp}, ULT 증가량: {data.ult_gauge}");
                    break;
                }
            }
        }
    }

    // 몬스터 공격 성공 시 호출할 메서드
    public void OnMonsterAttackSuccess()
    {
        IncreaseUltGauge();
    }

    public bool IsUltReady()
    {
        return currentUlt >= maxUlt;
    }

    public void UseUlt()
    {
        if (IsUltReady())
        {
            UpdateUltGauge(0, maxUlt);
            Debug.Log("[ProfileUI] 궁극기 사용! ULT 게이지 초기화");
        }
        else
        {
            Debug.Log($"[ProfileUI] 궁극기 사용 불가 - 현재: {currentUlt}/{maxUlt}");
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
                
                Debug.Log("[ProfileUI] playerImg에 플레이어 스프라이트 설정 완료");
            }
            else
            {
                Debug.LogWarning("[ProfileUI] playerImg에 Image 컴포넌트가 없습니다.");
            }
        }
        else
        {
            Debug.LogWarning("[ProfileUI] IconBG/mask/playerImg 오브젝트를 찾을 수 없습니다.");
        }
    }

    // 게터 메서드들
    public string GetCurrentJobType() => currentJobType;
    public int GetCurrentHp() => currentHp;
    public float GetCurrentUlt() => currentUlt;
    public float GetUltIncrement() => ultIncrement;
    public bool GetUltReady() => IsUltReady();
}