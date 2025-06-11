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
                var playerObj = Instantiate(playerPrefab);
                //guid 할당
                players[pid] = playerObj;
                PlayerController playerCtrl = playerObj.GetComponent<PlayerController>();
                playerCtrl.playerGUID = pid;
                
                if (string.IsNullOrEmpty(GameManager.Instance.myGUID) || GameManager.Instance.myGUID == null)
                {
                    GameManager.Instance.myGUID = pid;
                    CameraFollow.Instance.setTarget(playerObj.transform);
                }
            }
        }
        else if (netMsg.type == "move")
        {
            var pid = netMsg.playerId;
            //내플레이어 이동은 내컴퓨터에서만 함. 서버에서 받는건 다른캐릭터들의 좌표
            if (GameManager.Instance.myGUID == pid) return;
            
            if (players.ContainsKey(pid))
            {
                Vector3 prevPos = players[pid].transform.position;
                Vector3 targetPos = new Vector3(netMsg.x, netMsg.y, 0);
                
                players[pid].transform.position = targetPos;
                float dx = targetPos.x - prevPos.x;
                if (Mathf.Abs(dx) > 0.01f)
                {
                    players[pid].GetComponent<SpriteRenderer>().flipX = dx > 0;
                }
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
