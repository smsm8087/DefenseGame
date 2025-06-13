using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Vector3 targetPosition;
    private bool hasTarget = false;

    private SpriteRenderer spriteRenderer;
    
    // 서버에서 주기적으로 받은 위치
    private Vector3 serverPosition;
    private float smoothing = 5f; // 동기화 보간 속도
    
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        serverPosition = transform.position;
    }

    void Update()
    {
        if (hasTarget)
        {
            // 서버 위치로 부드럽게 보간
            transform.position = Vector3.Lerp(transform.position, serverPosition, Time.deltaTime * smoothing);

            // 시선 방향 처리
            Vector2 diff = serverPosition - transform.position;
            if (Mathf.Abs(diff.x) > 0.01f)
            {
                GetComponent<SpriteRenderer>().flipX = diff.x < 0;
            }
        }
    }

    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
        hasTarget = true;
    }
    // 서버에서 위치를 받을 때 호출
    public void SyncFromServer(float x, float y)
    {
        serverPosition = new Vector3(x, y, 0);
    }
}