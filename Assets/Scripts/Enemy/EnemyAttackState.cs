using Enemy;
using UnityEngine;

public class EnemyAttackState : IEnemyState
{
    public void Enter(EnemyController enemy)
    {
        enemy.animator.Play("Attack_Clip");

        // Animation Event or Coroutine 사용해서 서버에 AttackHit 송신
        // 예시: Animator Event 에서 NetworkManager.Instance.SendAttackHit(enemy.guid)
    }

    public void Update(EnemyController enemy)
    {
        // 보통 공격 상태는 대기
    }

    public void Exit(EnemyController enemy) { }
}