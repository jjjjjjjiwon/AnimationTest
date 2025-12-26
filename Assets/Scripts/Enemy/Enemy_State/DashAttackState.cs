using UnityEngine;

public class DashState : State
{
    private Animator animator;
    private Rigidbody rb;
    private bool hasStarted = false;

    public DashState(IEnemy enemy) : base(enemy)
    {
        animator = enemy.Animator;
        rb = enemy.Rigidbody;
    }

    public override void Enter()
    {
        Debug.Log("DashState Enter - 돌진!");
        
        hasStarted = false;
        animator.SetTrigger("DASHATTACK");
    }

    public override void Execute()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        // ========== Animator 전환 대기 ==========
        if (!hasStarted)
        {
            if (!stateInfo.IsTag("Movement"))
            {
                hasStarted = true;
                Debug.Log("대시 애니메이션 시작!");
            }
            return;
        }
        
        // ========== 대시 진행 ==========
        Vector3 direction = (enemy.Player.position - enemy.Transform.position).normalized;
        direction.y = 0;
        
        float speed = enemy.Data.dashSpeed;
        
        rb.velocity = new Vector3(
            direction.x * speed,
            rb.velocity.y,
            direction.z * speed
        );
        
        // ========== 종료 조건 체크 ==========
        float distanceToPlayer = Vector3.Distance(enemy.Transform.position, enemy.Player.position);
        
        // 1. 거리 도달
        bool reachedDistance = distanceToPlayer <= enemy.Data.dashStopDistance;
        
        // 2. 애니메이션 끝
        bool animationFinished = stateInfo.IsTag("Movement");
        
        // 둘 중 하나라도 만족하면 종료
        if (reachedDistance && animationFinished)
        {
            Debug.Log($"대시 완료! (거리: {reachedDistance}, 애니: {animationFinished})");
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            enemy.OnAttackFinished();
        }
    }

    public override void Exit()
    {
        Debug.Log("DashState Exit - 돌진 종료");
        
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        animator.ResetTrigger("DASHATTACK");
    }
}