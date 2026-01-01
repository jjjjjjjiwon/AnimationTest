using UnityEngine;

/// <summary>
/// Player 공격 상태
/// 공격 애니메이션 재생 및 State 전환 관리
/// 
/// 역할:
/// - ComboSystem으로부터 애니메이션 이름 받아서 재생
/// - 애니메이션 종료 감지 및 다음 State 전환 (Idle/Finisher)
/// - PlayerController로부터 다음 타 재생 명령 수신
/// - 공격 중 이동 정지 (물리 처리)
/// 
/// 콤보 로직 및 Perfect 타이밍 관리는 ComboSystem이 담당
/// </summary>
public class PlayerAttackState : PlayerState
{
    private Animator animator;
    private Rigidbody rb;
    private ComboSystem comboSystem;
    
    // 애니메이션 파라미터
    private readonly int isMovingHash = Animator.StringToHash("IsMoving");
    
    // 애니메이션 시작 대기
    private bool animationStarted = false;
    
    public PlayerAttackState(PlayerController player) : base(player)
    {
        animator = player.Animator;
        rb = player.Rigidbody;
        comboSystem = player.ComboSystem;
    }
    
    public override void Enter()
    {
        Debug.Log("PlayerAttackState 진입");
        
        // IsMoving = false (공격 중 이동 애니메이션 중지)
        animator.SetBool(isMovingHash, false);
        
        // 이동 정지
        rb.velocity = Vector3.zero;
        
        // 애니메이션 시작 플래그 리셋
        animationStarted = false;
        
        // 현재 콤보 단계의 애니메이션 재생
        string animationName = comboSystem.GetCurrentAnimation();
        animator.Play(animationName);
        
        Debug.Log($"공격 애니메이션 재생: {animationName}");
    }
    
    public override void Execute()
    {
        // 애니메이션 시작 대기
        if (!WaitForAnimationStart(animator, ref animationStarted, out var stateInfo))
            return;
        
        // 애니메이션 종료 체크
        if (stateInfo.normalizedTime >= 0.95f)
        {
            // 콤보 완료?
            if (comboSystem.IsComboComplete())
            {
                // 피니셔로 전환
                Debug.Log("콤보 완료! 피니셔로!");
                player.StateMachine.ChangeState(player.FinisherState);
            }
            else
            {
                // Idle로 복귀
                Debug.Log("공격 종료, Idle로");
                player.StateMachine.ChangeState(player.IdleState);
            }
        }
    }
    
    public override void Exit()
    {
        Debug.Log("PlayerAttackState 종료");
    }
    
    // ========== PlayerController에서 호출되는 메서드 ==========
    
    /// <summary>
    /// 다음 콤보 타 재생
    /// PlayerController.TryAttack()에서 ComboSystem.ProcessInput() 성공 시 호출
    /// </summary>
    public void PlayNextStep()
    {
        // 애니메이션 리셋
        animationStarted = false;
        
        // 다음 타 애니메이션 재생
        string animationName = comboSystem.GetCurrentAnimation();
        animator.Play(animationName);
        
        Debug.Log($"다음 타 재생: {animationName}");
    }
    
    /// <summary>
    /// 콤보 실패 처리
    /// PlayerController.TryAttack()에서 ComboSystem.ProcessInput() 실패 시 호출
    /// 현재 공격 애니메이션은 끝까지 재생 후 Idle로 복귀
    /// </summary>
    public void OnComboFailed()
    {
        Debug.Log("콤보 실패! 현재 공격 애니메이션은 끝까지 재생");
        
        // 콤보는 실패했지만 현재 공격은 끝까지
        // 애니메이션 종료 후 Idle로
        // (Execute에서 자동 처리됨)
    }
}