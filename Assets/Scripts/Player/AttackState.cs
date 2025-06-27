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
    }

    public override void Update()
    {
        elapsedTime += Time.deltaTime;

        // 애니메이션 끝나면 이전 상태로 복귀
        if (elapsedTime >= attackDuration)
        {
            // 이전 상태가 null이면 Idle로
            PlayerState targetState = player.GetPrevState() ?? player.idleState;
            player.ChangeState(targetState);
            elapsedTime = 0f;
        }
    }
    public override void Exit()
    {
    }
}