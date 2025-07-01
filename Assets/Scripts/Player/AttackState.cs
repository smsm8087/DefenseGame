using UnityEngine;

public class AttackState : PlayerState
{
    private float attackDuration = 0.5f;
    private float elapsedTime = 0f;

    public AttackState(BasePlayer player) : base(player) { }

    public override void Enter()
    {
        player._animator.Play(AnimationNames.Attack);
        player.SendAnimationMessage(AnimationNames.Attack);
        attackDuration = GetAnimationClipLength(AnimationNames.Attack);
    }
    private float GetAnimationClipLength(string clipName)
    {
        var clips = player._animator.runtimeAnimatorController.animationClips;

        foreach (var clip in clips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }

        Debug.LogWarning($"[AttackState] 애니메이션 클립 '{clipName}' 을 찾을 수 없습니다. 기본 0.5초 사용.");
        return 0.5f; // fallback
    }
    public override void Update()
    {
        elapsedTime += Time.deltaTime;

        // 애니메이션 끝나면 이전 상태로 복귀
        if (elapsedTime >= attackDuration)
        {
            // 이전 상태가 null이면 Idle로
            PlayerState targetState = player.GetPrevState() ?? player.idleState;
            player.ChangeState(targetState);
            elapsedTime = 0f;
        }
    }
    public override void Exit()
    {
    }
}