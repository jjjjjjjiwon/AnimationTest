using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RushRushRushRushRushRushRushRushRushRushRushRush
/// </summary>
public class RushState : State
{
    private Animator animator;

    private bool hasStarted; // 애니메이션 시작 여부
    private bool isTeleport;


    public RushState(IEnemy enemy) : base(enemy)
    {
        animator = enemy.Animator;
    }

    public override void Enter()
    {
        hasStarted = false; // 애니메이션 시작 플래그 리셋
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {

    }
}
