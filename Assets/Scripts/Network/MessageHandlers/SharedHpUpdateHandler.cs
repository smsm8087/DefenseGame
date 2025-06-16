using UnityEngine;
using System.Collections.Generic;

public class SharedHpUpdateHandler : INetworkMessageHandler
{
    public string Type => "shared_hp_update";
    private SharedHpManager sharedHpManager;
    public SharedHpUpdateHandler(SharedHpManager sharedHpManager)
    {
        this.sharedHpManager = sharedHpManager;
    }

    public void Handle(NetMsg msg)
    {
        sharedHpManager.UpdateHPBar(msg.currentHp,msg.maxHp);
    }
}