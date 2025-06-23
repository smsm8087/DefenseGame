using UnityEngine;

public class AttackState : PlayerState
{
    private float attackDuration = 0.5f;
    private float elapsedTime = 0f;

    public AttackState(PlayerController player) : base(player) { }

    public override void Enter()
    {
        player._animator.Play("ATTACK_Clip");
        player.SendAnimationMessage("ATTACK_Clip");
    }
    public override void Update()
    {
        elapsedTime += Time.deltaTime;

        // 애니메이션 끝나면 IdleState 복귀
        if (elapsedTime >= attackDuration)
        {
            player.ChangeState(player.GetPrevState());
            elapsedTime = 0f;
        }
        // 디버그용 히트박스 시각화
        var target = player.transform.Find("AttackRanageCollider");
        BoxCollider2D box = target.GetComponent<BoxCollider2D>();
        if (box != null)
        {
            Vector3 center = target.transform.position + (Vector3)box.offset;
            Vector3 size = new Vector3(box.size.x * target.transform.lossyScale.x, box.size.y * target.transform.lossyScale.y, 1f);

            Vector3 topLeft = center + new Vector3(-size.x / 2f, size.y / 2f, 0);
            Vector3 topRight = center + new Vector3(size.x / 2f, size.y / 2f, 0);
            Vector3 bottomLeft = center + new Vector3(-size.x / 2f, -size.y / 2f, 0);
            Vector3 bottomRight = center + new Vector3(size.x / 2f, -size.y / 2f, 0);

            Debug.DrawLine(topLeft, topRight, Color.red);
            Debug.DrawLine(topRight, bottomRight, Color.red);
            Debug.DrawLine(bottomRight, bottomLeft, Color.red);
            Debug.DrawLine(bottomLeft, topLeft, Color.red);
        }
    }

    public override void Exit()
    {
        // 필요 시 처리
    }
}