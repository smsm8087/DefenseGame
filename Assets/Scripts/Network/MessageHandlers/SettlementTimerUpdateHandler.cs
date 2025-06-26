using System;
using UnityEngine;
using System.Collections.Generic;
using UI;

public class SettlementTimerUpdateHandler : INetworkMessageHandler
{
    public string Type => "settlement_timer_update";
    CenterText centerText;
    public SettlementTimerUpdateHandler(CenterText centerText)
    {
        this.centerText =  centerText;
    }
    public void Handle(NetMsg msg)
    {
        float duration = msg.duration;
        UIManager.Instance.UpdateSettlementTimer(duration);
        centerText.UpdateText(-1, msg.isReady ? string.Empty : "다른 플레이어의 선택을 기다리는 중");
    }
}