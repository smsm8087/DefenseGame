using Enemy;
using UnityEngine;

public class EnemyAttackState : IEnemyState
{
    public void Enter(EnemyController enemy)
    {
        enemy.animator.Play("attack");
    }

    public void Update(EnemyController enemy)
    {
    }

    public void Exit(EnemyController enemy) { }
}