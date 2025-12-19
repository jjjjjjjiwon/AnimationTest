using UnityEngine;

public class AttackState : State
{
    private Animator animator;
    
    // 공격 BlendTree 이름 배열
    private string[] attackBlendTrees = new string[]
    {
        "1Attack",
        "2Attack",
    };

    private string currentAttackBlendTree;  // 현재 선택된 BlendTree

    public AttackState(EnemyController enemy) : base(enemy) 
    {
        animator = enemy.GetComponent<Animator>();
    }

    public override void Enter()
    {
        Debug.Log("AttackState Enter - 공격!");

        // 랜덤 BlendTree 선택
        int randomIndex = Random.Range(0, attackBlendTrees.Length);
        currentAttackBlendTree = attackBlendTrees[randomIndex];

        // Animator에서 BlendTree 실행 (0번 레이어, 0f 시작)
        animator.Play(currentAttackBlendTree, 0, 0f);
    }

    public override void Execute()
    {
        // BlendTree가 끝났는지 체크
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    }

    public override void Exit()
    {
        Debug.Log("AttackState Exit - 공격 종료");
    }
}
