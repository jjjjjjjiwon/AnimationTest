using UnityEngine;

public class AttackState : State
{
    private Animator animator;
    private string[] comboTriggers;  // ← 초기화 제거

    public AttackState(IEnemy enemy) : base(enemy)
    {
        animator = enemy.Animator;
        comboTriggers = enemy.Data.enabledAttacks.ToArray();
    }

    public override void Enter()
    {
        Debug.Log("AttackState Enter - 공격!");

        int randomIndex = Random.Range(0, comboTriggers.Length);
        string selectedTrigger = comboTriggers[randomIndex];

        animator.SetTrigger(selectedTrigger);
    }

    public override void Execute()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        if (stateInfo.IsName("Move"))
        {
            enemy.OnAttackFinished();  // ← 그대로
        }
    }

    public override void Exit()
    {
        Debug.Log("AttackState Exit - 공격 종료");
    }
}