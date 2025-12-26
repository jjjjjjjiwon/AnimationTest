using UnityEngine;

public class DeathState : State
{
    private Animator animator;
    private float deathTimer;
    private bool hasStarted = false;
    private bool hasDisabledCollision = false;

    public DeathState(IEnemy enemy) : base(enemy)
    {
        animator = enemy.Animator;
    }

    public override void Enter()
    {
        Debug.Log("DeathState Enter - 사망!");
        
        hasStarted = false;
        deathTimer = 0f;
        hasDisabledCollision = false;
        
        // 사망 애니메이션 Trigger
        animator.SetTrigger("DEATH");
        
        // 움직임 정지
        enemy.Rigidbody.velocity = Vector3.zero;
        enemy.Rigidbody.isKinematic = true;  // 물리 비활성화
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
                Debug.Log("사망 애니메이션 시작!");
            }
            return;
        }
        
        // ========== Collider 비활성화 (1회만) ==========
        if (!hasDisabledCollision)
        {
            hasDisabledCollision = true;
            
            // 모든 Collider 비활성화
            Collider[] colliders = enemy.Transform.GetComponentsInChildren<Collider>();
            foreach (var col in colliders)
            {
                col.enabled = false;
            }
            
            Debug.Log("Collider 비활성화!");
        }
        
        // ========== 시간 체크 ==========
        deathTimer += Time.deltaTime;
        
        if (deathTimer >= enemy.Data.deathDelay)
        {
            Debug.Log("오브젝트 제거!");
            Object.Destroy(enemy.Transform.gameObject);
        }
    }

    public override void Exit()
    {
        Debug.Log("DeathState Exit");
        // 사망 후엔 Exit 안 될 것 (오브젝트 제거됨)
    }
}