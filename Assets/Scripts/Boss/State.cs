public abstract class State
{
    protected EnemyController enemy;

    public State(EnemyController enemy)
    {
        this.enemy = enemy;
    }

    public virtual void Enter() { }    // 상태 시작
    public abstract void Execute();     // 매 프레임 실행
    public virtual void Exit() { }     // 상태 종료
}
