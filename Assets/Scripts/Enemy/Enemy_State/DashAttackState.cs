using UnityEngine;

/// <summary>
/// 돌진 상태
/// Player 방향으로 빠르게 돌진
/// 일정 거리 도달 또는 애니메이션 완료 시 IdleState로 복귀
/// </summary>
public class DashState : State
{
    private Animator animator;
    private Rigidbody rb;
    private bool hasStarted; // 애니메이션 시작 여부

    public DashState(IEnemy enemy) : base(enemy)
    {
        animator = enemy.Animator;
        rb = enemy.Rigidbody;
    }

    public override void Enter()
    {
        hasStarted = false; // 애니메이션 시작 플래그 리셋

        // 돌진 애니메이션 시작
        animator.SetTrigger(AnimationConstants.DASH_TRIGGER);
    }

    public override void Execute()
    {
        // ========== 1. 애니메이션 시작 대기 ==========
        if (!WaitForAnimationStart(animator, ref hasStarted, out AnimatorStateInfo stateInfo))
        {
            // 아직 돌진 애니메이션 시작 안 됨 (Move 중)
            return;
        }

        // ========== 2. Player 방향으로 돌진 ==========
        Vector3 direction = (enemy.Player.position - enemy.Transform.position).normalized;
        direction.y = 0; // 수평 방향만

        float speed = enemy.Data.dashSpeed;

        rb.velocity = new Vector3(
            direction.x * speed,
            rb.velocity.y, // Y축은 중력 유지
            direction.z * speed
        );

        // ========== 3. 완료 조건 체크 ==========
        float distanceToPlayer = Vector3.Distance(
            enemy.Transform.position,
            enemy.Player.position
        );

        bool reachedDistance = distanceToPlayer <= enemy.Data.dashStopDistance;
        bool animationFinished = stateInfo.IsTag(AnimationConstants.MOVEMENT_TAG);

        // 목표 거리 도달 AND 애니메이션 완료 = 돌진 완료
        if (reachedDistance && animationFinished)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            enemy.ChangeToIdle();
        }
    }

    public override void Exit()
    {
        // 이동 멈춤
        rb.velocity = new Vector3(0, rb.velocity.y, 0);

        // Trigger 리셋
        animator.ResetTrigger(AnimationConstants.DASH_TRIGGER);
    }
}