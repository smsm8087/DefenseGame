using UnityEngine;

public class AttackState : PlayerState
{
    private float attackDuration = 0.5f;
    private float elapsedTime = 0f;
    private bool hasProcessedAttack = false; // 공격 처리 여부 체크

    public AttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        Debug.Log($"[AttackState] Enter - 공격 상태 진입 시간: {Time.time:F3}");
        
        player._animator.Play("ATTACK_Clip");
        player.SendAnimationMessage("ATTACK_Clip");
    }

    // 서버에서 공격 성공 응답을 받았을 때 호출될 메서드
    public static void OnAttackSuccess(ProfileUI profileUI)
    {
        // ProfileUI 찾아서 ULT 게이지 증가
        profileUI.OnMonsterAttackSuccess();
    }

    public override void Update()
    {
        elapsedTime += Time.deltaTime;

        // 애니메이션 끝나면 이전 상태로 복귀
        if (elapsedTime >= attackDuration)
        {
            Debug.Log($"[AttackState] 공격 완료, 이전 상태로 복귀 - 시간: {Time.time:F3}");
            
            // 이전 상태가 null이면 Idle로
            PlayerState targetState = player.GetPrevState() ?? player.idleState;
            player.ChangeState(targetState);
            elapsedTime = 0f;
        }
        
        // 디버그용 히트박스 시각화
        DrawAttackBox();
    }

    private void DrawAttackBox()
    {
        if (player.attackRangeTransform == null || player.attackRangeCollider == null) return;
        
        Vector3 center = player.attackRangeTransform.position;
        Vector3 lossyScale = player.attackRangeTransform.lossyScale;
        Vector2 size = player.attackRangeCollider.size;
        Vector3 worldSize = new Vector3(size.x * lossyScale.x, size.y * lossyScale.y, 1f);

        Vector3 topLeft = center + new Vector3(-worldSize.x / 2f, worldSize.y / 2f, 0);
        Vector3 topRight = center + new Vector3(worldSize.x / 2f, worldSize.y / 2f, 0);
        Vector3 bottomLeft = center + new Vector3(-worldSize.x / 2f, -worldSize.y / 2f, 0);
        Vector3 bottomRight = center + new Vector3(worldSize.x / 2f, -worldSize.y / 2f, 0);

        Debug.DrawLine(topLeft, topRight, Color.red);
        Debug.DrawLine(topRight, bottomRight, Color.red);
        Debug.DrawLine(bottomRight, bottomLeft, Color.red);
        Debug.DrawLine(bottomLeft, topLeft, Color.red);
    }

    public override void Exit()
    {
        Debug.Log($"[AttackState] Exit - 공격 상태 종료 - 시간: {Time.time:F3}");
    }
}