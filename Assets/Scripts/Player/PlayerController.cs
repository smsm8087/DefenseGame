
using System.Collections;
using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour
{
    public string playerGUID;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    private Rigidbody2D _rb;
    private bool _isGrounded = true;
    private bool _isRunning = false;
    private SpriteRenderer _sr;
    private Animator _animator;
    private Vector3 _lastSentPosition;

    //flip 조절용
    private bool isFacingRight = false;
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        //내플레이어만 업데이트. 다른 플레이어는 네트워크 매니저에서 업데이트.
        if (playerGUID != NetworkManager.Instance.MyGUID)  return;
        HandleInput();
        SendMoveToServer();
    }

    void SendMoveToServer()
    {
        //매 프레임마다 다른플레이어에게 내 좌표 전송
        
        var pos = transform.position;

        _lastSentPosition = pos;

        var moveMsg = new NetMsg
        {
            type = "move",
            playerId = NetworkManager.Instance.MyGUID,
            x = pos.x,
            y = pos.y,
            isJumping = !_isGrounded,
            isRunning = _isRunning,
        };
        NetworkManager.Instance.SendMsg(moveMsg);
    }
    void HandleInput()
    {
        float moveInput = InputManager.GetMoveInput();

        // 이동
        MovementHelper.Move(_rb, moveInput, moveSpeed);

        // 방향 설정
        _isRunning = Mathf.Abs(moveInput) > 0.01f; 
        if (_isRunning)
        {
            isFacingRight = moveInput > 0;
        }
        if (_sr)
        {
            _sr.flipX = isFacingRight;
        }
        
        // 점프
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            MovementHelper.Jump(_rb, jumpForce);
            _isGrounded = false;
        }

        // 애니메이터
        if (_animator)
        {
            _animator.SetFloat("isRunning", _isRunning ? 1.0f : 0.0f);
            _animator.SetBool("isJumping", !_isGrounded);
        }
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
    public static float GetMoveInput()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return Input.GetAxisRaw("Horizontal");
#elif UNITY_ANDROID || UNITY_IOS
        return 0f;
#else
        return 0f;
#endif
    }
}