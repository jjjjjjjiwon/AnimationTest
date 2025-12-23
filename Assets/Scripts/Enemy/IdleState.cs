using UnityEngine;

public class IdleState : State
{
    public IdleState(IEnemy enemy) : base(enemy) { }  // ← 수정

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