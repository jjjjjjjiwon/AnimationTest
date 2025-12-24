using UnityEngine;

public class DashState : State
{
    private Animator animator;
    private Rigidbody rb;
    private float dashTimer;
    
    public DashState(IEnemy enemy) : base(enemy)
    {
        animator = enemy.Animator;
        rb = enemy.Rigidbody;
    }

    public override void Enter()
    {
        Debug.Log("DashState Enter - 돌진!");
        
        animator.SetTrigger("DASHATTACK");
        
        // Data에서 가져오기
        dashTimer = enemy.Data.dashStopDistance ;
    }

    public override void Execute()
{
    Vector3 direction = (enemy.Player.position - enemy.Transform.position).normalized;
    direction.y = 0;
    
    float speed = enemy.Data.dashSpeed;
    
    rb.velocity = new Vector3(
        direction.x * speed,
        rb.velocity.y,
        direction.z * speed
    );
    
    // 플레이어와의 거리
    float distanceToPlayer = Vector3.Distance(
        enemy.Transform.position, 
        enemy.Player.position
    );
    
    // 설정된 거리 이내면 종료
if (distanceToPlayer <= enemy.Data.dashStopDistance)
{
    rb.velocity = new Vector3(0, rb.velocity.y, 0);  // 멈춤!
    enemy.OnAttackFinished();
    return;  // Execute 끝
}
}

public override void Exit()
{
    Debug.Log($"Dash Exit - Velocity before: {rb.velocity}");
    rb.velocity = new Vector3(0, rb.velocity.y, 0);
    Debug.Log($"Dash Exit - Velocity after: {rb.velocity}");
}

}