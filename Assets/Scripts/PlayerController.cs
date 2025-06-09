
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;

    void Update()
    {
        Move();
    }

    void Move()
    {
        Vector2 moveInput = InputManager.GetMoveInput();
        MovementHelper.Move(transform, moveInput, moveSpeed);
    }
}

public static class MovementHelper
{
    public static void Move(Transform target, Vector2 direction, float speed)
    {
        if (direction.sqrMagnitude < 0.01f) return;

        Vector3 delta = new Vector3(direction.x, direction.y, 0) * speed * Time.deltaTime;
        target.position += delta;
    }
}
public static class InputManager
{
    public static Vector2 GetMoveInput()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
#elif UNITY_ANDROID || UNITY_IOS
#else
        return Vector2.zero;
#endif
    }
}