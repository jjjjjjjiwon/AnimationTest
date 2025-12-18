using UnityEngine;

public class ChaseState : State
{
    private Rigidbody rb;
    private float moveSpeed = 5f;

    public ChaseState(EnemyController enemy) : base(enemy)
    {
        rb = enemy.GetComponent<Rigidbody>();
    }

    public override void Enter()
    {
        Debug.Log("ChaseState Enter - 추적 시작!");
    }

    public override void Execute()
    {
        // 플레이어 방향 계산
        Vector3 direction = (enemy.player.position - enemy.transform.position).normalized;
        direction.y = 0;  // 수평 이동만
        
        // 이동
        rb.velocity = new Vector3(
            direction.x * moveSpeed,
            rb.velocity.y,  // Y축 유지 (중력)
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
    }

    public override void Exit()
    {
        Debug.Log("ChaseState Exit - 추적 종료");
        // 이동 멈춤
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
    }
}
