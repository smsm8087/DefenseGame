using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 모든 플레이어 직업의 공통 기능을 담당하는 기본 클래스.
/// 서버에서 받은 PlayerInfo 데이터를 기반으로 능력치를 설정하며,
/// FSM 상태 관리, 애니메이션 전송, 공격 메시지 처리 등 공통 동작을 포함.
/// </summary>
public abstract class BasePlayer : MonoBehaviour
{
    [Header("Player Info")]
    public string playerGUID;
    public string job_type;

    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] public float jumpForce = 10f;

    [Header("Battle Stats")]
    public float attackPower;
    public float attackSpeed;
    public int critChance;
    public int critDamage;
    public int currentHp;
    public int maxHp;
    public float currentUlt;
    public float currentUltGauge;
    public List<int> cardIds = new();

    [Header("Unity Components")]
    public Rigidbody2D _rb;
    public SpriteRenderer _sr;
    public Animator _animator;

    [Header("Ground Check")]
    public bool _isGrounded = true;

    [Header("Attack Range")]
    public Transform attackRangeTransform;
    public BoxCollider2D attackRangeCollider;

    // FSM
    protected PlayerState currentState;
    protected PlayerState prevState;
    public IdleState idleState { get; private set; }
    public MoveState moveState { get; private set; }
    public JumpState jumpState { get; private set; }
    public AttackState attackState { get; private set; }

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

        var attackRangeObj = transform.Find("AttackRanageCollider");
        if (attackRangeObj)
        {
            attackRangeTransform = attackRangeObj.transform;
            attackRangeCollider = attackRangeObj.GetComponent<BoxCollider2D>();    
        }
    }

    protected virtual void Start()
    {
        idleState = new IdleState(this);
        moveState = new MoveState(this);
        jumpState = new JumpState(this);
        attackState = new AttackState(this);

        ChangeState(idleState);
    }

    protected virtual void Update()
    {
        // 본인 캐릭터일 때만 상태 업데이트 및 서버로 위치 전송
        if (!IsMyPlayer) return;

        SendMoveToServer();
        currentState?.Update();
    }

    public virtual void ChangeState(PlayerState newState)
    {
        currentState?.Exit();
        prevState = currentState;
        currentState = newState;
        currentState.Enter();
    }

    public PlayerState GetCurrentState() => currentState;
    public PlayerState GetPrevState() => prevState;

    /// <summary>
    /// 서버에서 받은 PlayerInfo를 바탕으로 능력치 적용
    /// </summary>
    public virtual void ApplyPlayerInfo(PlayerInfo info)
    {
        playerGUID = info.id;
        job_type = info.job_type;
        moveSpeed = info.currentMoveSpeed;
        jumpForce = 10f; // 필요 시 직업별 기본값 설정 가능

        attackPower = info.currentAttack;
        attackSpeed = info.currentAttackSpeed;
        critChance = info.currentCriPct;
        critDamage = info.currentCriDmg;

        currentHp = info.currentHp;
        maxHp = info.currentMaxHp;
        currentUlt = info.currentUlt;
        currentUltGauge = info.currentUltGauge;

        cardIds = new List<int>(info.cardIds);
    }

    public float GetMoveSpeed() => moveSpeed;

    public virtual void SendAnimationMessage(string animation)
    {
        var animationMsg = new NetMsg
        {
            type = "player_animation",
            playerId = NetworkManager.Instance.MyGUID,
            animation = animation
        };
        NetworkManager.Instance.SendMsg(animationMsg);
    }

    private void SendMoveToServer()
    {
        var pos = transform.position;

        var moveMsg = new NetMsg
        {
            type = "move",
            playerId = NetworkManager.Instance.MyGUID,
            x = pos.x,
            y = pos.y,
        };

        NetworkManager.Instance.SendMsg(moveMsg);
    }

    public virtual void OnSendAttackRequest()
    {
        if (!IsMyPlayer) return;
        NetMsg attackMsg = AttackBoxHelper.BuildAttackMessage(this);
        NetworkManager.Instance.SendMsg(attackMsg);
    }

    protected virtual void OnCollisionEnter2D(Collision2D col)
    {
        if (col.contacts[0].normal.y > 0.5f)
        {
            _isGrounded = true;
        }
    }
    /// <summary>
    /// 점프 중 공격이 가능한 직업인지 여부
    /// </summary>
    public virtual bool CanAttackWhileJumping => true;
    public virtual bool IsMyPlayer
    {
        get => playerGUID == NetworkManager.Instance.MyGUID;
    }
}