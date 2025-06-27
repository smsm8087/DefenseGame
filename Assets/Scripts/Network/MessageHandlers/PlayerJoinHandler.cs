using UnityEngine;
using System.Collections.Generic;
using DataModels;
using UnityEngine.UI;

public class PlayerJoinHandler : INetworkMessageHandler
{
    private readonly List<GameObject> playerPrefabs;
    private readonly Dictionary<string, GameObject> players;
    private readonly NetworkManager networkManager;
    private readonly Dictionary<string, int> jobToPrefabIndex;
    private readonly ProfileUI profileUI;

    public string Type => "player_join";

    public PlayerJoinHandler(
        List<GameObject> playerPrefabs,
        Dictionary<string, GameObject> players,
        NetworkManager networkManager,
        ProfileUI profileUI
    )
    {
        this.playerPrefabs = playerPrefabs;
        this.players = players;
        this.networkManager = networkManager;
        this.profileUI = profileUI;
        
        // 직업과 프리팹 인덱스 매핑 초기화 (서버와 일치)
        jobToPrefabIndex = new Dictionary<string, int>
        {
            {"tank", 0},        // Player → tank
            {"programmer", 1}   // Programmer → programmer
        };
    }

    public void Handle(NetMsg msg)
    {
        var pid = msg.playerInfo.id;
        if (players.ContainsKey(pid))
        {
            Debug.LogError("이미 존재하는 플레이어 id 입니다.");
            return;
        }
    
        // 내 플레이어인지 확인
        bool isMyPlayer = string.IsNullOrEmpty(networkManager.MyGUID);
    
        if (isMyPlayer)
        {
            // 내 플레이어 설정
            networkManager.SetMyGUID(pid);
        
            // 내 플레이어 프리팹 생성
            int prefabIndex = GetPrefabIndex(msg.playerInfo.job_type);
            var myPlayerObj = GameObject.Instantiate(playerPrefabs[prefabIndex]);
            players[pid] = myPlayerObj;
            
            // 내 캐릭터 설정
            CameraFollow.Instance.setTarget(myPlayerObj.transform);
            PlayerController playerController = myPlayerObj.GetComponent<PlayerController>();
            playerController.enabled = true;
            playerController.playerGUID = pid;
            myPlayerObj.GetComponent<SpriteRenderer>().sortingOrder = 100;
        
            // 모바일 입력 등록
            MobileInputUI.Instance.RegisterPlayer(playerController);
        
            // ProfileUI 업데이트
            UpdateProfileUIForMyPlayer(msg.playerInfo, myPlayerObj);
        }
        else
        {
            // 다른 플레이어 생성
            int prefabIndex = GetPrefabIndex(msg.playerInfo.job_type);
            var playerObj = GameObject.Instantiate(playerPrefabs[prefabIndex]);
        
            players[pid] = playerObj;
            playerObj.GetComponent<NetworkCharacterFollower>().enabled = true;
        }
    }

    // ProfileUI 업데이트를 별도 메서드로 분리
    private void UpdateProfileUIForMyPlayer(PlayerInfo playerinfo, GameObject myPlayerObj)
    {
        profileUI.InitializeProfile(playerinfo, myPlayerObj);
    }
    
    private int GetPrefabIndex(string jobType)
    {
        if (!string.IsNullOrEmpty(jobType) && jobToPrefabIndex.ContainsKey(jobType))
        {
            return jobToPrefabIndex[jobType];
        }
        else
        {
            Debug.LogWarning($"알 수 없는 직업: {jobType}. 기본 프리팹 사용");
            return 0; // 기본값
        }
    }
}