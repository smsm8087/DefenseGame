using System.Collections.Generic;
using DataModels;
using UI;
using UnityEngine;

namespace NativeWebSocket.MessageHandlers
{
    public class SettlementStartHandler : INetworkMessageHandler
    {
        public string Type => "settlement_start";
        private readonly System.Action<List<CardData>, int> onSettlementStart;

        public SettlementStartHandler(System.Action<List<CardData>, int> onSettlementStart)
        {
            this.onSettlementStart = onSettlementStart;
        }
        public void Handle(NetMsg msg)
        {
            if (NetworkManager.Instance.MyGUID != msg.playerId) return;
            List<CardData> cards = msg.cards;
            int duration = msg.duration;
            onSettlementStart?.Invoke(cards, duration);
        }
    }
}
