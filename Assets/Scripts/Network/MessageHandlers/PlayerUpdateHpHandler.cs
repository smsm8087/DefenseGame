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
        // 플레이어 HP 업데이트 (사망 체크 포함)
        if (players.TryGetValue(msg.playerId, out GameObject playerObj))
        {
            BasePlayer player = playerObj.GetComponent<BasePlayer>();
            if (player != null)
            {
                player.UpdateHp(msg.playerInfo.currentHp);
            }
        }
        
        if (msg.playerId == NetworkManager.Instance.MyGUID)
        {
            profileUI.UpdateHp(msg.playerInfo.currentHp, msg.playerInfo.currentMaxHp);
        }
    }
}