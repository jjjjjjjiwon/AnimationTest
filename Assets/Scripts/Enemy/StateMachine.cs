using UnityEngine;

public class StateMachine
{
    public State CurrentState { get; private set; }
    private bool justChanged;

    public void ChangeState(State newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState?.Enter();
        justChanged = true;
    }

    public void Update()
    {
        Debug.Log($"[StateMachine] Update called - CurrentState: {CurrentState?.GetType().Name}, justChanged: {justChanged}");
        
        if (justChanged)
        {
            justChanged = false;
            Debug.Log("[StateMachine] Skipping Execute due to justChanged");
            return;
        }

        Debug.Log($"[StateMachine] Executing {CurrentState?.GetType().Name}");
        CurrentState?.Execute();
    }
}