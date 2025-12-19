using UnityEngine;

public class ChaseState : State
{
    private Rigidbody rb;
    private Animator animator;
    private float moveSpeed = 5f;


    public ChaseState(EnemyController enemy) : base(enemy)
    {
        rb = enemy.GetComponent<Rigidbody>();
        animator = enemy.GetComponent<Animator>();
    }

    public override void Enter()
    {
        Debug.Log("ChaseState Enter - 추적 시작!");
        animator.SetFloat("SPEED", moveSpeed);  // 상태 시작 시 바로 달리기
    }

    public override void Execute()
    {
        // 플레이어 방향 계산
        Vector3 direction = (enemy.player.position - enemy.transform.position).normalized;
        direction.y = 0;

        // 이동
        rb.velocity = new Vector3(
            direction.x * moveSpeed,
            rb.velocity.y,
            direction.z * moveSpeed
        );

        // 회전
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemy.transform.rotation = Quaternion.Slerp(
                enemy.transform.rotation,
                targetRotation,
                0.1f
            );
        }

        // BlendTree 파라미터 설정
        animator.SetFloat("SPEED", moveSpeed);
    }

    public override void Exit()
    {
        Debug.Log("ChaseState Exit - 추적 종료");
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        animator.SetFloat("SPEED", 0);         // BlendTree 종료 시 정지
    }
}
