using UnityEngine;
using System.Collections.Generic;

public class RevivalCancelledHandler : INetworkMessageHandler
{
    private readonly Dictionary<string, GameObject> players;
    
    public string Type => "revival_cancelled";
    
    public RevivalCancelledHandler(Dictionary<string, GameObject> players)
    {
        this.players = players;
    }
    
    public void Handle(NetMsg msg)
    {
        // 부활 대상 플레이어 상태 초기화
        if (players.TryGetValue(msg.targetId, out GameObject playerObj))
        {
            BasePlayer player = playerObj.GetComponent<BasePlayer>();
            if (player != null)
            {
                player.SetRevivalState(false, "");
            }
        }
        
        //부활 이펙트 중단
        if (RevivalEffectManager.Instance != null)
        {
            RevivalEffectManager.Instance.StopRevivalEffect(msg.targetId);
            Debug.Log($"[RevivalCancelledHandler] 부활 이펙트 중단: {msg.targetId}");
        }
        
        // UI 업데이트
        if (RevivalUI.Instance != null)
        {
            RevivalUI.Instance.OnRevivalCancelled(msg.targetId, msg.reason);
        }
        
        Debug.Log($"[RevivalCancelledHandler] {msg.targetId} 부활 취소: {msg.reason}");
    }
}