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
            if (RoomSession.RoomInfos.Contains(roomInfo)) continue;
            RoomSession.AddUser(roomInfo.playerId, roomInfo.nickName);
        }
        callback?.Invoke();
    }
}
