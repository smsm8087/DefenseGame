using UnityEngine;
using System.Collections.Generic;

public class PlayerUpdateHpHandler : INetworkMessageHandler
{
    private readonly Dictionary<string, GameObject> players;
    private readonly ProfileUI profileUI;
    
    public string Type => "player_update_hp";
    
    public PlayerUpdateHpHandler(Dictionary<string, GameObject> players, ProfileUI profileUI)
    {
        this.players = players;
        this.profileUI = profileUI;
    }
    
    public void Handle(NetMsg msg)
    {
        if (msg.playerId == NetworkManager.Instance.MyGUID)
        {
            profileUI.UpdateHp(msg.playerInfo.currentHp, msg.playerInfo.currentMaxHp);
        }
    }
}