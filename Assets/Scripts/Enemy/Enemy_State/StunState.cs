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
        IsFinished = false;
        hasStarted = false;
        stunTimer = 0f;

        Debug.Log("StunState Enter - 기절!");
        animator.SetTrigger(AnimationConstants.STUN_TRIGGER);
        enemy.Rigidbody.velocity = new Vector3(0, enemy.Rigidbody.velocity.y, 0);
    }

    public override void Execute()
    {
        // ========== out으로 stateInfo 받기 ==========
        if (!WaitForAnimationStart(animator, ref hasStarted, out AnimatorStateInfo stateInfo))
            return;

        enemy.Rigidbody.velocity = new Vector3(0, enemy.Rigidbody.velocity.y, 0);

        stunTimer += Time.deltaTime;

        bool timeFinished = stunTimer >= enemy.Data.stunDuration;
        bool animationFinished = stateInfo.IsTag(AnimationConstants.MOVEMENT_TAG);

        if (timeFinished && animationFinished)
        {
            Debug.Log("기절 완전히 종료!");
            Finish();
        }
    }

    public override void Exit()
    {
        Debug.Log("StunState Exit - 기절 해제");
        animator.ResetTrigger(AnimationConstants.STUN_TRIGGER);
    }
}