using UnityEngine;
using System.Collections.Generic;
using UI;

public class CountDownHandler : INetworkMessageHandler
{
    public string Type => "countdown";
    private CountDownText countdownText;
    public CountDownHandler(CountDownText countdownText)
    {
        this.countdownText =  countdownText;
    }
    public void Handle(NetMsg msg)
    {
        //단순 카운트다운용 메시지 핸들러
        this.countdownText.UpdateText(msg.countDown, msg.startMessage);
    }
}