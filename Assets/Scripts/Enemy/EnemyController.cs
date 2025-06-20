using Enemy;
using UnityEngine;
public enum EnemyState
{
    None,
    Move,
    Attack,
    Dead
}
public class EnemyController : MonoBehaviour
{
    public Animator animator;
    public string guid;
    public Vector3 serverPosition;
    public SpriteRenderer spriteRenderer;

    private IEnemyState currentState;
    public EnemyMoveState moveState = new ();
    public EnemyAttackState attackState = new ();
    public EnemyDeadState deadState = new ();

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        serverPosition = transform.position;
        currentState = null;
    }

    void Start()
    {
        ChangeState(moveState);
    }

    void Update()
    {
        currentState?.Update(this);
    }
    public void ChangeStateByEnum(EnemyState stateEnum)
    {
        switch (stateEnum)
        {
            case EnemyState.Move:
                ChangeState(moveState);
                break;
            case EnemyState.Attack:
                ChangeState(attackState);
                break;
            case EnemyState.Dead:
                ChangeState(deadState);
                break;
        }
    }
    public void ChangeState(IEnemyState newState)
    {
        if (currentState == newState) return;

        currentState?.Exit(this);
        currentState = newState;
        currentState?.Enter(this);
    }

    public void SyncFromServer(float posX)
    {
        if (currentState == null)
        {
            ChangeState(moveState);
        }
        serverPosition = new Vector3(posX, transform.position.y,transform.position.z);
    }

    public void SetGuid(string guid)
    {
        this.guid = guid;
    }
    public void OnAttackHit()
    {
        Debug.Log($"Enemy {guid} 공격 HIT!");

        // 서버에 공격 명중 메시지 전송
        var enemy_hit_msg = new NetMsg
        {
            type = "enemy_attack_hit",
            enemyId = guid,
        };
        NetworkManager.Instance.SendMsg(enemy_hit_msg);
    }
    public void OnDeadAction()
    {
        Debug.Log($"Enemy {guid} Dead");
        NetworkManager.Instance.RemoveEnemy(guid);
    }
}