using UI;
using System;
using UnityEngine;

public class GameResultHandler : INetworkMessageHandler
{
    private CenterText centerText;
    public string Type => "game_result";

    public GameResultHandler(CenterText centerText)
    {
        this.centerText = centerText;
    }

    public void Handle(NetMsg msg)
    {
        //TODO: result_Type -> clear | gameover
        //ui 띄워주고 액션 시작
        centerText.UpdateText(-1, msg.result_type);
        GameManager.Instance.PauseGame();
    }
}