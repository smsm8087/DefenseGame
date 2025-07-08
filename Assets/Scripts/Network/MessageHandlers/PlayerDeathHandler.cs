using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathHandler : INetworkMessageHandler
{
    private readonly Dictionary<string, GameObject> players;
    
    public string Type => "player_death";
    
    public PlayerDeathHandler(Dictionary<string, GameObject> players)
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
                player.Die();
                Debug.Log($"[PlayerDeathHandler] {msg.playerId} 사망 처리");
            }
        }
    }
}