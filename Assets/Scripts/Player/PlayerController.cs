using UnityEngine;

/// <summary>
/// Player 전체 제어 컨트롤러
/// </summary>
public class PlayerController : MonoBehaviour
{
    // ========================================
    // Components
    // ========================================

    [Header("Components")]
    [Tooltip("Player 데이터 (체력, 속도 등)")]
    [SerializeField] private PlayerData playerData;

    /// <summary>애니메이션 제어</summary>
    private Animator animator;

    /// <summary>물리 이동 제어</summary>
    private Rigidbody rb;

    // ========================================
    // Camera
    // ========================================

    [Header("Camera")]
    [Tooltip("카메라 Transform (이동 방향 계산용)")]
    [SerializeField] private Transform cameraTransform;

    // ========================================
    // Systems
    // ========================================

    [Header("Systems")]
    /// <summary>State Machine - Player 상태 관리</summary>
    private PlayerStateMachine stateMachine;

    /// <summary>Combo System - 콤보 입력 및 판정</summary>
    private ComboSystem comboSystem;

    // ========================================
    // States
    // ========================================

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }
    public PlayerFinisherState FinisherState { get; private set; }
    public PlayerDodgeState DodgeState { get; private set; }
    public PlayerHitState HitState { get; private set; }
    public PlayerDeadState DeadState { get; private set; }

    // ========================================
    // 전투 설정
    // ========================================

    [Header("전투 설정")]
    [Tooltip("공격 범위 반경 (m)")]
    [SerializeField] private float attackRange = 2f;

    [Tooltip("공격 중심점까지의 거리")]
    [SerializeField] private float attackDistance = 1.5f;

    // ========================================
    // Properties
    // ========================================

    public PlayerData PlayerData => playerData;
    public PlayerData Data => playerData;  // MoveState에서 사용
    public PlayerStateMachine StateMachine => stateMachine;
    public ComboSystem ComboSystem => comboSystem;
    public Animator Animator => animator;
    public Rigidbody Rigidbody => rb;
    public Transform CameraTransform => cameraTransform;
    public Transform Transform => transform;

    // ========================================
    // Unity 생명주기
    // ========================================

    void Start()
    {
        // Component 가져오기
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // Rigidbody 설정
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // System 초기화
        stateMachine = new PlayerStateMachine();

        // ========== ComboSystem 생성 (combos 전달) ==========
        comboSystem = new ComboSystem(playerData.combos);

        // State 인스턴스 생성
        IdleState = new PlayerIdleState(this);
        MoveState = new PlayerMoveState(this);
        AttackState = new PlayerAttackState(this);
        FinisherState = new PlayerFinisherState(this);
        DodgeState = new PlayerDodgeState(this);
        HitState = new PlayerHitState(this);
        DeadState = new PlayerDeadState(this);

        // 초기 State
        stateMachine.ChangeState(IdleState);
    }

    void Update()
    {
        HandleInput();

        // 테스트용: T키로 공격 판정
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("테스트: 공격 판정!");
            OnAttackHit();
        }
    }

    void FixedUpdate()
    {
        stateMachine.Update();
    }

    // ========================================
    // 입력 처리
    // ========================================

    void HandleInput()
    {
        // 회피 입력
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryDodge();
            return;
        }

        // 공격 입력
        if (Input.GetMouseButtonDown(0))
        {
            TryAttack(InputType.LeftClick);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            TryAttack(InputType.RightClick);
        }
    }

    // ========================================
    // 공격 시도
    // ========================================

    void TryAttack(InputType inputType)
    {
        PlayerState currentState = stateMachine.CurrentState;

        // Idle 또는 Move State
        if (currentState == IdleState || currentState == MoveState)
        {
            bool success = comboSystem.StartCombo(inputType);

            if (success)
            {
                stateMachine.ChangeState(AttackState);
            }
            else
            {
                Debug.Log("시작 가능한 콤보가 없습니다.");
            }
        }
        // Attack State
        else if (currentState == AttackState)
        {
            bool success = comboSystem.ProcessInput(inputType);

            if (success)
            {
                AttackState.PlayNextStep();
            }
            else
            {
                AttackState.OnComboFailed();
            }
        }
    }

    // ========================================
    // 회피 시도
    // ========================================
    // ========================================
    // 회피 쿨타임
    // ========================================

    /// <summary>마지막 회피 시간</summary>
    private float lastDodgeTime = -999f;

    /// <summary>
    /// 회피 가능 여부 체크
    /// - 쿨타임 경과했는지 확인
    /// </summary>
    private bool CanDodge()
    {
        return Time.time >= lastDodgeTime + playerData.dodgeCooldown;
    }

    /// <summary>회피 시도</summary>
    void TryDodge()
    {
        if (stateMachine == null)
            return;

        PlayerState currentState = stateMachine.CurrentState;

        // 회피 가능한 State 체크
        if (currentState == IdleState ||
            currentState == MoveState ||
            currentState == AttackState)
        {
            // 쿨타임 체크
            if (!CanDodge())
            {
                float remaining = playerData.dodgeCooldown - (Time.time - lastDodgeTime);
                Debug.Log($"회피 쿨타임! {remaining:F1}초 남음");
                return;
            }

            // 회피 실행
            lastDodgeTime = Time.time;
            stateMachine.ChangeState(DodgeState);
        }
        else
        {
            Debug.Log("현재 상태에서는 회피할 수 없습니다.");
        }
    }

    // ========================================
    // 전투 시스템 - 공격
    // ========================================

    /// <summary>
    /// 공격 타격 판정
    /// </summary>
    public void OnAttackHit()
    {
        // 공격 범위 중심점 계산
        Vector3 attackPosition = transform.position + transform.forward * attackDistance;

        // Enemy Collider 감지
        Collider[] hitColliders = Physics.OverlapSphere(
            attackPosition,
            attackRange,
            LayerMask.GetMask("Enemy")
        );

        Debug.Log($"공격 범위 내 Enemy: {hitColliders.Length}명");

        // 감지된 Enemy들 처리
        foreach (Collider col in hitColliders)
        {
            EnemyController enemy = col.GetComponent<EnemyController>();

            if (enemy != null)
            {
                // 데미지 계산
                float damage = ComboSystem.GetCurrentDamage();
                float stunDuration = ComboSystem.GetCurrentStunDuration();

                Debug.Log($"Enemy 타격! 데미지: {damage}, 스턴: {stunDuration}초");

                // Enemy에게 적용
                enemy.TakeDamage(damage);
                enemy.ApplyStun(stunDuration);
            }
        }
    }

    // ========================================
    // 전투 시스템 - 피격
    // ========================================

    /// <summary>
    /// Player가 데미지 받기
    /// </summary>
    public void TakeDamage(float damage)
    {
        // 체력 감소
        playerData.currentHealth -= damage;

        Debug.Log($"Player 피격! 데미지: {damage}, 남은 체력: {playerData.currentHealth}");

        // 사망 체크
        if (playerData.currentHealth <= 0)
        {
            playerData.currentHealth = 0;
            Die();
            return;
        }

        // HitState로 전환
        StateMachine.ChangeState(HitState);
    }

    /// <summary>
    /// Player가 스턴 데미지 받기
    /// </summary>
    public void TakeStunDamage(float stunDamage)
    {
        // TODO: 구현
        Debug.Log($"Player 스턴 데미지: {stunDamage}");
    }



    /// <summary>
    /// Player 사망 처리
    /// </summary>
    public void Die()
    {
        Debug.Log("Player 사망!");

        // DeadState로 전환
        StateMachine.ChangeState(DeadState);
    }

    /// <summary>
    /// 스턴 게이지 회복
    /// PlayerHitState에서 호출
    /// </summary>
    public void RecoverStunGauge(float amount)
    {
        // TODO: 구현
        Debug.Log($"스턴 게이지 회복: {amount}");
    }

    // ========================================
    // 디버그 시각화
    // ========================================

    void OnDrawGizmosSelected()
    {
        if (transform != null)
        {
            Vector3 attackPosition = transform.position + transform.forward * attackDistance;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPosition, attackRange);
        }
    }


