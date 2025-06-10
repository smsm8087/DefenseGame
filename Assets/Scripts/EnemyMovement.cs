using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    
    private Vector3 targetPosition;
    private bool hasTarget = false;
    private Rigidbody2D rb2D;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (hasTarget)
        {
            MoveTowardsTarget();
        }
    }

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
        hasTarget = true;
    }
    void MoveTowardsTarget()
    {
        Vector2 direction = (targetPosition - transform.position).normalized;
        if (rb2D == null) return;
        rb2D.linearVelocityX = direction.x * moveSpeed;
    }
}