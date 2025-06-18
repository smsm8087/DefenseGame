using System.Collections;
using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    public string playerGUID;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    public Rigidbody2D _rb;
    public SpriteRenderer _sr;
    public Animator _animator;
    
    public bool _isGrounded = true;
    
    //공격판정
    public Transform attackRangeTransform;
    public BoxCollider2D attackRangeCollider;
    
    //FSM
    private PlayerState currentState;
    private PlayerState prevState;
    public  IdleState idleState;
    public  MoveState moveState;
    public  JumpState jumpState;
    public  AttackState attackState;
    
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }
    private void Start()
    {
        idleState = new IdleState(this);
        moveState = new MoveState(this);
        jumpState = new JumpState(this);
        attackState = new AttackState(this);
    
        // 처음에는 IdleState 로 시작
        ChangeState(new IdleState(this));
    }
    void Update()
    {
        //내플레이어만 업데이트. 다른 플레이어는 네트워크 매니저에서 업데이트.
        if (playerGUID != NetworkManager.Instance.MyGUID)  return;
        SendMoveToServer();
        currentState?.Update();
    }
    public void ChangeState(PlayerState newState)
    {
        // 기존 상태 Exit 호출
        currentState?.Exit();

        prevState =  currentState;
        // 새 상태로 변경
        currentState = newState;

        // 새 상태 Enter 호출
        currentState.Enter();
    }
    public void SendAnimationMessage(string animation)
    {
        var animationMsg = new NetMsg
        {
            type = "player_animation",
            playerId = NetworkManager.Instance.MyGUID,
            animation = animation
        };
            
        NetworkManager.Instance.SendMsg(animationMsg);
    }
    void SendMoveToServer()
    {
        //매 프레임마다 다른플레이어에게 내 좌표 전송
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
    public void SendAttackRequest()
    {
        Vector2 centerWorldPos = attackRangeTransform.position;
        Vector3 lossyScale = attackRangeTransform.lossyScale;
        Vector2 size = attackRangeCollider.size;
        
        var attackMsg = new NetMsg
        {
            type = "player_attack",
            playerId = NetworkManager.Instance.MyGUID,
            attackBoxCenterX = centerWorldPos.x,
            attackBoxCenterY = centerWorldPos.y,
            attackBoxWidth = size.x * lossyScale.x,
            attackBoxHeight = size.y * lossyScale.y
        };

        NetworkManager.Instance.SendMsg(attackMsg);
    }
    public PlayerState GetCurrentState()
    {
        return currentState;
    }
    public PlayerState GetPrevState()
    {
        return prevState;
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.contacts[0].normal.y > 0.5f)
        {
            _isGrounded = true;
        }
    }
}

public static class MovementHelper
{
    public static void Move(Rigidbody2D rb, float direction, float speed)
    {
        Vector2 velocity = rb.linearVelocity;
        velocity.x = direction * speed;
        rb.linearVelocity = velocity;
    }

    public static void Jump(Rigidbody2D rb, float jumpForce)
    {
        Vector2 velocity = rb.linearVelocity;
        velocity.y = jumpForce;
        rb.linearVelocity = velocity;
    }
}

public static class InputManager
{
    public static FixedJoystick joystick;
    public static bool isJumpPressed = false;
    public static bool isAttackPressed = false;

    public static float GetMoveInput()
    {
        
        if (joystick != null && Mathf.Abs(joystick.Horizontal) > 0.01f)
        {
            return joystick.Horizontal;
        }

        
        return Input.GetAxisRaw("Horizontal");
    }

    public static bool GetJumpInput()
    {
        if (isJumpPressed)
        {
            isJumpPressed = false;
            return true;
        }

        return Input.GetKeyDown(KeyCode.Space);
    }

    public static bool GetAttackInput()
    {
        if (isAttackPressed)
        {
            isAttackPressed = false;
            return true;
        }

        return Input.GetKeyDown(KeyCode.Z);
    }
}
