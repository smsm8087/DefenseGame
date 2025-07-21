using UnityEngine;
using System.Collections.Generic;
using DataModels;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerJoinHandler : INetworkMessageHandler
{
    private readonly Dictionary<string, GameObject> players;
    private readonly NetworkManager networkManager;
    private readonly Dictionary<string, GameObject> prefabMap;

    public string Type => "player_join";

    private string pid = "";
    private PlayerInfo playerInfo;
    public PlayerJoinHandler(
        Dictionary<string, GameObject> prefabMap,
        Dictionary<string, GameObject> players,
        NetworkManager networkManager
    )
    {
        this.prefabMap = prefabMap;
        this.players = players;
        this.networkManager = networkManager;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "InGameScene")
        {
            // 내 플레이어인지 확인
            bool isMyPlayer = string.IsNullOrEmpty(networkManager.MyGUID);
    
            if (isMyPlayer)
            {
                // 내 플레이어 설정
                networkManager.SetMyGUID(pid);
        
                // 내 플레이어 프리팹 생성
                var myPlayerObj = PlayerSpawn(playerInfo);

                if (myPlayerObj == null) return;
                players[pid] = myPlayerObj;
            
                // 내 캐릭터 설정
                CameraFollow.Instance.setTarget(myPlayerObj.transform);
                BasePlayer playerController = myPlayerObj.GetComponent<BasePlayer>();
                playerController.enabled = true;
                myPlayerObj.GetComponent<SpriteRenderer>().sortingOrder = 1000;
        
                // 모바일 입력 등록
                MobileInputUI.Instance.RegisterPlayer(playerController);
        
                // ProfileUI 업데이트
                UpdateProfileUIForMyPlayer(playerInfo, myPlayerObj);
            }
            else
            {
                // 다른 플레이어 생성
                var playerObj = PlayerSpawn(playerInfo);
        
                players[pid] = playerObj;
                playerObj.GetComponent<NetworkCharacterFollower>().enabled = true;
            }
            
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
    public void Handle(NetMsg msg)
    {
        var pid = msg.playerInfo.id;
        if (players.ContainsKey(pid))
        {
            Debug.LogError("이미 존재하는 플레이어 id 입니다.");
            return;
        }
        this.pid = pid;
        this.playerInfo = msg.playerInfo;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // ProfileUI 업데이트를 별도 메서드로 분리
    private void UpdateProfileUIForMyPlayer(PlayerInfo playerinfo, GameObject myPlayerObj)
    {
        GameObject profileObj = GameObject.Find("ProfileUI");
        if (!profileObj) return;
        ProfileUI profileUI = profileObj.GetComponent<ProfileUI>();
        if(!profileUI) return;
        profileUI.InitializeProfile(playerinfo, myPlayerObj);
    }

    private GameObject PlayerSpawn(PlayerInfo playerinfo)
    {
        if (!prefabMap.TryGetValue(playerinfo.job_type.ToLower(), out var prefab))
        {
            Debug.LogError($"[PlayerSpawner] 알 수 없는 직업 타입: {playerinfo.job_type}");
            return null;
        }

        GameObject playerGO = GameObject.Instantiate(prefab);
        BasePlayer player = playerGO.GetComponent<BasePlayer>();
        player.ApplyPlayerInfo(playerinfo);

        return playerGO;
    }
}