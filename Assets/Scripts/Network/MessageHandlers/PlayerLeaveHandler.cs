using UnityEngine;
using System.Collections.Generic;

public class PlayerLeaveHandler : MonoBehaviour, INetworkMessageHandler
{
    private readonly Dictionary<string, GameObject> players;
    public string Type => "player_leave";

    public PlayerLeaveHandler(Dictionary<string, GameObject> players)
    {
        this.players = players;
    }

    public void Handle(NetMsg msg)
    {
        string pid = msg.playerId;
        if (players.ContainsKey(pid))
        {
            Destroy(players[pid]);
            players.Remove(pid);
        }
    }
}