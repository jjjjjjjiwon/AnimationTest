using UnityEngine;

/// <summary>
/// Player 공격 상태
/// 공격 애니메이션 재생 및 State 전환 관리
/// </summary>
public class PlayerAttackState : PlayerState
{
    private Animator animator;
    private Rigidbody rb;
    private ComboSystem comboSystem;
    
    private readonly int isMovingHash = Animator.StringToHash("IsMoving");
    private bool animationStarted = false;
    
    // 공격 타이머
    private float attackTimer = 0f;
    private const float MIN_ATTACK_TIME = 0.5f;
    
    // ========== 콤보 완료 플래그 ==========
    /// <summary>
    /// 마지막 타를 재생했는지 여부
    /// true면 애니메이션 종료 후 피니셔로 전환
    /// </summary>
    private bool isComboFinished = false;
    
    public PlayerAttackState(PlayerController player) : base(player)
    {
        animator = player.Animator;
        rb = player.Rigidbody;
        comboSystem = player.ComboSystem;
    }
    
    public override void Enter()
    {
        Debug.Log("PlayerAttackState 진입");
        
        animator.SetBool(isMovingHash, false);
        rb.velocity = Vector3.zero;
        animationStarted = false;
        attackTimer = 0f;
        
        // ========== 콤보 완료 플래그 리셋 ==========
        isComboFinished = false;
        
        string animationName = comboSystem.GetCurrentAnimation();
        animator.Play(animationName);
        
        Debug.Log($"공격 애니메이션 재생: {animationName}");
    }
    
    public override void Execute()
    {
        // 최소 시간 대기
        attackTimer += Time.fixedDeltaTime;
        
        if (attackTimer < MIN_ATTACK_TIME)
            return;
        
        // 애니메이션 시작 대기
        if (!WaitForAnimationStart(animator, ref animationStarted, out var stateInfo))
            return;
        
        // ========== 콤보 완료 플래그 체크 ==========
        // 마지막 타 재생 중이면 애니메이션 종료 후 피니셔로
        if (isComboFinished)
        {
            if (stateInfo.normalizedTime >= 0.95f)
            {
                Debug.Log("마지막 타 완료! 피니셔로!");
                player.StateMachine.ChangeState(player.FinisherState);
                return;  // 아래 로직 실행 안 함
            }
            // 아직 애니메이션 재생 중이면 대기
            return;
        }
        
        // ========== 일반 애니메이션 종료 체크 ==========
        if (stateInfo.normalizedTime >= 0.95f)
        {
            if (comboSystem.IsComboComplete())
            {
                // 여기 도달하는 경우:
                // - 0.5초 대기 중에 입력 없이 시간이 다 지남
                // - 그런데 이미 콤보는 완료된 상태
                // (거의 발생 안 함, 플래그로 먼저 잡힘)
                Debug.Log("콤보 완료! 피니셔로!");
                player.StateMachine.ChangeState(player.FinisherState);
            }
            else
            {
                // 콤보 미완료 (중간에 끊김)
                Debug.Log("공격 종료, Idle로 (콤보 리셋)");
                comboSystem.ResetCombo();
                player.StateMachine.ChangeState(player.IdleState);
            }
        }
    }
    
    public override void Exit()
    {
        Debug.Log("PlayerAttackState 종료");
    }
    
    /// <summary>
    /// 다음 콤보 타 재생
    /// </summary>
    public void PlayNextStep()
    {
        animationStarted = false;
        attackTimer = 0f;
        
        string animationName = comboSystem.GetCurrentAnimation();
        animator.Play(animationName);
        
        Debug.Log($"다음 타 재생: {animationName}");
        
        // ========== 마지막 타 체크 (플래그만 설정) ==========
        if (comboSystem.IsComboComplete())
        {
            Debug.Log("마지막 타! 애니메이션 후 피니셔로!");
            isComboFinished = true;
            // State 전환은 Execute()에서 애니메이션 종료 후!
        }
    }
    
    /// <summary>
    /// 콤보 실패 처리
    /// </summary>
    public void OnComboFailed()
    {
        Debug.Log("콤보 실패! 현재 공격 애니메이션은 끝까지 재생");
    }
}
