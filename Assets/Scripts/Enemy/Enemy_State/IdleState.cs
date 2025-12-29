using UnityEngine;

/// <summary>
/// 대기 상태 (Hub State)
/// 실제 행동 없이 조건 체크만 하는 "통로" 역할
/// 모든 State는 완료 후 IdleState로 복귀
/// IdleState에서 조건에 맞는 다음 State 선택
/// </summary>
public class IdleState : State
{
    public IdleState(IEnemy enemy) : base(enemy) { }

    public override void Enter()
    {
        // 애니메이션 설정 안 함 (논리적 상태일 뿐)
        // 실제로는 Move 애니메이션이 재생 중
    }

    public override void Execute()
    {
        // 행동 없음
        // EnemyController.SelectNextState()에서 다음 State 선택됨
    }

    public override void Exit()
    {
        // 정리 작업 없음
    }
}