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

    public ChaseState(IEnemy enemy) : base(enemy)
    {
        rb = enemy.Rigidbody;
    }

    public override void Enter()
    {
        // 추격 시작 (애니메이션은 Move 그대로)
    }

    public override void Execute()
    {
        // ========== Player 방향 계산 ==========
        Vector3 direction = (enemy.Player.position - enemy.Transform.position).normalized;
        direction.y = 0; // 수평 방향만

        float moveSpeed = enemy.Data.moveSpeed;

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
            enemy.Transform.rotation = Quaternion.Slerp(
                enemy.Transform.rotation,
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