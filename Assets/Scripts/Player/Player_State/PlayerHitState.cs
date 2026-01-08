using UnityEngine;

/// <summary>
/// Player 피격/스턴 상태
/// 스턴 게이지 0이 되면 진입
/// 
/// 역할:
/// - 피격 애니메이션 재생
/// - 일정 시간 동안 조작 불가 (HandleInput에서 차단)
/// - 애니메이션 종료 후 스턴 게이지 회복
/// - Idle로 복귀
/// </summary>
public class PlayerHitState : PlayerState
{
    private Animator animator;
    private Rigidbody rb;
    
    // 애니메이션 파라미터
    private readonly int isMovingHash = Animator.StringToHash("IsMoving");
    
    // 애니메이션 시작 대기
    private bool animationStarted = false;

    public override bool InterruptsCombo => true;
    
    public PlayerHitState(PlayerController player) : base(player)
    {
        animator = player.Animator;
        rb = player.Rigidbody;
    }
    
    public override void Enter()
    {

        base.Enter();

        Debug.Log("PlayerHitState 진입 - 스턴!");

        // IsMoving = false (피격 중 이동 애니메이션 중지)
        animator.SetBool(isMovingHash, false);
        
        // 이동 정지
        rb.velocity = Vector3.zero;
        
        // 애니메이션 시작 플래그 리셋
        animationStarted = false;
        
        // 피격 애니메이션 재생
        animator.Play(AnimationConstants.HIT);
        
        Debug.Log("스턴 상태! 조작 불가!");
    }
    
    public override void Execute()
    {
        // 애니메이션 시작 대기
        if (!WaitForAnimationStart(animator, ref animationStarted, out var stateInfo))
            return;
        
        // 애니메이션 종료 체크
        if (stateInfo.normalizedTime >= 0.95f)
        {
            Debug.Log("스턴 종료! Idle로 복귀");
            
            // 스턴 게이지 완전 회복
            player.RecoverStunGauge(player.Data.stunRecoveryRate * Time.deltaTime);
            
            // Idle로 복귀
            player.StateMachine.ChangeState(player.IdleState);
        }
    }
    
    public override void Exit()
    {
        Debug.Log("PlayerHitState 종료");
    }
}