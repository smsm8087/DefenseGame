using UnityEngine;

public class AttackState : PlayerState
{
    private float attackDuration = 0.5f;
    private float elapsedTime = 0f;

    public AttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player._animator.Play("ATTACK_Clip");
        player.SendAnimationMessage("ATTACK_Clip");

        // 공격 판정 타이밍 → 바로 보내기
        Vector3 localPos = player.attackRangeTransform.localPosition;
        localPos.x = Mathf.Abs(localPos.x) * (player._sr.flipX ? 1f: -1f);
        player.attackRangeTransform.localPosition = localPos;
        
        player.SendAttackRequest();
    }

    public override void Update()
    {
        elapsedTime += Time.deltaTime;

        // 애니메이션 끝나면 IdleState 복귀
        if (elapsedTime >= attackDuration)
        {
            player.ChangeState(player.GetPrevState());
            elapsedTime = 0f;
        }
    }

    public override void Exit()
    {
        // 필요 시 처리
    }
}