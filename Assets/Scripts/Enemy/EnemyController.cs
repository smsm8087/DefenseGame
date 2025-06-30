using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;
public enum EnemyState
{
    None,
    Move,
    Attack,
    Dead
}
public class EnemyController : MonoBehaviour
{
    public Animator animator;
    public string guid;
    public Vector3 serverPosition;
    public SpriteRenderer spriteRenderer;
    public GameObject outlineObj;

    private IEnemyState currentState;
    public EnemyMoveState moveState = new ();
    public EnemyAttackState attackState = new ();
    public EnemyDeadState deadState = new ();

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        serverPosition = transform.position;
        currentState = null;
    }

    void Start()
    {
        ChangeState(moveState);
    }

    void Update()
    {
        currentState?.Update(this);
    }

    public void ShowOutline(float duration = 3f)
    {
        if (outlineObj == null) return;

        outlineObj.SetActive(true);
        CancelInvoke(nameof(HideOutline));
        Invoke(nameof(HideOutline), duration);
    }

    private void HideOutline()
    {
        outlineObj.SetActive(false);
    }
    public void ChangeStateByEnum(EnemyState stateEnum)
    {
        switch (stateEnum)
        {
            case EnemyState.Move:
                ChangeState(moveState);
                break;
            case EnemyState.Attack:
                ChangeState(attackState);
                break;
            case EnemyState.Dead:
                ChangeState(deadState);
                break;
        }
    }
    public void ChangeState(IEnemyState newState)
    {
        if (currentState == newState) return;

        currentState?.Exit(this);
        currentState = newState;
        currentState?.Enter(this);
    }

    public void SyncFromServer(float posX)
    {
        if (currentState == null)
        {
            ChangeState(moveState);
        }
        serverPosition = new Vector3(posX, transform.position.y,transform.position.z);
    }

    public void SetGuid(string guid)
    {
        this.guid = guid;
    }
    public void OnAttackHit()
    {
        Debug.Log($"Enemy {guid} 공격 HIT!");

        // 서버에 공격 명중 메시지 전송
        var enemy_hit_msg = new NetMsg
        {
            type = "enemy_attack_hit",
            enemyId = guid,
        };
        NetworkManager.Instance.SendMsg(enemy_hit_msg);
    }
    public void OnDeadAction()
    {
        Debug.Log($"Enemy {guid} Dead");
        StartFadeOut();
    }
    public void StartFadeOut()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        CancelInvoke(nameof(HideOutline));
        outlineObj.SetActive(false);
        float elapsed = 0f;
        Color color = spriteRenderer.color;
        float duration = 0.5f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            spriteRenderer.color = new Color(color.r, color.g, color.b, alpha);
            yield return null;
        }
        gameObject.SetActive(false); // 완전히 사라지면 비활성화
        NetworkManager.Instance.RemoveEnemy(guid);
    }
}