using UnityEngine;

/// <summary>
/// Player 사망 상태
/// HP 0이 되면 진입
/// 
/// 역할:
/// - 사망 애니메이션 재생
/// - 모든 입력 차단 (HandleInput에서 차단)
/// - 게임 오버 처리 (TODO)
/// </summary>
public class PlayerDeadState : PlayerState
{
    private Animator animator;
    private Rigidbody rb;
    
    // 애니메이션 파라미터
    private readonly int isMovingHash = Animator.StringToHash("IsMoving");
    
    // 애니메이션 시작 대기
    private bool animationStarted = false;
    
    public PlayerDeadState(PlayerController player) : base(player)
    {
        animator = player.Animator;
        rb = player.Rigidbody;
    }
    
    public override void Enter()
    {
        Debug.Log("PlayerDeadState 진입 - 사망!");

                
        // IsMoving = false (사망 중 이동 애니메이션 중지)
        animator.SetBool(isMovingHash, false);
        
        // 이동 정지
        rb.velocity = Vector3.zero;
        
        // 애니메이션 시작 플래그 리셋
        animationStarted = false;
        
        // 사망 애니메이션 재생
        animator.Play(AnimationConstants.DEAD);
        
        Debug.Log("사망! 게임 오버!");
    }
    
    public override void Execute()
    {
        // 애니메이션 시작 대기
        if (!WaitForAnimationStart(animator, ref animationStarted, out var stateInfo))
            return;
        
        // 사망 애니메이션 종료 후
        if (stateInfo.normalizedTime >= 0.95f)
        {
            // TODO: 게임 오버 UI 표시
            // TODO: 재시작 또는 메인 메뉴
            
            Debug.Log("게임 오버 - 재시작 대기");
        }
    }
    
    public override void Exit()
    {
        Debug.Log("PlayerDeadState 종료");
        
        // 사망 상태는 보통 Exit되지 않음
        // 게임 재시작이나 씬 리로드로 끝남
    }
}