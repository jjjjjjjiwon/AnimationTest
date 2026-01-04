using UnityEngine;

/// <summary>
/// Player 공격 상태
/// - 공격 애니메이션 재생
/// - 콤보 진행 및 피니셔 전환
/// - 공격 중 제한된 회전 (±60도)
/// - 최소 0.5초 공격 유지
/// </summary>
public class PlayerAttackState : PlayerState
{
    // ========================================
    // Components
    // ========================================
    
    private Animator animator;
    private Rigidbody rb;
    private ComboSystem comboSystem;
    
    // ========================================
    // Animation
    // ========================================
    
    /// <summary>IsMoving 파라미터 해시</summary>
    private readonly int isMovingHash = Animator.StringToHash("IsMoving");
    
    /// <summary>애니메이션 시작 플래그</summary>
    private bool animationStarted = false;
    
    // ========================================
    // Timer
    // ========================================
    
    /// <summary>공격 경과 시간</summary>
    private float attackTimer = 0f;
    
    /// <summary>최소 공격 지속 시간</summary>
    private const float MIN_ATTACK_TIME = 0.5f;
    
    // ========================================
    // Combo State
    // ========================================
    
    /// <summary>콤보 완료 플래그 (마지막 타 재생 시 true)</summary>
    private bool isComboFinished = false;
    
    // ========================================
    // Rotation Limit
    // ========================================
    
    /// <summary>공격 시작 시 Y축 회전값 (회전 제한 기준점)</summary>
    private float attackStartY;
    
    /// <summary>최대 회전 각도 (좌우 60도)</summary>
    private const float MAX_ATTACK_ANGLE = 60f;
    
    /// <summary>부드러운 회전을 위한 현재 회전값</summary>
    private float currentRotationY;
    
    // ========================================
    // Constructor
    // ========================================
    
    public PlayerAttackState(PlayerController player) : base(player)
    {
        animator = player.Animator;
        rb = player.Rigidbody;
        comboSystem = player.ComboSystem;
    }
    
    // ========================================
    // State Lifecycle
    // ========================================
    
    public override void Enter()
    {
        Debug.Log("PlayerAttackState 진입");
        
        // 애니메이션 설정
        animator.SetBool(isMovingHash, false);
        
        // 이동 정지
        rb.velocity = Vector3.zero;
        
        // 플래그 초기화
        animationStarted = false;
        isComboFinished = false;
        attackTimer = 0f;
        
        // 회전 초기화
        attackStartY = player.Transform.eulerAngles.y;
        currentRotationY = attackStartY;
        
        // 애니메이션 재생
        string animationName = comboSystem.GetCurrentAnimation();
        animator.Play(animationName);
        
        Debug.Log($"공격 애니메이션 재생: {animationName}");
    }
    
    public override void Execute()
    {
        // 공격 중 회전 처리
        HandleRotationDuringAttack();
        
        // 최소 시간 대기
        attackTimer += Time.fixedDeltaTime;
        if (attackTimer < MIN_ATTACK_TIME)
            return;
        
        // 애니메이션 시작 대기
        if (!WaitForAnimationStart(animator, ref animationStarted, out var stateInfo))
            return;
        
        // 콤보 완료 플래그 체크
        if (isComboFinished)
        {
            if (stateInfo.normalizedTime >= 0.95f)
            {
                Debug.Log("마지막 타 완료! 피니셔로!");
                player.StateMachine.ChangeState(player.FinisherState);
                return;
            }
            return;
        }
        
        // 애니메이션 종료 체크
        if (stateInfo.normalizedTime >= 0.95f)
        {
            if (comboSystem.IsComboComplete())
            {
                Debug.Log("콤보 완료! 피니셔로!");
                player.StateMachine.ChangeState(player.FinisherState);
            }
            else
            {
                Debug.Log("공격 종료, Idle로");
                comboSystem.ResetCombo();
                player.StateMachine.ChangeState(player.IdleState);
            }
        }
    }
    
    public override void Exit()
    {
        Debug.Log("PlayerAttackState 종료");
        
        // 회전 정지
        rb.angularVelocity = Vector3.zero;
    }
    
    // ========================================
    // Public Methods
    // ========================================
    
    /// <summary>다음 콤보 타 재생</summary>
    public void PlayNextStep()
    {
        // 플래그 리셋
        animationStarted = false;
        attackTimer = 0f;
        
        // 회전 갱신
        attackStartY = player.Transform.eulerAngles.y;
        currentRotationY = attackStartY;
        
        // 애니메이션 재생
        string animationName = comboSystem.GetCurrentAnimation();
        animator.Play(animationName);
        
        Debug.Log($"다음 타 재생: {animationName}");
        
        // 마지막 타 체크
        if (comboSystem.IsComboComplete())
        {
            Debug.Log("마지막 타! 애니메이션 후 피니셔로!");
            isComboFinished = true;
        }
    }
    
    /// <summary>콤보 실패 처리</summary>
    public void OnComboFailed()
    {
        Debug.Log("콤보 실패! 현재 공격 애니메이션은 끝까지 재생");
    }
    
    // ========================================
    // Private Methods
    // ========================================
    
    /// <summary>
    /// 공격 중 제한된 회전 처리
    /// - 입력 방향으로 회전
    /// - ±60도로 제한
    /// - Lerp로 부드럽게
    /// </summary>
    private void HandleRotationDuringAttack()
    {
        // 입력 받기
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal == 0 && vertical == 0)
            return;

        // 카메라 확인
        Transform cameraTransform = player.CameraTransform;
        if (cameraTransform == null)
            return;

        // 카메라 기준 방향
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // 입력 방향 계산
        Vector3 targetDirection = forward * vertical + right * horizontal;
        if (targetDirection == Vector3.zero)
            return;

        // 목표 Y 회전값
        float targetY = Quaternion.LookRotation(targetDirection).eulerAngles.y;

        // 공격 시작 기준 각도 차이
        float deltaFromStart = Mathf.DeltaAngle(attackStartY, targetY);

        // 각도 제한 (±60도)
        float clampedDelta = Mathf.Clamp(
            deltaFromStart,
            -MAX_ATTACK_ANGLE,
            MAX_ATTACK_ANGLE
        );

        // 최종 목표 각도
        float finalY = attackStartY + clampedDelta;

        // 부드러운 회전 (Lerp)
        // 10f: 회전 속도 (5f=느림, 10f=적당, 20f=빠름)
        currentRotationY = Mathf.LerpAngle(
            currentRotationY,
            finalY,
            10f * Time.fixedDeltaTime
        );

        // 회전 적용
        player.Transform.rotation = Quaternion.Euler(0f, currentRotationY, 0f);
    }
}