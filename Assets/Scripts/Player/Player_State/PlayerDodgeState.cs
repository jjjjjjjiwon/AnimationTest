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

    // ========================================
    // Rotation Limit
    // ========================================

    /// <summary>공격 시작 시 Y축 회전값 (회전 제한 기준점)</summary>
    private float attackStartY;

    /// <summary>최대 회전 각도 (좌우 60도)</summary>
    private const float MAX_ATTACK_ANGLE = 60f;

    /// <summary>부드러운 회전을 위한 현재 회전값</summary>
    private float currentRotationY;

    // 애니메이션 파라미터
    private readonly int isMovingHash = Animator.StringToHash("IsMoving");

    // 애니메이션 시작 대기
    private bool animationStarted = false;

     public override bool InterruptsCombo => true;

    public PlayerDodgeState(PlayerController player) : base(player)
    {
        animator = player.Animator;
        rb = player.Rigidbody;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("PlayerDodgeState 진입");

        player.ForceDisableHitbox();

        // IsMoving = false (회피 중 이동 애니메이션 중지)
        animator.SetBool(isMovingHash, false);

        // 이동 정지 (루트 모션이 이동 담당)
        rb.velocity = Vector3.zero;

        // 애니메이션 시작 플래그 리셋
        animationStarted = false;

        // 회피 애니메이션 재생
        animator.Play(AnimationConstants.DODGE);

        OnBackStepStart();
        Debug.Log("회피!");
    }

    public override void Execute()
    {
        HandleRotationDuringDodge();

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


    void OnBackStepStart()
    {
        attackStartY = player.Transform.eulerAngles.y;
        currentRotationY = attackStartY;
    }
    /// <summary>
    /// 공격 중 제한된 회전 처리
    /// - 입력 방향으로 회전
    /// - ±60도로 제한
    /// - Lerp로 부드럽게
    /// </summary>
    private void HandleRotationDuringDodge()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(horizontal) < 0.01f)
            return;

        Transform cam = player.CameraTransform;
        if (cam == null)
            return;

        // 공격 시작 방향
        Vector3 startForward = Quaternion.Euler(0f, attackStartY, 0f) * Vector3.forward;

        // 카메라 오른쪽 벡터
        Vector3 camRight = cam.right;
        camRight.y = 0f;
        camRight.Normalize();

        // 🔥 핵심: 카메라 right가 startForward 기준 어느 쪽인가
        float side = Vector3.SignedAngle(startForward, camRight, Vector3.up);

        // side > 0 → camRight가 오른쪽
        // side < 0 → camRight가 왼쪽

        float directionSign = side > 0 ? 1f : -1f;

        // 입력 보정
        float correctedInput = -horizontal * directionSign;

        // 회전 각도
        float targetOffset = correctedInput * MAX_ATTACK_ANGLE;
        float finalY = attackStartY + targetOffset;

        currentRotationY = Mathf.LerpAngle(currentRotationY, finalY, 10f * Time.deltaTime);

        player.Transform.rotation = Quaternion.Euler(0f, currentRotationY, 0f);
    }
}