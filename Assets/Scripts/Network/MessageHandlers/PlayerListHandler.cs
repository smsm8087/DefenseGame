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
        
        // 직업과 프리팹 인덱스 매핑 초기화 (PlayerJoinHandler와 동일)
        jobToPrefabIndex = new Dictionary<string, int>
        {
            {"Player", 0},     
            {"Programmer", 1}
        };
    }

    public void Handle(NetMsg msg)
    {
        if (msg.players == null) return;

        foreach (var playerData in msg.players)
        {
            string pid = playerData.playerId;
            string jobType = playerData.jobType;
            
            if (players.ContainsKey(pid)) continue;
            if (pid == NetworkManager.Instance.MyGUID) continue;
            
            // 직업에 맞는 프리팹 선택
            int prefabIndex = 0; 
            if (!string.IsNullOrEmpty(jobType) && jobToPrefabIndex.ContainsKey(jobType))
            {
                prefabIndex = jobToPrefabIndex[jobType];
                Debug.Log($"기존 플레이어 {pid}의 직업 {jobType} 확인됨 (프리팹 인덱스: {prefabIndex})");
            }
            else
            {
                Debug.LogWarning($"플레이어 {pid}의 직업 정보 없음. 기본 프리팹 사용");
            }
            
            var playerObj = GameObject.Instantiate(playerPrefabs[prefabIndex]);
            players[pid] = playerObj;
            
            NetworkCharacterFollower playerFollower = playerObj.GetComponent<NetworkCharacterFollower>();
            if (playerFollower)
            {
                playerObj.GetComponent<NetworkCharacterFollower>().enabled = true;
            }
        }
    }
}