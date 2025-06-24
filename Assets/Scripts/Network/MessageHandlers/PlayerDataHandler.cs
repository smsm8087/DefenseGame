using UnityEngine;
using DataModels;

public class PlayerDataHandler : INetworkMessageHandler
{
    public string Type => "player_data_response";

    public void Handle(NetMsg msg)
    {
        if (msg.playerData == null)
        {
            Debug.LogWarning("[PlayerDataHandler] 플레이어 데이터가 null입니다.");
            return;
        }

        // PlayerData 파싱
        var playerData = new PlayerData
        {
            id = int.Parse(msg.playerData.id),
            job_type = msg.playerData.job_type,
            hp = msg.playerData.hp,
            ult_gauge = msg.playerData.ult_gauge,
            attack_power = msg.playerData.attack_power
        };

        // ProfileUI 찾아서 데이터 전달
        var profileUI = Object.FindFirstObjectByType<ProfileUI>();
        if (profileUI != null)
        {
            profileUI.OnPlayerDataReceived(playerData);
        }
        else
        {
            Debug.LogWarning("[PlayerDataHandler] ProfileUI를 찾을 수 없습니다.");
        }

        Debug.Log($"[PlayerDataHandler] 플레이어 데이터 처리 완료 - ID: {playerData.id}, 직업: {playerData.job_type}, HP: {playerData.hp}, ULT 증가량: {playerData.ult_gauge}");
    }
}

// 공격 성공 시 ULT 증가를 위한 핸들러
public class AttackSuccessHandler : INetworkMessageHandler
{
    public string Type => "attack_success";

    public void Handle(NetMsg msg)
    {
        // 공격 성공 시 ULT 게이지 증가
        AttackState.OnAttackSuccess();
        Debug.Log("[AttackSuccessHandler] 공격 성공 메시지 처리 완료");
    }
}