using UnityEngine;

public class StunState : State
{
    private Animator animator;
    private float stunTimer;
    private bool hasStarted = false;

    public StunState(IEnemy enemy) : base(enemy)
    {
        animator = enemy.Animator;
    }

    public override void Enter()
    {
        Debug.Log("StunState Enter - 기절!");
        
        hasStarted = false;
        stunTimer = 0f;
        
        animator.SetTrigger("STUN");
        enemy.Rigidbody.velocity = new Vector3(0, enemy.Rigidbody.velocity.y, 0);
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
                Debug.Log("기절 애니메이션 시작!");
            }
            return;
        }
        
        // ========== 기절 유지 ==========
        enemy.Rigidbody.velocity = new Vector3(0, enemy.Rigidbody.velocity.y, 0);
        
        // ========== 종료 조건: 시간 AND 애니메이션 ==========
        stunTimer += Time.deltaTime;
        
        bool timeFinished = stunTimer >= enemy.Data.stunDuration;
        bool animationFinished = stateInfo.IsTag("Movement");
        
        // 둘 다 만족해야 종료
        if (timeFinished && animationFinished)
        {
            Debug.Log("기절 완전히 종료! (시간 + 애니메이션)");
            enemy.OnStunFinished();
        }
    }

    public override void Exit()
    {
        Debug.Log("StunState Exit - 기절 해제");
        animator.ResetTrigger("STUN");
    }
}