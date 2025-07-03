using UnityEngine;
using System.Collections.Generic;

public class PlayerUpdateHpHandler : INetworkMessageHandler
{
    public string Type => "player_update_hp";
    private readonly ProfileUI profileUI;

    public PlayerUpdateHpHandler(ProfileUI profileUI)
    {
        this.profileUI = profileUI;
    }
    public void Handle(NetMsg msg)
    {
        //프로필 ui 업데이트
        profileUI.UpdateHp(msg.playerInfo.currentHp, msg.playerInfo.currentMaxHp);
    }
}