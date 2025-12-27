using UnityEngine;

public class AttackState : State
{
    private Animator animator;
    private bool hasStarted = false;  // ← 다시 추가! (자기 것)

    public AttackState(IEnemy enemy) : base(enemy)
    {
        animator = enemy.Animator;
    }

    public override void Enter()
    {
        IsFinished = false;
        hasStarted = false;  // ← 자기 변수 리셋

        Debug.Log("AttackState Enter - 공격!");

        if (enemy.Data.enabledAttacks.Count > 0)
        {
            int randomIndex = Random.Range(0, enemy.Data.enabledAttacks.Count);
            string attackTrigger = enemy.Data.enabledAttacks[randomIndex];

            animator.SetTrigger(attackTrigger);
            Debug.Log($"공격 선택: {attackTrigger}");
        }

        enemy.Rigidbody.velocity = new Vector3(0, enemy.Rigidbody.velocity.y, 0);
    }

    public override void Execute()
    {
        Debug.Log("ATTACK EXECUTE");

        enemy.Rigidbody.velocity = new Vector3(0, enemy.Rigidbody.velocity.y, 0);

        // ========== ref 키워드로 자기 hasStarted 전달 ==========
        if (!WaitForAnimationStart(animator, ref hasStarted, out AnimatorStateInfo stateInfo))
            return;

        if (stateInfo.IsTag(AnimationConstants.MOVEMENT_TAG))
        {
            Debug.Log("공격 완료!");
            Finish();
        }
    }

    public override void Exit()
    {
        Debug.Log("AttackState Exit - 공격 종료");

        foreach (string attackTrigger in enemy.Data.enabledAttacks)
        {
            animator.ResetTrigger(attackTrigger);
        }
    }
}