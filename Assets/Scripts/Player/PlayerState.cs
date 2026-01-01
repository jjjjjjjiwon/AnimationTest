using UnityEngine;

/// <summary>
/// Player State 기본 클래스
/// 모든 Player State는 이 클래스를 상속
/// </summary>
public abstract class PlayerState
{
    /// <summary>
    /// PlayerController 참조
    /// State에서 Player의 컴포넌트와 데이터에 접근
    /// </summary>
    protected PlayerController player;

    /// <summary>
    /// State 생성자
    /// </summary>
    /// <param name="player">PlayerController 참조</param>
    public PlayerState(PlayerController player)
    {
        this.player = player;
    }

    /// <summary>
    /// State 진입 시 1회 실행
    /// 애니메이션 시작, 초기화 등
    /// </summary>
    public abstract void Enter();

    /// <summary>
    /// State 실행 중 매 FixedUpdate마다 실행
    /// 이동, 공격, 조건 체크 등
    /// </summary>
    public abstract void Execute();

    /// <summary>
    /// State 종료 시 1회 실행
    /// 정리 작업, 플래그 리셋 등
    /// </summary>
    public abstract void Exit();

    /// <summary>
    /// 애니메이션이 시작될 때까지 대기하는 헬퍼 메서드
    /// 애니메이션 전환 구간 스킵용
    /// </summary>
    /// <param name="animator">Animator</param>
    /// <param name="hasStarted">시작 여부 플래그 (ref)</param>
    /// <param name="stateInfo">현재 애니메이션 정보 (out)</param>
    /// <returns>애니메이션이 시작되었으면 true</returns>
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
