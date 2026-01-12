using UnityEngine;

public class PlayerFinisherState : PlayerState
{
    private Animator animator;
    private Rigidbody rb;
    private SocketManager socketManager;   // ← 변경!
    
    private readonly int isMovingHash = Animator.StringToHash("IsMoving");
    private bool animationStarted = false;
    
    public PlayerFinisherState(PlayerController player) : base(player)
    {
        animator = player.Animator;
        rb = player.Rigidbody;
        socketManager = player.SocketManager;
    }
    
    public override void Enter()
    {
        Debug.Log("PlayerFinisherState 진입");
        
        animator.SetBool(isMovingHash, false);
        rb.velocity = Vector3.zero;
        animationStarted = false;
        
        // 피니셔 애니메이션 재생
        animator.Play(AnimationConstants.FINISHER);
        
        Debug.Log("피니셔 시전!");
    }
    
    public override void Execute()
    {
        if (!WaitForAnimationStart(animator, ref animationStarted, out var stateInfo))
            return;
        
        if (stateInfo.normalizedTime >= 0.95f)
        {
            Debug.Log("피니셔 완료! Idle로");
            socketManager.ResetCombo();  // ← 변경!
            player.StateMachine.ChangeState(player.IdleState);
        }
    }
    
    public override void Exit()
    {
        Debug.Log("PlayerFinisherState 종료");
    }
}
