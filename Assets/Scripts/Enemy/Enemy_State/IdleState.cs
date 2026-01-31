using UnityEngine;

public class IdleState : State
{
    private EnemyIdleStateJsonData data;

    public IdleState(IEnemy enemy, EnemyIdleStateJsonData data) : base(enemy)
    {
        this.data = data;
        Debug.Log($"[IdleState] {data.animation_Name} ================================");

    }

    public override void Enter()
    {
        if (data != null && !string.IsNullOrEmpty(data.animation_Name))
        {
            // JSON에 적힌 이름으로 애니메이션 재생
            enemy.EnemyAnimator.Play(data.animation_Name);
            Debug.Log($"[IdleState] {data.animation_Name} 애니메이션 재생 시작");
        }

        Debug.Log($"[IdleState] {data.animation_Name} ================================");

    }

    public override void Execute()
    {
        // 매 프레임 컨트롤러에게 "다음 행동 결정해!"라고 신호 보냄 (통로 역할)
        enemy.SelectNextState();
    }

    public override void Exit() { }
}