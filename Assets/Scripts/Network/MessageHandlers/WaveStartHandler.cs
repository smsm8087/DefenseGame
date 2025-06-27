using System;
using System.Collections;
using UnityEngine;
using UI;

public class WaveStartHandler : INetworkMessageHandler
{
    public string Type => "wave_start";
    private CenterText centerText;
    
    public WaveStartHandler(CenterText centerText)
    {
        this.centerText = centerText;
    }
    
    public void Handle(NetMsg msg)
    {
        // 웨이브 시작 토스트 메시지 표시
        string waveText = $"Wave {msg.wave}";
        centerText.UpdateText(-1, waveText);
        
    }
}