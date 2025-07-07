using UnityEngine;
using Enemy;
public class EnemyDeadState : IEnemyState
{
    public void Enter(EnemyController enemy)
    {
        enemy.animator.Play("dead");
    }

    public void Update(EnemyController enemy) { }

    public void Exit(EnemyController enemy) { }
}