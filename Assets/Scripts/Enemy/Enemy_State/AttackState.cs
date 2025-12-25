using UnityEngine;
using System.Linq;

public class AttackState : State
{
    private Animator animator;
    private string[] comboTriggers;

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
    
    Debug.Log($"Current State: {stateInfo.shortNameHash}");
    
    // Move의 Hash 값 확인
    int moveHash = Animator.StringToHash("Move");
    Debug.Log($"Move Hash: {moveHash}");
    
    if (stateInfo.shortNameHash == moveHash)
    {
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