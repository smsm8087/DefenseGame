using UnityEngine;
public enum EnemyState
{
    None,
    Move,
    Attack,
    Dead
}
public class EnemyMovement : MonoBehaviour
{
    private Vector3 targetPosition;
    private bool hasTarget = false;

    private SpriteRenderer spriteRenderer;
    public Animator animator;    
    // 서버에서 주기적으로 받은 위치
    private Vector3 serverPosition;
    private float smoothing = 5f; // 동기화 보간 속도
    public string guid;
    public EnemyState state = EnemyState.None;
    
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        serverPosition = transform.position;
        animator = GetComponent<Animator>();
        changeState(EnemyState.Move);
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
        this.guid = guid;
    }
    // 서버에서 위치를 받을 때 호출
    public void SyncFromServer(float x)
    {
        hasTarget = true;
        serverPosition = new Vector3(x, transform.position.y, transform.position.z);
    }

    public void changeState(EnemyState newState)
    {
        state = newState;
        switch (newState)
        {
            case EnemyState.Move:
                animator.Play("Idle_Clip");
                break;
            case EnemyState.Attack:
                animator.Play("Attack_Clip");
                break;
            case EnemyState.Dead:
                animator.Play("Dead_Clip");
                AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
                foreach (var clip in clips)
                {
                    if (clip.name == "DUST MONSTER ATTACKED_Clip")
                    {
                        NetworkManager.Instance.RemoveEnemy(this.guid, clip.length);
                    }
                }
                break;
        }
    }
}