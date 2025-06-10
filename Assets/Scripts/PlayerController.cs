
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpforce = 10f;
    
    private bool isJumping = false;
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Jump();
    }

    void Move()
    {
        float moveInput = InputManager.GetMoveInput();
        MovementHelper.Move(transform, moveInput, moveSpeed);
        SpriteRenderer sr =  gameObject.GetComponent<SpriteRenderer>();
        if (sr && Mathf.Abs(moveInput) > 0)
        {
            sr.flipX = moveInput > 0;
        }
        animator.SetFloat("isRunning", Mathf.Abs(moveInput) );
    }
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isJumping)
            {
                isJumping = true;
                MovementHelper.Jump(transform, jumpforce);
            }
        }
        animator.SetBool("isJumping", isJumping);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
    }

}

public static class MovementHelper
{
    public static void Move(Transform target, float direction, float speed)
    {
        Rigidbody2D rb2D = target.GetComponent<Rigidbody2D>();
        if (rb2D == null) return;
        rb2D.linearVelocityX = direction * speed;
    }
    public static void Jump(Transform target, float jumpForce)
    {
        Rigidbody2D rb2D = target.GetComponent<Rigidbody2D>();
        if (rb2D == null) return;
        rb2D.linearVelocityY = jumpForce;
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