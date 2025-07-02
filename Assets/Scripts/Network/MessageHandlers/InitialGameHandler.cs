using System;
using UnityEngine;
using System.Collections.Generic;
using UI;

public class InitialGameHandler : INetworkMessageHandler
{
    public string Type => "initial_game";
    public void Handle(NetMsg msg)
    {
        int wave_id = msg.wave_id;
        GameManager.Instance.InitializeGame(wave_id);
    }
}