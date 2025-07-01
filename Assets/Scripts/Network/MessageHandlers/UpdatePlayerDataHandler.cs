using System;
using UnityEngine;
using System.Collections.Generic;
using UI;
using UnityEditor.UIElements;

public class UpdatePlayerDataHandler : INetworkMessageHandler
{
    public string Type => "update_player_data";
    private readonly Dictionary<string, GameObject> players;
    private readonly ProfileUI profileUI;
    public UpdatePlayerDataHandler(Dictionary<string, GameObject> players, ProfileUI profileUI)
    {
        this.players =  players;
        this.profileUI = profileUI;
    }
    public void Handle(NetMsg msg)
    {
        //프로필 최신화
        string pid = msg.playerInfo.id;
        var myPlayerObj = players[pid];
        if (myPlayerObj == null)
        {
            Debug.LogError("Player " + pid + " doesn't exist");
            return;
        }
        BasePlayer playerController = myPlayerObj.GetComponent<BasePlayer>();
        playerController.ApplyPlayerInfo(msg.playerInfo);
        
        profileUI.UpdatePlayerInfo(msg.playerInfo);
    }
}