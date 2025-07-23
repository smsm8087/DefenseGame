using Newtonsoft.Json;
using UnityEngine;


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
