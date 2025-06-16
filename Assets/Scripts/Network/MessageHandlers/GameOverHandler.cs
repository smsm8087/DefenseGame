using UI;
using System;

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
        NetworkManager.Instance.TriggerGameOver();
    }
}