using UnityEngine;

public class DashAttackState : State
{
    private Animator animator;
    private EnemyController enemy;
    private Vector3 targetPosition;
    private float dashSpeed = 10f;

    private bool isDashFinished = false;

    public DashAttackState(EnemyController enemy) : base(enemy)
    {
        this.enemy = enemy;
        animator = enemy.GetComponent<Animator>();
    }

    public override void Enter()
    {
        // 현재 플레이어 위치 기록
        targetPosition = enemy.player.position;

        // 돌진 공격 Trigger 발동
        animator.SetTrigger("DASHATTACK");
    }

    public override void Execute()
    {
        if (isDashFinished) return;

    Vector3 direction = (targetPosition - enemy.transform.position).normalized;
    enemy.transform.position += direction * dashSpeed * Time.deltaTime;

    if (Vector3.Distance(enemy.transform.position, targetPosition) < 0.5f)
    {
        isDashFinished = true;
        enemy.OnAttackFinished(); // 상태 결정은 EnemyController에 맡김
    }
    }

    public override void Exit()
{
    Vector3 lookDir = (enemy.player.position - enemy.transform.position).normalized;
    if (lookDir != Vector3.zero)
        enemy.transform.forward = lookDir;
}
}
