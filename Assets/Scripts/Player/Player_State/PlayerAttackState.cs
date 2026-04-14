using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private Animator animator;
    private Rigidbody rb;
    private SocketManager socketManager;

    private readonly int isMovingHash = Animator.StringToHash("IsMoving");
    private bool animationStarted = false;
    private float attackTimer = 0f;

    private bool canProcessNext = false;
    private bool inputReceived = false;
    private bool isLastInputPerfect = false;

    private float attackStartY;
    private const float MAX_ATTACK_ANGLE = 60f;

    public override bool InterruptsCombo => false;

    public PlayerAttackState(PlayerController player) : base(player)
    {
        animator = player.Animator;
        rb = player.Rigidbody;
        socketManager = player.SocketManager;
    }

    public override void Enter()
    {
        base.Enter();
        animationStarted = false;
        attackTimer = 0f;
        canProcessNext = false;
        inputReceived = false;

        animator.SetBool(isMovingHash, false);
        rb.velocity = Vector3.zero;
        
        // 공격 시작 시점의 각도 저장
        attackStartY = player.Transform.eulerAngles.y;

        PlayCurrentSkill();

        if (player.ComboUI != null)
        {
            player.ComboUI.gameObject.SetActive(true);
            player.ComboUI.RefreshPerfectZone();
        }
    }

    public override void Execute()
    {
        HandleRotationDuringAttack();
        attackTimer += Time.fixedDeltaTime;

        PlayerSkillData skill = socketManager.GetCurrentSkill();
        if (skill == null) return;

        if (!WaitForAnimationStart(animator, ref animationStarted, out var stateInfo)) return;

        // 입력 가능 구간 (JSON 데이터 기반으로 확장 가능)
        if (stateInfo.normalizedTime > 0.2f && stateInfo.normalizedTime < 0.95f)
        {
            canProcessNext = true;
        }

        // 다음 타수 진행
        if (inputReceived && stateInfo.normalizedTime >= 0.7f)
        {
            PlayNextStep();
            return;
        }

        // 콤보 종료 및 타임아웃 체크
        if (stateInfo.normalizedTime >= 0.98f)
        {
            if (socketManager.IsComboComplete())
            {
                player.StateMachine.ChangeState(player.FinisherState);
            }
            else if (!inputReceived)
            {
                // 애니메이션 길이 + 설정된 후딜레이(post_Delay) 체크
                if (attackTimer >= stateInfo.length + skill.post_Delay)
                {
                    Debug.Log($"[Reset] 콤보 타임아웃");
                    socketManager.ResetCombo();
                    player.StateMachine.ChangeState(player.IdleState);
                }
            }
        }
    }

    private void PlayCurrentSkill()
    {
        PlayerSkillData skill = socketManager.GetCurrentSkill();
        if (skill != null)
        {
            animator.Play(skill.animation_Name, 0, 0f);
        }
        else
        {
            player.StateMachine.ChangeState(player.IdleState);
        }
    }

    private void PlayNextStep()
    {
        animationStarted = false;
        attackTimer = 0f;
        inputReceived = false;
        canProcessNext = false;
        PlayCurrentSkill();
    }

    public void RegisterInput()
    {
        inputReceived = true;
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float currentTime = stateInfo.normalizedTime % 1.0f;

        PlayerSkillData skill = socketManager.GetCurrentSkill();

        if (currentTime >= skill.perfect_Start && currentTime <= skill.perfect_End)
        {
            isLastInputPerfect = true;
            Debug.Log("<color=cyan>✨ PERFECT! ✨</color>");
        }
        else
        {
            isLastInputPerfect = false;
        }
    }

    // [에러 해결된 회전 로직]
    private void HandleRotationDuringAttack()
    {
        Vector3 moveInput = player.MoveInput; 
        if (moveInput.sqrMagnitude < 0.01f) return;

        if (player.CameraTransform == null) return;

        Vector3 forward = player.CameraTransform.forward;
        Vector3 right = player.CameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 desiredDirection = (forward * moveInput.z + right * moveInput.x).normalized;
        
        if (desiredDirection != Vector3.zero)
        {
            // Mathf.RadDeg로 오타 수정 완료
            float targetAngle = Mathf.Atan2(desiredDirection.x, desiredDirection.z) * Mathf.Rad2Deg;
            
            // 시작 각도 기준으로 제한된 회전 적용
            float angleDiff = Mathf.DeltaAngle(attackStartY, targetAngle);
            angleDiff = Mathf.Clamp(angleDiff, -MAX_ATTACK_ANGLE, MAX_ATTACK_ANGLE);
            
            float finalAngle = attackStartY + angleDiff;
            Quaternion targetRotation = Quaternion.Euler(0, finalAngle, 0);

            player.Transform.rotation = Quaternion.Slerp(
                player.Transform.rotation, 
                targetRotation, 
                Time.fixedDeltaTime * 10f
            );
        }
    }

    // Controller에서 호출하는 입력 가능 여부 체크
    public bool CanInputCombo() => canProcessNext && !inputReceived;
}