/// <summary>
    /// 디버그 정보 표시 (좌측 상단)
    /// - 현재 State
    /// - HP
    /// - 스턴 게이지
    /// - 콤보 단계
    /// - 회피 쿨타임
    /// </summary>
    void OnGUI()
    {
        // 배경 스타일
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.alignment = TextAnchor.UpperLeft;
        boxStyle.fontSize = 16;
        boxStyle.normal.textColor = Color.white;
        
        // 텍스트 스타일
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = 14;
        labelStyle.normal.textColor = Color.white;
        
        // 배경 박스
        GUI.Box(new Rect(10, 10, 250, 160), "Player Debug Info", boxStyle);
        
        // 정보 표시
        int yPos = 35;
        int lineHeight = 20;
        
        // State
        string stateName = StateMachine?.CurrentState?.GetType().Name ?? "None";
        stateName = stateName.Replace("Player", "").Replace("State", "");
        GUI.Label(new Rect(20, yPos, 230, 20), $"State: {stateName}", labelStyle);
        yPos += lineHeight;
        
        // HP
        GUI.Label(new Rect(20, yPos, 230, 20), 
            $"HP: {playerData.currentHealth:F0} / {playerData.maxHealth:F0}", labelStyle);
        yPos += lineHeight;
        
        // 스턴 게이지
        GUI.Label(new Rect(20, yPos, 230, 20), 
            $"Stun: {playerData.currentStunGauge:F0} / {playerData.maxStunGauge:F0}", labelStyle);
        yPos += lineHeight;
        
        // 콤보 단계
        int currentStep = ComboSystem?.GetCurrentStep() ?? -1;
        int totalSteps = ComboSystem?.GetCurrentCombo()?.steps.Length ?? 0;
        GUI.Label(new Rect(20, yPos, 230, 20), 
            $"Combo: {currentStep + 1} / {totalSteps}", labelStyle);
        yPos += lineHeight;
        
        // Perfect 카운트
        int perfectCount = ComboSystem?.GetPerfectCount() ?? 0;
        GUI.Label(new Rect(20, yPos, 230, 20), 
            $"Perfect: {perfectCount}", labelStyle);
        yPos += lineHeight;
        
        // 회피 쿨타임
        bool canDodge = CanDodge();
        float cooldownRemaining = canDodge ? 0f : 
            playerData.dodgeCooldown - (Time.time - lastDodgeTime);
        
        string dodgeText = canDodge ? "Ready!" : $"{cooldownRemaining:F1}s";
        GUI.Label(new Rect(20, yPos, 230, 20), 
            $"Dodge: {dodgeText}", labelStyle);
    }
}
