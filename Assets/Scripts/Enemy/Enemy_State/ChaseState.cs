using UnityEngine;

/// <summary>
/// 추격 상태
/// Player를 향해 이동하며 추적
/// EnemyController가 매 프레임 탈출 조건 체크 (거리)
/// 조건 맞으면 Controller가 IdleState로 전환
/// </summary>
public class ChaseState : State
{
    private Rigidbody rb;
    private EnemyChaseStateJsonData chaseData; // JSON 데이터 저장용

    public ChaseState(IEnemy enemy, EnemyChaseStateJsonData data) : base(enemy)
    {
        rb = enemy.EnemyRigidbody;
        chaseData = data;
    }

    public override void Enter()
    {
        if (chaseData != null && !string.IsNullOrEmpty(chaseData.animation_Name))
        {
            // JSON에 적힌 이름으로 애니메이션 재생
            enemy.EnemyAnimator.Play(chaseData.animation_Name);
            Debug.Log($"[ChaseState] {chaseData.animation_Name} 애니메이션 재생 시작");
        }
    }

    public override void Execute()
    {
        if (enemy.Player == null) return;

        // ========== Player 방향 계산 ==========
        Vector3 direction = (enemy.Player.position - enemy.EnemyTransform.position).normalized;
        direction.y = 0; // 수평 방향만

        float moveSpeed = chaseData.chaseSpeed;

        // ========== Rigidbody로 이동 ==========
        rb.velocity = new Vector3(
            direction.x * moveSpeed,
            rb.velocity.y, // Y축은 중력 유지
            direction.z * moveSpeed
        );

        // ========== Player 방향으로 회전 ==========
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemy.EnemyTransform.rotation = Quaternion.Slerp(
                enemy.EnemyTransform.rotation,
                targetRotation,
                0.1f // 부드러운 회전
            );
        }

        // 탈출 조건은 EnemyController.CheckChaseExit()에서 체크
        // 이 State는 "추격"만 담당
    }

    public override void Exit()
    {
        // 이동 멈춤
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
    }
}