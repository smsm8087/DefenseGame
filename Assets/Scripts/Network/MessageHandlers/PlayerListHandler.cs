using UnityEngine;
using System.Collections.Generic;

public class PlayerListHandler : INetworkMessageHandler
{
    private readonly GameObject localPlayerPrefab;
    private readonly GameObject remotePlayerPrefab;
    private readonly Dictionary<string, GameObject> players;

    public string Type => "player_list";

    public PlayerListHandler(GameObject localPrefab, GameObject remotePrefab, Dictionary<string, GameObject> players)
    {
        this.localPlayerPrefab = localPrefab;
        this.remotePlayerPrefab = remotePrefab;
        this.players = players;
    }

    public void Handle(NetMsg msg)
    {
        if (msg.players == null) return;

         foreach (string pid in msg.players)
        {
            if (players.ContainsKey(pid)) continue;

            GameObject prefabToUse = (pid == NetworkManager.Instance.MyGUID)
                ? localPlayerPrefab
                : remotePlayerPrefab;

            var playerObj = GameObject.Instantiate(prefabToUse);
            players[pid] = playerObj;
        }
    }
}