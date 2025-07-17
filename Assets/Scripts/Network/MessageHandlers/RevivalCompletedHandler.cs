using UnityEngine;
using System.Collections;
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
                
                // 부활 이펙트 시작
                if (RevivalEffectManager.Instance != null)
                {
                    Vector3 effectPosition = playerObj.transform.position + Vector3.up * 1.4f;
                    RevivalEffectManager.Instance.StartRevivalEffect(msg.targetId, effectPosition);
                    Debug.Log($"[RevivalCompletedHandler] 부활 이펙트 시작: {msg.targetId}");
                }
                
                // 부활 처리
                NetworkManager.Instance.StartCoroutine(DelayedRevive(player, msg, 3f));
                
                // 내 플레이어라면 ProfileUI는 부활 후 업데이트됨
                Debug.Log($"[RevivalCompletedHandler] {msg.targetId} 부활 준비 완료, 3초 후 부활");
            }
        }
        
        // UI 업데이트
        if (RevivalUI.Instance != null)
        {
            RevivalUI.Instance.OnRevivalCompleted(msg.targetId);
        }
    }
    
    /// <summary>
    /// 지연 부활 처리
    /// </summary>
    private IEnumerator DelayedRevive(BasePlayer player, NetMsg msg, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (player != null && player.isDead)
        {
            player.Revive();
            player.currentHp = (int)msg.currentHp;
            player.maxHp = (int)msg.maxHp;
            player.StartInvulnerability(msg.invulnerabilityDuration);
            
            Debug.Log($"[RevivalCompletedHandler] {player.playerGUID} 타이머 부활 완료! HP: {msg.currentHp}/{msg.maxHp}");
        }
    }
}