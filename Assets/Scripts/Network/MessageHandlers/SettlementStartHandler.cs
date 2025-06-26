using System.Collections.Generic;
using DataModels;
using UI;
using UnityEngine;

namespace NativeWebSocket.MessageHandlers
{
    public class SettlementStartHandler : INetworkMessageHandler
    {
        public string Type => "settlement_start";
        private readonly System.Action<List<CardData>, float> onSettlementStart;

        public SettlementStartHandler(System.Action<List<CardData>, float> onSettlementStart)
        {
            this.onSettlementStart = onSettlementStart;
        }
        public void Handle(NetMsg msg)
        {
            if (NetworkManager.Instance.MyGUID != msg.playerId) return;
            List<CardData> cards = msg.cards;
            float duration = msg.duration;
            onSettlementStart?.Invoke(cards, duration);
        }
    }
}
