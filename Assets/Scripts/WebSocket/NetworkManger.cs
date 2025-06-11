using System;
using System.Collections.Generic;
using System.Text;
using NativeWebSocket;
using UnityEngine;
using Newtonsoft.Json;



public class NetworkManger : MonoBehaviour
{
    public static NetworkManger Instance;
    public GameObject playerPrefab;
    public Dictionary<string, GameObject> players = new();
    void Awake()
    {
        Instance = this;
    }
    public void SendMsg( params object[] msg)
    {
        string json = JsonConvert.SerializeObject(msg[0]);
        DefenseGameWebSocket.Instance.websocket.SendText(json);
    }
    public void HandleMessage(string msg)
    {
        NetMsg netMsg = JsonConvert.DeserializeObject<NetMsg>(msg);

        if (netMsg.type == "player_list")
        {
            if (netMsg.players != null)
            {
                foreach (var pid in netMsg.players)
                {
                    if (!players.ContainsKey(pid))
                    {
                        var go = Instantiate(playerPrefab);
                        players[pid] = go;
                    }
                }
            }
        }
        else if (netMsg.type == "player_join")
        {
            var pid = netMsg.playerId;
            if (!players.ContainsKey(pid))
            {
                var go = Instantiate(playerPrefab);
                players[pid] = go;
                CameraFollow.Instance.setTarget(go.transform);
            }
            if (string.IsNullOrEmpty(GameManager.Instance.myGUID) || GameManager.Instance.myGUID == null)
            {
                GameManager.Instance.myGUID = pid;
            }
        }
        else if (netMsg.type == "move")
        {
            var pid = netMsg.playerId;
            if (players.ContainsKey(pid))
            {
                players[pid].transform.position = new Vector3(netMsg.x, netMsg.y, 0);
            }
        }
        else if (netMsg.type == "player_leave")
        {
            var pid = netMsg.playerId;
            if (players.ContainsKey(pid))
            {
                Destroy(players[pid]);
                players.Remove(pid);
            }
        }
    }
}
