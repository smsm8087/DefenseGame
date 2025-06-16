using UnityEngine;
using System.Collections.Generic;
using UI;

public class CountDownHandler : INetworkMessageHandler
{
    public string Type => "countdown";
    private CenterText centerText;
    public CountDownHandler(CenterText centerText)
    {
        this.centerText =  centerText;
    }
    public void Handle(NetMsg msg)
    {
        //단순 카운트다운용 메시지 핸들러
        centerText.UpdateText(msg.countDown, msg.message);
    }
}