﻿using UnityEngine;
using System.Collections.Generic;
public class PlayerJoinHandler : INetworkMessageHandler
{
    private readonly GameObject playerPrefab;
    private readonly Dictionary<string, GameObject> players;
    private readonly NetworkManager networkManager;

    public string Type => "player_join";

    public PlayerJoinHandler(
        GameObject playerPrefab,
        Dictionary<string, GameObject> players,
        NetworkManager networkManager)
    {
        this.playerPrefab = playerPrefab;
        this.players = players;
        this.networkManager = networkManager;
    }

    public void Handle(NetMsg msg)
    {
        var pid = msg.playerId;
        if (!players.ContainsKey(pid))
        {
            var playerObj = GameObject.Instantiate(playerPrefab);
            
            if (string.IsNullOrEmpty(networkManager.MyGUID))
            {
                //내 캐릭터 생성.
                networkManager.SetMyGUID(pid); 
                CameraFollow.Instance.setTarget(playerObj.transform);
                PlayerController playerController = playerObj.GetComponent<PlayerController>();
                playerController.enabled = true;
                playerController.playerGUID = pid;
                playerObj.GetComponent<SpriteRenderer>().sortingOrder = 10;
            }
            else
            {
                players[pid] = playerObj;
                playerObj.GetComponent<NetworkCharacterFollower>().enabled = true;
            }
        }
    }
}