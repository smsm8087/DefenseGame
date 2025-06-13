using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private Vector3 targetPosition;
    private bool hasTarget = false;

    private SpriteRenderer spriteRenderer;
    
    // 서버에서 주기적으로 받은 위치
    private Vector3 serverPosition;
    private float smoothing = 5f; // 동기화 보간 속도
    public string guid;
    
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
            if ((serverPosition - transform.position).sqrMagnitude > 0.01f)
            {
                transform.position = Vector3.Lerp(transform.position, serverPosition, Time.deltaTime * smoothing);
            }

            // 시선 방향 처리
            Vector2 diff = serverPosition - transform.position;
            if (Mathf.Abs(diff.x) > 0.01f)
            {
                spriteRenderer.flipX = diff.x < 0;
            }
        }
    }

    public void setEnemy(string guid)
    {
        hasTarget = true;
        this.guid = guid;
    }
    // 서버에서 위치를 받을 때 호출
    public void SyncFromServer(float x)
    {
        serverPosition = new Vector3(x, transform.position.y, transform.position.z);
    }
}