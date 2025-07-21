using UnityEngine;
using System.Collections.Generic;
using UI;
using UnityEngine.SceneManagement;

public class CreateRoomHandler : INetworkMessageHandler
{
    public string Type => "room_created";

    public void Handle(NetMsg msg)
    {
        //룸 생성 완료
        SceneManager.LoadScene("CharacterSelectScene");
    }
}
public class JoinRoomHandler : INetworkMessageHandler
{
    public string Type => "room_joined";

    public void Handle(NetMsg msg)
    {
        //룸 참가 완료
        SceneManager.LoadScene("CharacterSelectScene");
    }
}
public class StartGameHandler : INetworkMessageHandler
{
    public string Type => "started_game";

    public void Handle(NetMsg msg)
    {
        
    }
}
