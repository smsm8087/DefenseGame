using UnityEngine;
using System.Collections.Generic;
public class UpdateUltGaugeHandler : INetworkMessageHandler
{
    private readonly ProfileUI profileUI;

    public string Type => "update_ult_gauge";

    public UpdateUltGaugeHandler(ProfileUI profileUI)
    {
        this.profileUI = profileUI;
    }

    public void Handle(NetMsg msg)
    {
        profileUI.UpdateUltGauge(msg.currentUlt,msg.maxUlt);
    }
}