using System.Collections.Generic;
using DataModels;
using UI;
using UnityEngine;

namespace NativeWebSocket.MessageHandlers
{
    public class SettlementStartHandler : INetworkMessageHandler
    {
        public string Type => "settlement_start";
        CenterText textPrefab;

        public SettlementStartHandler(CenterText textPrefab)
        {
            this.textPrefab =  textPrefab;
        }
        public void Handle(NetMsg msg)
        {
            if (NetworkManager.Instance.MyGUID != msg.playerId) return;
            List<CardData> cards = msg.cards;
            int duration = msg.duration;
            Debug.Log($"settlement_start playerid : {msg.playerId} | cards : {cards} | duration : {duration}");
            textPrefab.startDurationAnimation(duration, msg.cards);
        }
    }
}
