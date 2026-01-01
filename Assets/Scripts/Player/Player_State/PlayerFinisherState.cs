using UnityEngine;

/// <summary>
/// Player 피니셔 상태
/// 콤보 완료 후 마무리 공격
/// 
/// 역할:
/// - 피니셔 애니메이션 재생
/// - Perfect 개수에 따른 데미지 계산 (ComboSystem 사용)
/// - 애니메이션 종료 후 Idle로 복귀
/// </summary>
public class PlayerFinisherState : PlayerState
{
    private Animator animator;
    private Rigidbody rb;
    private ComboSystem comboSystem;
    
    // 애니메이션 파라미터
    private readonly int isMovingHash = Animator.StringToHash("IsMoving");
    
    // 애니메이션 시작 대기
    private bool animationStarted = false;
    
    public PlayerFinisherState(PlayerController player) : base(player)
    {
        animator = player.Animator;
        rb = player.Rigidbody;
        comboSystem = player.ComboSystem;
    }
    
    public override void Enter()
    {
        Debug.Log("PlayerFinisherState 진입");
        
        // IsMoving = false (피니셔 중 이동 애니메이션 중지)
        animator.SetBool(isMovingHash, false);
        
        // 이동 정지
        rb.velocity = Vector3.zero;
        
        // 애니메이션 시작 플래그 리셋
        animationStarted = false;
        
        // 피니셔 애니메이션 재생
        string animationName = comboSystem.GetFinisherAnimation();
        animator.Play(animationName);
        
        // Perfect 개수 및 데미지 로그
        int perfectCount = comboSystem.GetPerfectCount();
        float damage = comboSystem.GetFinisherDamage();
        
        Debug.Log($"피니셔 애니메이션 재생: {animationName}");
        Debug.Log($"Perfect {perfectCount}개! 피니셔 데미지: {damage}");
    }
    
    public override void Execute()
    {
        // 애니메이션 시작 대기
        if (!WaitForAnimationStart(animator, ref animationStarted, out var stateInfo))
            return;
        
        // 애니메이션 종료 체크
        if (stateInfo.normalizedTime >= 0.95f)
        {
            Debug.Log("피니셔 완료! Idle로");
            
            // 콤보 리셋
            comboSystem.ResetCombo();
            
            // Idle로 복귀
            player.StateMachine.ChangeState(player.IdleState);
        }
    }
    
    public override void Exit()
    {
        Debug.Log("PlayerFinisherState 종료");
    }
}