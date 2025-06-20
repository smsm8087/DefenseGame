using UnityEngine;
using System.Collections.Generic;

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
        
        // 직업과 프리팹 인덱스 매핑 초기화
        jobToPrefabIndex = new Dictionary<string, int>
        {
            {"Player", 0},      
            {"Programmer", 1}  
        };
    }

    public void Handle(NetMsg msg)
    {
        var pid = msg.playerId;
        if (!players.ContainsKey(pid))
        {
            int prefabIndex;
            
            if (!string.IsNullOrEmpty(msg.jobType) && jobToPrefabIndex.ContainsKey(msg.jobType))
            {
                prefabIndex = jobToPrefabIndex[msg.jobType];
                Debug.Log($"플레이어 {pid}에게 직업 {msg.jobType} 할당됨 (프리팹 인덱스: {prefabIndex})");
            }
            else
            {
                prefabIndex = Random.Range(0, playerPrefabs.Count);
                Debug.LogWarning($"플레이어 {pid}의 직업 정보가 없거나 잘못됨. 랜덤 선택: {prefabIndex}");
            }
            
            var playerObj = GameObject.Instantiate(playerPrefabs[prefabIndex]);
            
            if (string.IsNullOrEmpty(networkManager.MyGUID))
            {
                //내 캐릭터 생성.
                networkManager.SetMyGUID(pid); 
                CameraFollow.Instance.setTarget(playerObj.transform);
                PlayerController playerController = playerObj.GetComponent<PlayerController>();
                playerController.enabled = true;
                playerController.playerGUID = pid;
                playerObj.GetComponent<SpriteRenderer>().sortingOrder = 10;
                
                //mobile input에 등록
                MobileInputUI.Instance.RegisterPlayer(playerController);
            }
            else
            {
                players[pid] = playerObj;
                playerObj.GetComponent<NetworkCharacterFollower>().enabled = true;
            }
        }
    }
}