
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpforce = 10f;
    void Update()
    {
        Move();
    }

    void Move()
    {
        float moveInput = InputManager.GetMoveInput();
        MovementHelper.Move(transform, moveInput, moveSpeed);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            MovementHelper.Jump(transform, jumpforce);
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