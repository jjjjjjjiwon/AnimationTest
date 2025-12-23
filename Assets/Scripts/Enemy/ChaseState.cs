using UnityEngine;

public class ChaseState : State
{
    private Rigidbody rb;
    
    public ChaseState(IEnemy enemy) : base(enemy)  // ← IEnemy
    {
        rb = enemy.Rigidbody;  // ← enemy.GetComponent 대신
    }

    public override void Enter()
    {
        Debug.Log("ChaseState Enter - 추적 시작!");
    }

    public override void Execute()
    {
        Vector3 direction = (enemy.Player.position - enemy.Transform.position).normalized;
        direction.y = 0;
        
        float moveSpeed = enemy.Data.moveSpeed;  // ← data에서 가져오기
        
        rb.velocity = new Vector3(
            direction.x * moveSpeed,
            rb.velocity.y,
            direction.z * moveSpeed
        );
        
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemy.Transform.rotation = Quaternion.Slerp(
                enemy.Transform.rotation, 
                targetRotation, 
                0.1f
            );
        }
    }

    public override void Exit()
    {
        Debug.Log("ChaseState Exit - 추적 종료");
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
    }
}