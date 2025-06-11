
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

    private Rigidbody2D rb;
    private bool isGrounded = true;
    private SpriteRenderer sr;
    private Animator animator;
    

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (GameManager.Instance.myGUID == null) return;
        if (playerGUID != GameManager.Instance.myGUID) return;

        HandleInput();
        if (needMove)
        {
            MovePositionOterPlayer();
        }
    }

    public void MovePositionOterPlayer()
    {
        if ((transform.position - needMoveTargetPos).sqrMagnitude > 0.0001f)
        {
            transform.position = Vector3.Lerp(transform.position, needMoveTargetPos, lerpSpeed * Time.deltaTime);
        }
    }
    void HandleInput()
    {
        float moveInput = InputManager.GetMoveInput();

        // 이동
        MovementHelper.Move(rb, moveInput, moveSpeed);

        // 방향 플립
        if (sr && Mathf.Abs(moveInput) > 0.01f)
            sr.flipX = moveInput > 0;

        // 점프
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            MovementHelper.Jump(rb, jumpForce);
            isGrounded = false;
        }

        // 애니메이터
        if (animator)
        {
            animator.SetFloat("isRunning", Mathf.Abs(moveInput));
            animator.SetBool("isJumping", !isGrounded);
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

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
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
#else
        return Vector2.zero;
#endif
    }
}