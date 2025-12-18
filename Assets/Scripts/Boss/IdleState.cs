using UnityEngine;

public class IdleState : State
{
    public IdleState(EnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        Debug.Log("IdleState Enter");
    }

    public override void Execute()
    {
        // 대기 행동
    }

    public override void Exit()
    {
        Debug.Log("IdleState Exit");
    }
}
