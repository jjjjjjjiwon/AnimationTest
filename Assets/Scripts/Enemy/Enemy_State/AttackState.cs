using UnityEngine;

/// <summary>
/// 공격 상태
/// 랜덤으로 선택된 공격 애니메이션 재생
/// 루트 모션을 허용하여 애니메이션에 따라 제자리 또는 전진 공격 가능
/// 애니메이션 완료 후 IdleState로 복귀
/// </summary>
public class AttackState : State
{
    private Animator animator;
    private bool hasStarted; // 애니메이션 시작 여부

    public AttackState(IEnemy enemy) : base(enemy)
    {
        animator = enemy.Animator;
    }

    public override void Enter()
    {
        hasStarted = false; // 애니메이션 시작 플래그 리셋

        // ========== 랜덤 공격 선택 ==========
        if (enemy.Data.enabledAttacks.Count > 0)
        {
            int randomIndex = Random.Range(0, enemy.Data.enabledAttacks.Count);
            string attackTrigger = enemy.Data.enabledAttacks[randomIndex];

            animator.SetTrigger(attackTrigger);
        }

        // 초기 velocity 리셋 (이전 상태의 이동 제거)
        enemy.Rigidbody.velocity = new Vector3(0, enemy.Rigidbody.velocity.y, 0);
    }

    public override void Execute()
    {
        // ========== 루트 모션 허용 ==========
        // velocity 설정 안 함!
        // 제자리 공격: 루트 모션 없음 → 안 움직임
        // 돌진 공격: 루트 모션 있음 → 앞으로 전진

        // ========== 1. 애니메이션 시작 대기 ==========
        if (!WaitForAnimationStart(animator, ref hasStarted, out AnimatorStateInfo stateInfo))
        {
            // 아직 공격 애니메이션 시작 안 됨 (Move 중)
            return;
        }

        // ========== 2. 애니메이션 완료 체크 ==========
        if (stateInfo.IsTag(AnimationConstants.MOVEMENT_TAG))
        {
            // Move로 복귀 = 공격 완료!
            enemy.ChangeToIdle();
        }

        // 공격 진행 중 (ATTACK_TAG)
    }

    public override void Exit()
    {
        // 모든 공격 Trigger 리셋 (다음 공격을 위해)
        foreach (string attackTrigger in enemy.Data.enabledAttacks)
        {
            animator.ResetTrigger(attackTrigger);
        }
    }
}