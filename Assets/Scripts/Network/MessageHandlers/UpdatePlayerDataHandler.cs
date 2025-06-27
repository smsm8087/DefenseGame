using System;
using UnityEngine;
using System.Collections.Generic;
using UI;

public class UpdatePlayerDataHandler : INetworkMessageHandler
{
    public string Type => "update_player_data";
    private readonly Dictionary<string, GameObject> players;
    public UpdatePlayerDataHandler(Dictionary<string, GameObject> players)
    {
        this.players =  players;
    }
    public void Handle(NetMsg msg)
    {
        string pid = msg.playerInfo.id;
        var myPlayerObj = players[pid];
        if (myPlayerObj == null)
        {
            Debug.LogError("Player " + pid + " doesn't exist");
            return;
        }
        PlayerController playerController = myPlayerObj.GetComponent<PlayerController>();
        playerController.setMoveSpeed(msg.playerInfo.currentMoveSpeed);
    }
}