using UnityEngine;

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

    public static float GetMoveInput()
    {
        if (joystick != null && Mathf.Abs(joystick.Horizontal) > 0.01f)
        {
            return joystick.Horizontal;
        }
        return Input.GetAxisRaw("Horizontal");
    }
}