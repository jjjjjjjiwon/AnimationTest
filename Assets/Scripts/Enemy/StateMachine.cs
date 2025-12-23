public class StateMachine
{
    public State CurrentState { get; private set; }

    public void ChangeState(State newState)
    {
        // 1. 현재 상태가 있으면 (null이 아니면)
        if (CurrentState != null)
            CurrentState.Exit();  // 종료 처리

        // 2. 새 상태로 교체
        CurrentState = newState;

        // 3. 새 상태 시작
        CurrentState.Enter();
    }

    public void Update()
    {
        if (CurrentState != null)
            CurrentState.Execute();
    }
}
