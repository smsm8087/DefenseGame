
using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public string playerGUID;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public bool needMove = false;
    public Vector3 needMoveTargetPos;
    public float lerpSpeed = 10f;
    public float jumpAnimationDuration = 1.58f;

    private Rigidbody2D _rb;
    private bool _isGrounded = true;
    private SpriteRenderer _sr;
    private Animator _animator;
    private bool _isJumping = false;
    private Coroutine _jumpAnimationCoroutine;
    

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _sr = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (GameManager.Instance.myGUID == null) return;
        if (playerGUID != GameManager.Instance.myGUID) return;
    
        HandleInput();
    }

    void HandleInput()
    {
        float moveInput = InputManager.GetMoveInput();

        // 이동
        MovementHelper.Move(_rb, moveInput, moveSpeed);

        // 방향 플립
        if (_sr && Mathf.Abs(moveInput) > 0.01f)
            _sr.flipX = moveInput > 0;

        // 점프
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            Debug.Log("Jump input detected!"); // 디버깅
            MovementHelper.Jump(_rb, jumpForce);
            _isGrounded = false;
            
            // 점프 애니메이션 시작
            StartJumpAnimation();
        }

        // 애니메이터
        if (_animator)
        {
            _animator.SetFloat("isRunning", Mathf.Abs(moveInput));
            _animator.SetBool("isJumping", _isJumping);
        }

        // 서버 전송 (매 프레임 or 일정 간격 추천)
        var pos = transform.position;
        var moveMsg = new NetMsg
        {
            type = "move",
            playerId = GameManager.Instance.myGUID,
            x = pos.x,
            y = pos.y
        };
        NetworkManger.Instance.SendMsg(moveMsg);
    }

    private void StartJumpAnimation()
    {
        if (_jumpAnimationCoroutine != null)
        {
            StopCoroutine(_jumpAnimationCoroutine);
        }
        
        _jumpAnimationCoroutine = StartCoroutine(PlayJumpAnimation());
    }

    private IEnumerator PlayJumpAnimation()
    {
        _isJumping = true;
    
        if (_animator != null && _animator.runtimeAnimatorController != null)
        {
            // 현재 애니메이션 상태 확인
            AnimatorStateInfo currentState = _animator.GetCurrentAnimatorStateInfo(0);
        
            // 애니메이션 재생 (여러 방법 시도)
            try
            {
                // 이름으로 재생
                _animator.Play("jump_side", 0, 0f);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Animation play error: " + e.Message);
            }
        
            // 1프레임 대기 후 상태 확인
            yield return null;
        }
        else
        {
            Debug.LogError("Animator or Controller is null!");
        }
    
        yield return new WaitForSeconds(jumpAnimationDuration);
    
        if (_isGrounded)
        {
            _isJumping = false;
        }
    
        _jumpAnimationCoroutine = null;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
            
            // 점프 애니메이션이 완료되었거나 땅에 닿았을 때 점프 상태 해제
            if (_jumpAnimationCoroutine == null)
            {
                _isJumping = false;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            _isGrounded = false;
            Debug.Log("Left ground"); // 디버깅
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