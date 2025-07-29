using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;


public class StartGameHandler : INetworkMessageHandler
{
    private readonly Action callback;
    public string Type => "started_game";

    public StartGameHandler(Action callback)
    {
        this.callback = callback;
    }
    public void Handle(NetMsg msg)
    {
        SceneLoader.Instance.LoadScene("IngameScene", () =>
        {
            //IngameScene이 로드 되었으므로
            var message = new
            {
                type = "scene_loaded",
                playerId = UserSession.UserId,
                roomCode = RoomSession.RoomCode,
            };
            string json = JsonConvert.SerializeObject(message);
            WebSocketClient.Instance.Send(json);
            callback?.Invoke();
        });
    }
}
public class CreateRoomHandler : INetworkMessageHandler
{
    public string Type => "room_created";
    public void Handle(NetMsg msg)
    {
        SceneLoader.Instance.LoadScene("CharacterSelectScene", () =>
        {
            Debug.Log($"방 생성 성공! 코드: {RoomSession.RoomCode}");
        }); 
    }
}
public class JoinRoomHandler : INetworkMessageHandler
{
    public string Type => "room_joined";
    public void Handle(NetMsg msg)
    {
        SceneLoader.Instance.LoadScene("CharacterSelectScene", () =>
        {
            Debug.Log($"입장 성공! 코드: {RoomSession.RoomCode}");
        }); 
    }
}
public class RoomInfoHandler : INetworkMessageHandler
{
    public string Type => "room_info";
    private readonly Action callback; 
    public RoomInfoHandler(Action callback)
    {
        this.callback = callback;
    }

    public void Handle(NetMsg msg)
    {
        List<RoomInfo> RoomInfos = msg.RoomInfos;
        foreach (var roomInfo in RoomInfos)
        {
            if (RoomSession.RoomInfos.Find(x=> x.playerId == roomInfo.playerId) != null) continue;
            RoomSession.AddUser(roomInfo.playerId, roomInfo.nickName);
        }

        if (RoomInfos.Count != RoomSession.RoomInfos.Count)
        {
            //추가를 했는데도 맞지않으면 룸인포에서 삭제해야함.
            for (int i = 0; i < RoomSession.RoomInfos.Count; i++)
            {
                RoomInfo roomInfo = RoomSession.RoomInfos[i];
                if (RoomInfos.Find(x => x.playerId == roomInfo.playerId) != null) continue;
                //방을 나간 유저 룸인포에서 삭제
                RoomSession.RemoveUser(roomInfo.playerId);
            }
        }
        callback?.Invoke();
    }
}
public class OutRoomHandler : INetworkMessageHandler
{
    public string Type => "out_room";
    public void Handle(NetMsg msg)
    {
        var roomInfo = RoomSession.RoomInfos.Find(x => x.playerId == msg.playerId);
        if (roomInfo != null)
        {
            Debug.Log($"이전에 룸인포에서 삭제가 안된모양임. 확인 필요");
            RoomSession.RoomInfos.Remove(roomInfo);
        }
        SceneLoader.Instance.LoadScene("LobbyScene", () =>
        {
            Debug.Log($"방 나가기 성공!");
        }); 
    }
}
