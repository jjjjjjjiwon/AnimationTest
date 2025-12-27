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
        IsFinished = false;
        hasStarted = false;

        Debug.Log("DashState Enter - 돌진!");
        animator.SetTrigger(AnimationConstants.DASH_TRIGGER);
    }

public override void Execute()
{
    // ========== 디버그 추가 ==========
    Debug.Log($"DashState Execute - hasStarted: {hasStarted}");
    
    if (!WaitForAnimationStart(animator, ref hasStarted, out AnimatorStateInfo stateInfo))
    {
        Debug.Log($"Waiting for animation start... Tag: {stateInfo.IsTag(AnimationConstants.MOVEMENT_TAG)}");
        return;
    }
    
    Debug.Log("대시 진행 중!");

    Vector3 direction = (enemy.Player.position - enemy.Transform.position).normalized;
    direction.y = 0;

    float speed = enemy.Data.dashSpeed;

    rb.velocity = new Vector3(
        direction.x * speed,
        rb.velocity.y,
        direction.z * speed
    );

    float distanceToPlayer = Vector3.Distance(
        enemy.Transform.position,
        enemy.Player.position
    );

    // ========== 디버그 추가 ==========
    Debug.Log($"Distance: {distanceToPlayer}, dashStopDistance: {enemy.Data.dashStopDistance}");

    bool reachedDistance = distanceToPlayer <= enemy.Data.dashStopDistance;
    bool animationFinished = stateInfo.IsTag(AnimationConstants.MOVEMENT_TAG);

    Debug.Log($"Reached: {reachedDistance}, AnimFinished: {animationFinished}");

    if (reachedDistance && animationFinished)
    {
        Debug.Log("대시 완료!");
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        Finish();
    }
}

    public override void Exit()
    {
        Debug.Log("DashState Exit - 돌진 종료");

        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        animator.ResetTrigger(AnimationConstants.DASH_TRIGGER);
    }
}