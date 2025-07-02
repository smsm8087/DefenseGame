using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DataModels;
public class PartyMemberData
{
    public string id { get; set; }
    public string job_type { get; set; }
    public float current_health { get; set; }
    public float max_health { get; set; }
    public float current_ult { get; set; }
    public float max_ult { get; set; }
}
public class PartyMemberUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform partyContainer; // Canvas/PartyMember
    [SerializeField] private GameObject memberPrefab; // 파티원 프리팹

    [Header("Job Icons")]
    [SerializeField] private Sprite tankIcon;
    [SerializeField] private Sprite programmerIcon;
    [SerializeField] private Sprite sniperIcon;

    [Header("Test")]
    [SerializeField] private bool enableTestMode = false;

    private Dictionary<string, PartyMemberIcon> partyMembers = new Dictionary<string, PartyMemberIcon>();
    private Dictionary<string, Sprite> jobIcons = new Dictionary<string, Sprite>();

    public static PartyMemberUI Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 자동으로 컨테이너 찾기
        if (partyContainer == null)
        {
            GameObject canvasObj = GameObject.Find("Canvas");
            if (canvasObj != null)
            {
                Transform partyMemberTransform = canvasObj.transform.Find("PartyMember");
                if (partyMemberTransform != null)
                    partyContainer = partyMemberTransform;
            }
        }

        InitializeJobIcons();
    }

    private void Update()
    {
        if (!enableTestMode) return;

        // 테스트 단축키들
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TestAddPlayer("player1", "tank");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TestAddPlayer("player2", "programmer");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TestUpdateHealth("player1", 50f, 100f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TestUpdateUlt("player1", 75f, 100f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            TestRemovePlayer("player1");
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            TestPartyInfo();
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ClearAllMembers();
        }
    }

    private void InitializeJobIcons()
    {
        if (tankIcon != null)
            jobIcons["tank"] = tankIcon;
        if (programmerIcon != null)
            jobIcons["programmer"] = programmerIcon;
        if (sniperIcon != null)
            jobIcons["sniper"] = sniperIcon;
    }

    public void AddPartyMember(string playerId, string jobType)
    {
        // 내 플레이어는 파티 UI에 추가하지 않음
        if (playerId == NetworkManager.Instance.MyGUID)
        {
            Debug.Log($"내 플레이어({playerId})는 파티 UI에서 제외");
            return;
        }

        if (partyMembers.ContainsKey(playerId))
        {
            Debug.LogWarning($"파티원 {playerId}가 이미 존재합니다.");
            return;
        }

        if (memberPrefab == null || partyContainer == null)
        {
            Debug.LogError("멤버 프리팹 또는 파티 컨테이너가 설정되지 않았습니다.");
            return;
        }

        // 프리팹 생성
        GameObject memberObj = Instantiate(memberPrefab, partyContainer);
        PartyMemberIcon memberIcon = memberObj.GetComponent<PartyMemberIcon>();

        if (memberIcon == null)
        {
            memberIcon = memberObj.AddComponent<PartyMemberIcon>();
        }

        // 직업에 맞는 아이콘 설정
        Sprite icon = jobIcons.ContainsKey(jobType) ? jobIcons[jobType] : null;
        memberIcon.Initialize(playerId, jobType, icon);

        partyMembers[playerId] = memberIcon;

        Debug.Log($"파티원 UI 추가: {playerId} ({jobType})");
    }

    public void RemovePartyMember(string playerId)
    {
        if (partyMembers.TryGetValue(playerId, out PartyMemberIcon memberIcon))
        {
            if (memberIcon != null)
                Destroy(memberIcon.gameObject);
            
            partyMembers.Remove(playerId);
            Debug.Log($"파티원 UI 제거: {playerId}");
        }
    }

    public void UpdateMemberHealth(string playerId, float currentHp, float maxHp)
    {
        if (partyMembers.TryGetValue(playerId, out PartyMemberIcon memberIcon))
        {
            memberIcon.UpdateHealth(currentHp, maxHp);
        }
    }

    public void UpdateMemberUlt(string playerId, float currentUlt, float maxUlt)
    {
        if (partyMembers.TryGetValue(playerId, out PartyMemberIcon memberIcon))
        {
            memberIcon.UpdateUlt(currentUlt, maxUlt);
        }
    }

    public void UpdateMemberStatus(string playerId, string status)
    {
        if (partyMembers.TryGetValue(playerId, out PartyMemberIcon memberIcon))
        {
            memberIcon.SetStatus(status);
        }
    }

    public void UpdatePartyInfo(List<PartyMemberData> members)
    {
        // 내 플레이어 제외한 멤버들만 필터링
        var otherMembers = members.Where(m => m.id != NetworkManager.Instance.MyGUID).ToList();
        
        // 기존 멤버들과 비교하여 추가/제거
        var currentMemberIds = partyMembers.Keys.ToHashSet();
        var newMemberIds = otherMembers.Select(m => m.id).ToHashSet();

        // 제거할 멤버들
        var toRemove = currentMemberIds.Except(newMemberIds);
        foreach (var memberId in toRemove)
        {
            RemovePartyMember(memberId);
        }

        // 추가/업데이트할 멤버들
        foreach (var member in otherMembers)
        {
            if (!partyMembers.ContainsKey(member.id))
            {
                AddPartyMember(member.id, member.job_type);
            }

            // 정보 업데이트
            UpdateMemberHealth(member.id, member.current_health, member.max_health);
            UpdateMemberUlt(member.id, member.current_ult, member.max_ult);
        }
    }

    public void ClearAllMembers()
    {
        foreach (var kvp in partyMembers)
        {
            if (kvp.Value != null)
                Destroy(kvp.Value.gameObject);
        }
        partyMembers.Clear();
    }

    public int GetMemberCount()
    {
        return partyMembers.Count;
    }

    // ====== 테스트 메서드들 ======
    [ContextMenu("Test Add Tank Player")]
    public void TestAddTankPlayer()
    {
        TestAddPlayer("tank_player", "tank");
    }

    [ContextMenu("Test Add Programmer Player")]
    public void TestAddProgrammerPlayer()
    {
        TestAddPlayer("programmer_player", "programmer");
    }

    [ContextMenu("Test Update Health")]
    public void TestUpdateHealthMenu()
    {
        TestUpdateHealth("tank_player", 50f, 100f);
    }

    [ContextMenu("Test Update Ult")]
    public void TestUpdateUltMenu()
    {
        TestUpdateUlt("tank_player", 75f, 100f);
    }

    [ContextMenu("Test Party Info")]
    public void TestPartyInfoMenu()
    {
        TestPartyInfo();
    }

    [ContextMenu("Clear All Members")]
    public void ClearAllMembersMenu()
    {
        ClearAllMembers();
    }

    public void TestAddPlayer(string playerId, string jobType)
    {
        AddPartyMember(playerId, jobType);
        Debug.Log($"[TEST] 플레이어 추가: {playerId} ({jobType})");
    }

    public void TestUpdateHealth(string playerId, float hp, float maxHp)
    {
        UpdateMemberHealth(playerId, hp, maxHp);
        Debug.Log($"[TEST] 체력 업데이트: {playerId} -> {hp}/{maxHp}");
    }

    public void TestUpdateUlt(string playerId, float ult, float maxUlt)
    {
        UpdateMemberUlt(playerId, ult, maxUlt);
        Debug.Log($"[TEST] 궁극기 업데이트: {playerId} -> {ult}/{maxUlt}");
    }

    public void TestRemovePlayer(string playerId)
    {
        RemovePartyMember(playerId);
        Debug.Log($"[TEST] 플레이어 제거: {playerId}");
    }

    public void TestPartyInfo()
    {
        var testMembers = new List<PartyMemberData>
        {
            new PartyMemberData { id = "test1", job_type = "tank", current_health = 80, max_health = 100, current_ult = 30, max_ult = 100 },
            new PartyMemberData { id = "test2", job_type = "programmer", current_health = 60, max_health = 100, current_ult = 90, max_ult = 100 }
        };
        UpdatePartyInfo(testMembers);
        Debug.Log("[TEST] 파티 정보 업데이트 완료");
    }
}