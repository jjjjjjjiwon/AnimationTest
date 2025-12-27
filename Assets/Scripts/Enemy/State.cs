using UnityEngine;

public abstract class State
{
    protected IEnemy enemy;
    
    public bool IsFinished { get; protected set; } = true;
    // hasStarted 제거! ← 각 State가 자기 것 가져야 함

    public State(IEnemy enemy)
    {
        this.enemy = enemy;
    }

    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();
    
    protected void Finish()
    {
        IsFinished = true;
    }
    
    // ========== 수정: hasStarted를 매개변수로 ==========
    protected bool WaitForAnimationStart(Animator animator, ref bool hasStarted, out AnimatorStateInfo stateInfo)
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        
        if (!hasStarted)
        {
            if (!stateInfo.IsTag(AnimationConstants.MOVEMENT_TAG))
            {
                hasStarted = true;
            }
            return false;
        }
        
        return true;
    }
}