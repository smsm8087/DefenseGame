using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerJoinHandler : INetworkMessageHandler
{
    private readonly List<GameObject> playerPrefabs;
    private readonly Dictionary<string, GameObject> players;
    private readonly NetworkManager networkManager;
    private readonly Dictionary<string, int> jobToPrefabIndex;

    public string Type => "player_join";

    public PlayerJoinHandler(
        List<GameObject> playerPrefabs,
        Dictionary<string, GameObject> players,
        NetworkManager networkManager)
    {
        this.playerPrefabs = playerPrefabs;
        this.players = players;
        this.networkManager = networkManager;
        
        // 직업과 프리팹 인덱스 매핑 초기화 (서버와 일치)
        jobToPrefabIndex = new Dictionary<string, int>
        {
            {"tank", 0},        // Player → tank
            {"programmer", 1}   // Programmer → programmer
        };
    }

    public void Handle(NetMsg msg)
    {
        var pid = msg.playerId;
    
        // 내 플레이어인지 확인
        bool isMyPlayer = string.IsNullOrEmpty(networkManager.MyGUID);
    
        if (isMyPlayer)
        {
            // 내 플레이어 설정
            networkManager.SetMyGUID(pid);
            Debug.Log($"[PlayerJoinHandler] 내 플레이어 설정: {pid}, 직업: {msg.jobType}");
        
            // 내 플레이어 프리팹 생성
            int prefabIndex = GetPrefabIndex(msg.jobType);
            var myPlayerObj = GameObject.Instantiate(playerPrefabs[prefabIndex]);
        
            // 내 캐릭터 설정
            CameraFollow.Instance.setTarget(myPlayerObj.transform);
            PlayerController playerController = myPlayerObj.GetComponent<PlayerController>();
            playerController.enabled = true;
            playerController.playerGUID = pid;
            myPlayerObj.GetComponent<SpriteRenderer>().sortingOrder = 100;
        
            // 모바일 입력 등록
            MobileInputUI.Instance.RegisterPlayer(playerController);
        
            // ProfileUI 업데이트
            UpdateProfileUIForMyPlayer(myPlayerObj, msg.jobType);
        
            Debug.Log($"[PlayerJoinHandler] 내 플레이어 생성 완료: {pid}");
        }
        else if (!players.ContainsKey(pid) && pid != networkManager.MyGUID)
        {
            // 다른 플레이어 생성
            int prefabIndex = GetPrefabIndex(msg.jobType);
            var playerObj = GameObject.Instantiate(playerPrefabs[prefabIndex]);
        
            players[pid] = playerObj;
            playerObj.GetComponent<NetworkCharacterFollower>().enabled = true;
        
            Debug.Log($"[PlayerJoinHandler] 다른 플레이어 생성: {pid}, 직업: {msg.jobType}");
        }
    }

    // ProfileUI 업데이트를 별도 메서드로 분리
    private void UpdateProfileUIForMyPlayer(GameObject myPlayerObj, string jobType)
    {
        var profileUI = Object.FindFirstObjectByType<ProfileUI>();
        profileUI?.OnMyPlayerCreated(jobType, myPlayerObj);
    }
    
    private int GetPrefabIndex(string jobType)
    {
        if (!string.IsNullOrEmpty(jobType) && jobToPrefabIndex.ContainsKey(jobType))
        {
            Debug.Log($"직업 {jobType} → 프리팹 인덱스: {jobToPrefabIndex[jobType]}");
            return jobToPrefabIndex[jobType];
        }
        else
        {
            Debug.LogWarning($"알 수 없는 직업: {jobType}. 기본 프리팹 사용");
            return 0; // 기본값
        }
    }
}