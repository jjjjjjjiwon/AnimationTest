using System.Threading;
using UnityEngine;

public class AttackState : State
{
    public AttackState(EnemyController enemy) : base(enemy) { }

    public override void Enter()
    {
        Debug.Log("AttackState Enter");
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {
        Debug.Log("AttackState Exit");
    }
}
