using UnityEngine;
using System.Collections.Generic;

public class PlayerListHandler : INetworkMessageHandler
{
    private readonly GameObject playerPrefab;
    private readonly Dictionary<string, GameObject> players;

    public string Type => "player_list";

    public PlayerListHandler(GameObject playerPrefab, Dictionary<string, GameObject> players)
    {
        this.playerPrefab = playerPrefab;
        this.players = players;
    }

    public void Handle(NetMsg msg)
    {
        if (msg.players == null) return;

         foreach (string pid in msg.players)
        {
            if (players.ContainsKey(pid)) continue;
            if (pid == NetworkManager.Instance.MyGUID) continue;
            var playerObj = GameObject.Instantiate(playerPrefab);
            players[pid] = playerObj;
            playerObj.GetComponent<NetworkCharacterFollower>().enabled = true;
        }
    }
}