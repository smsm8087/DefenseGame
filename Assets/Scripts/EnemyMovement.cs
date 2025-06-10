using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    
    private Vector3 targetPosition;
    private bool hasTarget = false;

    void Start()
    {
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
        
        if (Vector2.Distance(transform.position, targetPosition) < 0.5f)
        {
            hasTarget = false;
            GameManager.Instance.gem.TakeDamage(5);
            Destroy(this.gameObject);
        }
        else
        {
            transform.Translate(direction * moveSpeed * Time.deltaTime, Space.World);
        }
    }
}