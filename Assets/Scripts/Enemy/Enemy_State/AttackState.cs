using UnityEngine;
using System.Linq;

public class AttackState : State
{
    private Animator animator;
    private string[] comboTriggers;
    private bool hasStarted = false;

    public AttackState(IEnemy enemy) : base(enemy)
    {
        animator = enemy.Animator;
        comboTriggers = enemy.Data.enabledAttacks.ToArray();
    }

    public override void Enter()
    {
        Debug.Log("AttackState Enter - 공격!");
        
        hasStarted = false;

        int randomIndex = Random.Range(0, comboTriggers.Length);
        string selectedTrigger = comboTriggers[randomIndex];
        animator.SetTrigger(selectedTrigger);
    }

    public override void Execute()
    {
            Debug.Log("ATTACK EXECUTE");
    
    // ========== velocity 리셋 (추가!) ==========
    enemy.Rigidbody.velocity = new Vector3(0, enemy.Rigidbody.velocity.y, 0);
    
    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        // Animator 전환 대기
        if (!hasStarted)
        {
            // Movement Tag가 아니면 = 공격 시작
            if (!stateInfo.IsTag("Movement"))
            {
                hasStarted = true;
                Debug.Log("공격 애니메이션 시작!");
            }
            return;
        }
        
        // 공격 종료 체크
        // Movement Tag로 돌아왔으면 = 끝
        if (stateInfo.IsTag("Movement"))
        {
            Debug.Log("공격 완료!");
            enemy.OnAttackFinished();
        }
    }

    public override void Exit()
    {
        Debug.Log("AttackState Exit - 공격 종료");
        
        foreach (var trigger in comboTriggers)
        {
            animator.ResetTrigger(trigger);
        }
    }
}