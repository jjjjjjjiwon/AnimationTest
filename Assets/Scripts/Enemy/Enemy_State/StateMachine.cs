using UnityEngine;

/// <summary>
/// State Machine
/// 현재 State를 관리하고 전환을 처리
/// justChanged 플래그로 State 전환 직후 Execute 스킵
/// </summary>
public class StateMachine
{
    /// <summary>현재 실행 중인 State</summary>
    public State CurrentState { get; private set; }

    /// <summary>
    /// State가 방금 전환되었는지 여부
    /// true면 다음 Update에서 Execute 스킵 (애니메이션 전환 대기)
    /// </summary>
    private bool justChanged;

    /// <summary>
    /// State 전환
    /// 이전 State Exit → 새 State Enter → justChanged 플래그 설정
    /// </summary>
    public void ChangeState(State newState)
    {
        if (CurrentState == newState) return;
        // 이전 State 종료
        CurrentState?.Exit();

        // 새 State로 전환
        CurrentState = newState;

        // 새 State 시작
        CurrentState?.Enter();

        // 전환 플래그 설정 (다음 Update에서 Execute 스킵)
        justChanged = true;
    }

    /// <summary>
    /// 현재 State의 Execute 실행
    /// FixedUpdate에서 호출됨
    /// State 전환 직후 1프레임은 스킵 (애니메이션 전환 대기)
    /// </summary>
    public void Update()
    {
        // State 전환 직후면 Execute 스킵
        if (justChanged)
        {
            justChanged = false; // 플래그 리셋
            return;
        }

        // 현재 State 실행
        CurrentState?.Execute();
    }
}