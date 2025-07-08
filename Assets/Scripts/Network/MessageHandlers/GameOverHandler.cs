using UI;
using System;
using UnityEngine;

public class GameOverHandler : INetworkMessageHandler
{
    private CenterText centerText;
    public string Type => "game_over";

    public GameOverHandler(CenterText centerText)
    {
        this.centerText = centerText;
    }

    public void Handle(NetMsg msg)
    {
        centerText.UpdateText(-1, msg.message);
        GameManager.Instance.PauseGame();
    }
}