using UnityEngine;
using System.Collections.Generic;

public class RevivalCompletedHandler : INetworkMessageHandler
{
    private readonly Dictionary<string, GameObject> players;
    private readonly ProfileUI profileUI;
    
    public string Type => "revival_completed";
    
    public RevivalCompletedHandler(Dictionary<string, GameObject> players, ProfileUI profileUI)
    {
        this.players = players;
        this.profileUI = profileUI;
    }
    
    public void Handle(NetMsg msg)
    {
        // 플레이어 부활 처리
        if (players.TryGetValue(msg.targetId, out GameObject playerObj))
        {
            BasePlayer player = playerObj.GetComponent<BasePlayer>();
            if (player != null)
            {
                // 위치 설정
                playerObj.transform.position = new Vector3(msg.reviveX, msg.reviveY, 0);
        
                // 부활 처리
                player.Revive();
                player.currentHp = (int)msg.currentHp;
                player.maxHp = (int)msg.maxHp;
        
                // 무적 상태 시작
                player.StartInvulnerability(msg.invulnerabilityDuration);
        
                // 내 플레이어가 부활했다면 ProfileUI 업데이트
                if (msg.targetId == NetworkManager.Instance.MyGUID && profileUI != null)
                {
                    profileUI.UpdateHp((int)msg.currentHp, (int)msg.maxHp);
                    Debug.Log($"[RevivalCompletedHandler] ProfileUI HP 업데이트: {msg.currentHp}/{msg.maxHp}");
                }
        
                Debug.Log($"[RevivalCompletedHandler] {msg.targetId} 부활 완료");
            }
        }
        else
        {
            // UI 업데이트
            if (RevivalUI.Instance != null)
            {
                RevivalUI.Instance.OnRevivalCompleted(msg.targetId);
            }
        }
    }
}