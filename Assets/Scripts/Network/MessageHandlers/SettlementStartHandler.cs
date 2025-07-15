using System.Collections.Generic;
using DataModels;
using UI;
using UnityEngine;

namespace NativeWebSocket.MessageHandlers
{
    public class SettlementStartHandler : INetworkMessageHandler
    {
        public string Type => "settlement_start";
        private readonly System.Action<List<CardData>, float, int> onSettlementStart;

        public SettlementStartHandler(System.Action<List<CardData>, float, int> onSettlementStart)
        {
            this.onSettlementStart = onSettlementStart;
        }

        public void Handle(NetMsg msg)
        {
            if (NetworkManager.Instance.MyGUID != msg.playerId) return;

            // 내 플레이어가 죽었는지 확인
            var players = NetworkManager.Instance.GetPlayers();
            if (players != null && players.TryGetValue(NetworkManager.Instance.MyGUID, out GameObject myPlayerObj))
            {
                if (myPlayerObj != null)
                {
                    BasePlayer myPlayer = myPlayerObj.GetComponent<BasePlayer>();
                    if (myPlayer != null && myPlayer.isDead)
                    {
                        Debug.Log("[SettlementStartHandler] 사망 상태로 카드 선택 불가");
                        return;
                    }
                }
            }

            List<CardData> cards = msg.cards;
            float duration = msg.duration;
            int alivePlayerCount = msg.alivePlayerCount;
        
            onSettlementStart?.Invoke(cards, duration, alivePlayerCount);
        }
    }
}
