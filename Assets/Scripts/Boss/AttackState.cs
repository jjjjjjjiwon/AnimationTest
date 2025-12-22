using UnityEngine;

public class AttackState : State
{
    private Animator animator;

    // 공격 BlendTree 이름 배열
    private string[] comboTriggers = new string[]
    {
        "1ATTACK",
        "2ATTACK"
    };

    private string currentAttackBlendTree;  // 현재 선택된 BlendTree

    public AttackState(EnemyController enemy) : base(enemy)
    {
        animator = enemy.GetComponent<Animator>();
    }

    public override void Enter()
    {
        Debug.Log("AttackState Enter - 공격!");

        // 랜덤 콤보 Trigger 선택
        int randomIndex = Random.Range(0, comboTriggers.Length);
        string selectedTrigger = comboTriggers[randomIndex];

        // Trigger 발동
        animator.SetTrigger(selectedTrigger);
    }

public override void Execute()
{
    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    
    // Idle로 돌아왔으면 공격 끝
    if (stateInfo.IsName("Move"))
    {
        enemy.OnAttackFinished();  // ← Controller에게 알림!
    }
}

    public override void Exit()
    {
        Debug.Log("AttackState Exit - 공격 종료");
    }
}
