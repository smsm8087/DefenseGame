using System.Collections.Generic;
using UnityEngine;

public class PlayerReviveHandler : INetworkMessageHandler
{
    private readonly Dictionary<string, GameObject> players;
    
    public string Type => "player_revive";
    
    public PlayerReviveHandler(Dictionary<string, GameObject> players)
    {
        this.players = players;
    }
    
    public void Handle(NetMsg msg)
    {
        if (players.TryGetValue(msg.playerId, out GameObject playerObj))
        {
            BasePlayer player = playerObj.GetComponent<BasePlayer>();
            if (player != null)
            {
                player.Revive();
                Debug.Log($"[PlayerReviveHandler] {msg.playerId} 부활 처리");
            }
        }
    }
}