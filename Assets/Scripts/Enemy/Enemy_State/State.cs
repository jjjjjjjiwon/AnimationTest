using UnityEngine;

/// <summary>
/// 모든 State의 기본 클래스
/// State는 "행동"만 담당하고, "판단"은 EnemyController가 담당
/// </summary>
public abstract class State
{
    protected IEnemy enemy;

    public State(IEnemy enemy)
    {
        this.enemy = enemy;
    }

    /// <summary>
    /// State 진입 시 1회 실행
    /// 애니메이션 설정, 초기화 등
    /// </summary>
    public abstract void Enter();

    /// <summary>
    /// State 실행 중 매 FixedUpdate마다 실행
    /// 실제 행동 로직 (이동, 공격 등)
    /// </summary>
    public abstract void Execute();

    /// <summary>
    /// State 종료 시 1회 실행
    /// 정리 작업 (velocity 리셋, trigger 리셋 등)
    /// </summary>
    public abstract void Exit();

    /// <summary>
    /// 애니메이션이 시작될 때까지 대기
    /// MOVEMENT_TAG가 아닌 애니메이션으로 전환되면 시작으로 판단 
    /// 프레임 문제로 같은 프레임에 실행 되면 문제가 되서 1~2 프레임뒤에 실행 되게
    /// </summary>
    protected bool WaitForAnimationStart(Animator animator, ref bool hasStarted, out AnimatorStateInfo stateInfo)
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!hasStarted)
        {
            // MOVEMENT_TAG가 아니면 애니메이션 시작됨
            if (!stateInfo.IsTag(AnimationConstants.MOVEMENT_TAG))
            {
                hasStarted = true;
            }
            return false; // 아직 대기 중
        }

        return true; // 시작됨
    }
}