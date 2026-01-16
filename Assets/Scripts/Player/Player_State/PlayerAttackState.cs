// PlayerAttackState.cs

using UnityEngine;

public class PlayerAttackState : PlayerState
{
    // ========================================
    // Components
    // ========================================

    private Animator animator;
    private Rigidbody rb;
    private SocketManager socketManager;

    // ========================================
    // Animation
    // ========================================

    private readonly int isMovingHash = Animator.StringToHash("IsMoving");
    private bool animationStarted = false;

    // ========================================
    // Timing
    // ========================================

    private float attackTimer = 0f;

    // ========== 추가: Hitbox 타이밍 ==========
    private bool hitboxFired = false;  // ← 추가!
    private float hitboxTiming = 0.5f;  // ← 추가!

    // ========================================
    // Combo State
    // ========================================

    private bool isComboFinished = false;

    // ========================================
    // Rotation Limit
    // ========================================

    private float attackStartY;
    private const float MAX_ATTACK_ANGLE = 60f;
    private float currentRotationY;
    public override bool InterruptsCombo => false;

    // ========================================
    // Constructor
    // ========================================

    public PlayerAttackState(PlayerController player) : base(player)
    {
        animator = player.Animator;
        rb = player.Rigidbody;
        socketManager = player.SocketManager;
    }

    // ========================================
    // State Lifecycle
    // ========================================

    public override void Enter()
    {
        base.Enter();
        Debug.Log("PlayerAttackState 진입");

        animator.SetBool(isMovingHash, false);
        rb.velocity = Vector3.zero;

        // 플래그 초기화
        animationStarted = false;
        isComboFinished = false;
        attackTimer = 0f;

        // 회전 초기화
        attackStartY = player.Transform.eulerAngles.y;
        currentRotationY = attackStartY;

        // ========== 스킬 애니메이션 재생 ==========
        AttackSkillData skill = socketManager.GetCurrentSkill(); 

        if (skill != null)
        {
            animator.Play(skill.animationName);
            Debug.Log($"공격: {skill.skillName} (지속: {skill.TotalTime}초, 타격: {skill.HitboxTime:F2}초)");
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
        AttackSkillData skill = socketManager.GetCurrentSkill(); 

        if (skill == null)
        {
            Debug.LogError("스킬 없음 → Idle");
            player.StateMachine.ChangeState(player.IdleState);
            return;
        }

        // ========================================
        // 애니메이션 시작 대기
        // ========================================
        if (!WaitForAnimationStart(animator, ref animationStarted, out var stateInfo))
            return;

        // ========================================
        // Phase 1: 공격 중 (duration 전)
        // ========================================
        if (attackTimer < skill.duration)
            return;

        // ========================================
        // Phase 2: 여유 시간 (duration ~ TotalTime)
        // ========================================
        if (attackTimer < skill.TotalTime)
            return;

        // ========================================
        // Phase 3: 완료 (TotalTime 이상)
        // ========================================

        // 마지막 타 플래그 체크
        if (isComboFinished)
        {
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
             if (socketManager.IsComboComplete())
            {
                Debug.Log("콤보 완료 → 피니셔");
                player.StateMachine.ChangeState(player.FinisherState);
            }
            else
            {
                Debug.Log("공격 종료 → Idle");
                socketManager.ResetCombo();
                player.StateMachine.ChangeState(player.IdleState);
            }
        }
    }

    public override void Exit()
    {
        Debug.Log("PlayerAttackState 종료");

        // ========== 안전장치: Hitbox 강제 종료 ==========
        player.ForceDisableHitbox();  // ← 추가!

        rb.angularVelocity = Vector3.zero;
    }

    // ========================================
    // Public Methods
    // ========================================

    public void PlayNextStep()
    {
        animationStarted = false;
        attackTimer = 0f;
        hitboxFired = false;  // ← 추가!

        attackStartY = player.Transform.eulerAngles.y;
        currentRotationY = attackStartY;

        AttackSkillData skill = socketManager.GetCurrentSkill();

        if (skill != null)
        {
            hitboxTiming = skill.hitboxTiming;  // ← 추가!

            animator.Play(skill.animationName, 0, 0f);
            Debug.Log($"다음 타: {skill.skillName} (애니메이션: {skill.animationName})");
        }
    }

    public void OnComboFailed()
    {
        Debug.Log("콤보 실패! 현재 공격은 끝까지 재생");
    }

    // ========================================
    // Private Methods
    // ========================================

    private void HandleRotationDuringAttack()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal == 0 && vertical == 0)
            return;

        Transform cameraTransform = player.CameraTransform;
        if (cameraTransform == null)
            return;

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 targetDirection = forward * vertical + right * horizontal;
        if (targetDirection == Vector3.zero)
            return;

        float targetY = Quaternion.LookRotation(targetDirection).eulerAngles.y;
        float deltaFromStart = Mathf.DeltaAngle(attackStartY, targetY);
        float clampedDelta = Mathf.Clamp(deltaFromStart, -MAX_ATTACK_ANGLE, MAX_ATTACK_ANGLE);
        float finalY = attackStartY + clampedDelta;

        currentRotationY = Mathf.LerpAngle(currentRotationY, finalY, 10f * Time.fixedDeltaTime);
        player.Transform.rotation = Quaternion.Euler(0f, currentRotationY, 0f);
    }
}
