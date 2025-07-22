using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;
using UI;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class CreateRoomHandler : INetworkMessageHandler
{
    public string Type => "room_created";

    public void Handle(NetMsg msg)
    {
        //룸 생성 완료
        SceneLoader.Instance.LoadScene("CharacterSelectScene");
    }
}
public class JoinRoomHandler : INetworkMessageHandler
{
    public string Type => "room_joined";

    public void Handle(NetMsg msg)
    {
        //룸 참가 완료
        SceneLoader.Instance.LoadScene("CharacterSelectScene");
    }
}
public class StartGameHandler : INetworkMessageHandler
{
    public string Type => "started_game";
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
        });
    }
}
