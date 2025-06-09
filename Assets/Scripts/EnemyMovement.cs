using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Transform hpTarget;
    
    private Vector3 targetPosition;
    private bool hasTarget = false;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 1f;
        }
        
        if (hpTarget != null)
        {
            SetTargetToHP();
        }
    }

    void Update()
    {
        if (hasTarget)
        {
            if (hpTarget != null)
            {
                targetPosition = hpTarget.position;
            }
            
            MoveTowardsTarget();
        }
    }

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
        hasTarget = true;
    }
    
    public void SetTargetToHP()
    {
        if (hpTarget != null)
        {
            targetPosition = hpTarget.position;
            hasTarget = true;
        }
    }

    void MoveTowardsTarget()
    {
        if (rb == null) return;
        
        Vector2 direction = (targetPosition - transform.position).normalized;
        
        if (direction.magnitude > 0.1f)
        {
            rb.linearVelocity = direction * moveSpeed;
        }
        
        if (Vector2.Distance(transform.position, targetPosition) < 0.5f)
        {
            hasTarget = false;
            rb.linearVelocity = Vector2.zero;
        }
    }
}