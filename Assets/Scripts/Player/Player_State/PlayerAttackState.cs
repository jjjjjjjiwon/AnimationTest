using UnityEngine;

/// <summary>
/// Player 공격 상태
/// - AttackSkillData 기반 공격
/// - 스킬마다 다른 duration/exitTime
/// - 공격 중 제한된 회전 (±60도)
/// </summary>
public class PlayerAttackState : PlayerState
{
    // ========================================
    // Components
    // ========================================
    
    private Animator animator;
    private Rigidbody rb;
    private ComboSocket comboSocket;
    
    // ========================================
    // Animation
    // ========================================
    
    private readonly int isMovingHash = Animator.StringToHash("IsMoving");
    private bool animationStarted = false;  // 애니메이션 시작 플래그
    
    // ========================================
    // Timing
    // ========================================
    
    private float attackTimer = 0f;  // 공격 경과 시간
    
    // ========================================
    // Combo State
    // ========================================
    
    private bool isComboFinished = false;  // 마지막 타 플래그
    
    // ========================================
    // Rotation Limit
    // ========================================
    
    private float attackStartY;                 // 공격 시작 Y 회전값
    private const float MAX_ATTACK_ANGLE = 60f; // 최대 회전 각도
    private float currentRotationY;             // 현재 회전값 (Lerp용)
    

    public override bool InterruptsCombo => false;

    // ========================================
    // Constructor
    // ========================================
    
    public PlayerAttackState(PlayerController player) : base(player)
    {
        animator = player.Animator;
        rb = player.Rigidbody;
        comboSocket = player.ComboSocket;
    }
    
    // ========================================
    // State Lifecycle
    // ========================================
    
    public override void Enter()
    {
        base.Enter(); 
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
        
        // ========== 스킬 애니메이션 재생 ==========
        AttackSkillData skill = comboSocket.GetCurrentSkill();
        
        if (skill != null)
        {
            animator.Play(skill.animationName);
            Debug.Log($"공격: {skill.skillName} (지속: {skill.TotalTime}초)");
        }
        else
        {
            Debug.LogError("스킬이 없습니다!");
            player.StateMachine.ChangeState(player.IdleState);
        }
    }
    
    public override void Execute()
    {
        // 공격 중 회전
        HandleRotationDuringAttack();
        
        // 타이머 증가
        attackTimer += Time.fixedDeltaTime;
        
        // ========== 스킬 가져오기 ==========
        AttackSkillData skill = comboSocket.GetCurrentSkill();
        
        if (skill == null)
        {
            Debug.LogError("스킬 없음 → Idle");
            player.StateMachine.ChangeState(player.IdleState);
            return;
        }
        
        // ========== Phase 1: 공격 중 (duration 전) ==========
        // 아직 공격이 진행 중
        // 입력 불가, 대기
        if (attackTimer < skill.duration)
            return;
        
        // ========== Phase 2: 여유 시간 (duration ~ TotalTime) ==========
        // 입력 가능 구간
        // 아직 TotalTime 전이므로 계속 대기
        if (attackTimer < skill.TotalTime)
            return;
        
        // ========== Phase 3: 완료 (TotalTime 이상) ==========
        // 애니메이션 시작 대기
        if (!WaitForAnimationStart(animator, ref animationStarted, out var stateInfo))
            return;
        
        // 마지막 타 플래그 체크
        if (isComboFinished)
        {
            // 애니메이션도 끝났는지 확인
            if (stateInfo.normalizedTime >= 0.95f)
            {
                Debug.Log("마지막 타 완료 → 피니셔");
                player.StateMachine.ChangeState(player.FinisherState);
                return;
            }
            return;
        }
        
        // 애니메이션 종료 체크
        if (stateInfo.normalizedTime >= 0.95f)
        {
            if (comboSocket.IsComboComplete())
            {
                Debug.Log("콤보 완료 → 피니셔");
                player.StateMachine.ChangeState(player.FinisherState);
            }
            else
            {
                Debug.Log("공격 종료 → Idle");
                comboSocket.ResetCombo();
                player.StateMachine.ChangeState(player.IdleState);
            }
        }
    }
    
    public override void Exit()
    {
        Debug.Log("PlayerAttackState 종료");
        rb.angularVelocity = Vector3.zero;
    }
    
    // ========================================
    // Public Methods
    // ========================================
    
    /// <summary>
    /// 다음 콤보 타 재생
    /// - PlayerController.TryAttack()에서 호출
    /// </summary>
    public void PlayNextStep()
{
    animationStarted = false;
    attackTimer = 0f;
    
    attackStartY = player.Transform.eulerAngles.y;
    currentRotationY = attackStartY;
    
    AttackSkillData skill = comboSocket.GetCurrentSkill();
    
    if (skill != null)
    {
        // ========== 강제 재시작 ==========
        // animator.Play()는 같은 애니메이션이면 무시됨
        // → 강제로 재시작시키기!
        
        animator.Play(skill.animationName, 0, 0f);
        // 파라미터:
        //   - stateName: 애니메이션 이름
        //   - layer: 0 (Base Layer)
        //   - normalizedTime: 0f (처음부터!)
        
        Debug.Log($"다음 타: {skill.skillName} (애니메이션: {skill.animationName})");
    }
}
    
    /// <summary>
    /// 콤보 실패 처리
    /// </summary>
    public void OnComboFailed()
    {
        Debug.Log("콤보 실패! 현재 공격은 끝까지 재생");
    }
    
    // ========================================
    // Private Methods
    // ========================================
    
    /// <summary>
    /// 공격 중 제한된 회전
    /// - 입력 방향으로 회전
    /// - ±60도 제한
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

        // 시작 기준 각도 차이
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
        currentRotationY = Mathf.LerpAngle(
            currentRotationY,
            finalY,
            10f * Time.fixedDeltaTime
        );

        // 회전 적용
        player.Transform.rotation = Quaternion.Euler(0f, currentRotationY, 0f);
    }
}
