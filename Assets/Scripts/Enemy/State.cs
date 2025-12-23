public abstract class State
{
    protected IEnemy enemy;  // ← EnemyController → IEnemy

    public State(IEnemy enemy)  // ← 타입 변경
    {
        this.enemy = enemy;
    }

    public virtual void Enter() { }
    public abstract void Execute();
    public virtual void Exit() { }
}