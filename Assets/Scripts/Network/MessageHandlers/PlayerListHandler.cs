using UnityEngine;
using System.Collections.Generic;

public class PlayerListHandler : INetworkMessageHandler
{
    private readonly List<GameObject> playerPrefabs;
    private readonly Dictionary<string, GameObject> players;
    
    // 직업명과 프리팹 인덱스 매핑 (PlayerJoinHandler와 동일하게)
    private readonly Dictionary<string, int> jobToPrefabIndex;

    public string Type => "player_list";

    public PlayerListHandler(List<GameObject> playerPrefabs, Dictionary<string, GameObject> players)
    {
        this.playerPrefabs = playerPrefabs;
        this.players = players;
        
        // 직업과 프리팹 인덱스 매핑 초기화 
        jobToPrefabIndex = new Dictionary<string, int>
        {
            {"tank", 0},     
            {"programmer", 1}
        };
    }

    public void Handle(NetMsg msg)
    {
        if (msg.players == null) return;

        Debug.Log($"[PlayerListHandler] 플레이어 목록 처리 시작. 총 {msg.players.Count}명");

        foreach (var playerData in msg.players)
        {
            string pid = playerData.id; // .ToString() 제거
            string jobType = playerData.job_type;
            
            Debug.Log($"[PlayerListHandler] 처리 중: PID={pid}, JobType={jobType}, MyGUID={NetworkManager.Instance.MyGUID}");
            
            // 잘못된 데이터 필터링
            if (string.IsNullOrEmpty(pid) || string.IsNullOrEmpty(jobType))
            {
                Debug.LogWarning($"[PlayerListHandler] 잘못된 플레이어 데이터 스킵: PID={pid}, JobType={jobType}");
                continue;
            }
            
            // 이미 존재하는 플레이어는 스킵
            if (players.ContainsKey(pid))
            {
                Debug.Log($"[PlayerListHandler] 이미 존재하는 플레이어 스킵: {pid}");
                continue;
            }
            
            // 직업에 맞는 프리팹 선택
            int prefabIndex = 0; 
            if (jobToPrefabIndex.ContainsKey(jobType))
            {
                prefabIndex = jobToPrefabIndex[jobType];
            }
            else
            {
                Debug.LogWarning($"알 수 없는 직업 타입: {jobType}. 기본 프리팹 사용");
            }
            
            var playerObj = GameObject.Instantiate(playerPrefabs[prefabIndex]);
            players[pid] = playerObj;
            
            NetworkCharacterFollower playerFollower = playerObj.GetComponent<NetworkCharacterFollower>();
            if (playerFollower)
            {
                playerObj.GetComponent<NetworkCharacterFollower>().enabled = true;
            }
            
            Debug.Log($"[PlayerListHandler] 다른 플레이어 생성 완료: {pid}");
        }
    }
}