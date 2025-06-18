using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player._animator.Play("IDLE_Clip");
        player.SendAnimationMessage("IDLE_Clip"); 
    }

    public override void Update()
    {
        float moveInput = InputManager.GetMoveInput();
        // 입력 체크 → 상태 전환
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.ChangeState(player.jumpState);
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            player.ChangeState(player.attackState);
        }
        else if (Mathf.Abs(moveInput) > 0.01f)
        {
            player.ChangeState(player.moveState);
        }
    }

    public override void Exit()
    {
        // 상태 종료 시 처리 (필요 없으면 비워도 됨)
    }
}