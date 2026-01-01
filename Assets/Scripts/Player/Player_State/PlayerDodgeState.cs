using UnityEngine;

/// <summary>
/// Player 회피 상태
/// Space키로 회피 (루트 모션 사용)
/// 
/// 역할:
/// - 회피 애니메이션 재생 (루트 모션으로 이동)
/// - 애니메이션 종료 후 Idle로 복귀
/// - 회피 쿨타임은 PlayerController에서 관리
/// </summary>
public class PlayerDodgeState : PlayerState
{
    private Animator animator;
    private Rigidbody rb;
    
    // 애니메이션 파라미터
    private readonly int isMovingHash = Animator.StringToHash("IsMoving");
    
    // 애니메이션 시작 대기
    private bool animationStarted = false;
    
    public PlayerDodgeState(PlayerController player) : base(player)
    {
        animator = player.Animator;
        rb = player.Rigidbody;
    }
    
    public override void Enter()
    {
        Debug.Log("PlayerDodgeState 진입");
        
        // IsMoving = false (회피 중 이동 애니메이션 중지)
        animator.SetBool(isMovingHash, false);
        
        // 이동 정지 (루트 모션이 이동 담당)
        rb.velocity = Vector3.zero;
        
        // 애니메이션 시작 플래그 리셋
        animationStarted = false;
        
        // 회피 애니메이션 재생
        animator.Play(AnimationConstants.DODGE);
        
        Debug.Log("회피!");
    }
    
    public override void Execute()
    {
        // 애니메이션 시작 대기
        if (!WaitForAnimationStart(animator, ref animationStarted, out var stateInfo))
            return;
        
        // 애니메이션 종료 체크
        if (stateInfo.normalizedTime >= 0.95f)
        {
            Debug.Log("회피 완료! Idle로");
            player.StateMachine.ChangeState(player.IdleState);
        }
    }
    
    public override void Exit()
    {
        Debug.Log("PlayerDodgeState 종료");
    }
}