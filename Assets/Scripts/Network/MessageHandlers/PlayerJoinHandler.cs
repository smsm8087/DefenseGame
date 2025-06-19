using UnityEngine;
using System.Collections.Generic;
public class PlayerJoinHandler : INetworkMessageHandler
{
    private readonly List<GameObject> playerPrefabs;
    private readonly Dictionary<string, GameObject> players;
    private readonly NetworkManager networkManager;

    public string Type => "player_join";

    public PlayerJoinHandler(
        List<GameObject> playerPrefabs,
        Dictionary<string, GameObject> players,
        NetworkManager networkManager)
    {
        this.playerPrefabs = playerPrefabs;
        this.players = players;
        this.networkManager = networkManager;
    }

    public void Handle(NetMsg msg)
    {
        var pid = msg.playerId;
        if (!players.ContainsKey(pid))
        {
            var rand = Random.Range(0, playerPrefabs.Count);
            var playerObj = GameObject.Instantiate(playerPrefabs[rand]);
            
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