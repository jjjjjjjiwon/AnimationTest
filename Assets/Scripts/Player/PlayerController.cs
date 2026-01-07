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

    // ========== ComboSocket 노출 ==========
    private ComboSocket comboSocket;
    public ComboSocket ComboSocket => comboSocket;  // ← 추가!

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

    /// <summary>
    /// 체력
    /// </summary>
    
    [Header("플레이어 정보")]
    [SerializeField] private float currentHealth;

    // ========================================
    // 전투 설정
    // ========================================

    [Header("전투 설정")]
    [Tooltip("공격 범위 반경 (m)")]
    [SerializeField] private float attackRange = 2f;

    [Tooltip("공격 중심점까지의 거리")]
    [SerializeField] private float attackDistance = 1.5f;

    // ========================================
    // 전투 능력치
    // ========================================

    [Header("전투 능력치")]
    [Tooltip("현재 장착 무기")]
    public WeaponData currentWeapon;

    [Tooltip("버프 배율 (1.0 = 기본, 1.5 = 150%)")]
    private float buffMultiplier = 1.0f;

    // ========================================
    // Weapon
    // ========================================  

    [Header("Hitbox")]
    [SerializeField] private WeaponHitbox weaponHitbox;

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

    public HPBar hPBar;

    void Start()
    {
        currentHealth = playerData.maxHealth;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ;

        stateMachine = new PlayerStateMachine();

        // ========== ComboSocket 생성 ==========
        // PlayerData 전달 → 복원 OR 기본 소켓 생성
        comboSocket = new ComboSocket(playerData);

        // State 생성
        IdleState = new PlayerIdleState(this);
        MoveState = new PlayerMoveState(this);
        AttackState = new PlayerAttackState(this);
        FinisherState = new PlayerFinisherState(this);
        DodgeState = new PlayerDodgeState(this);
        HitState = new PlayerHitState(this);
        DeadState = new PlayerDeadState(this);

        stateMachine.ChangeState(IdleState);

    }

    void Update()
    {
        HandleInput();

        // 테스트용: T키로 공격 판정
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10);
            Debug.Log("데미지 받음");
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
        // ========== UI 열려있으면 게임 입력 차단! ==========
        if (SocketManagerUI.IsUIOpen)
            return;

        // 회피 입력
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryDodge();
            return;
        }

        // 공격 입력
        InputTypes inputType = GetInputType();
        if (inputType != InputTypes.None)
        {
            TryAttack(inputType);
        }
    }

    /// <summary>
    /// 현재 프레임 입력을 InputType으로 변환
    /// </summary>
    private InputTypes GetInputType()
    {
        // ========== UI 열려있으면 None 반환 ==========
        if (SocketManagerUI.IsUIOpen)
            return InputTypes.None;

        if (Input.GetMouseButtonDown(0))
            return InputTypes.LeftClick;

        if (Input.GetMouseButtonDown(1))
            return InputTypes.RightClick;

        if (Input.GetKeyDown(KeyCode.Q))
            return InputTypes.QKey;

        if (Input.GetKeyDown(KeyCode.E))
            return InputTypes.EKey;

        if (Input.GetKeyDown(KeyCode.R))
            return InputTypes.RKey;

        return InputTypes.None;
    }


    /// <summary>
    /// 공격 시도
    /// </summary>
    void TryAttack(InputTypes inputType)
    {
        PlayerState currentState = stateMachine.CurrentState;

        // Idle 또는 Move에서 시작
        if (currentState == IdleState || currentState == MoveState)
        {
            bool success = comboSocket.StartCombo(inputType);

            if (success)
            {
                stateMachine.ChangeState(AttackState);
            }
            else
            {
                Debug.Log("콤보 시작 실패!");
            }
        }
        // Attack 중
        else if (currentState == AttackState)
        {
            bool success = comboSocket.ProcessNext(inputType);

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
    // 데미지 계산
    // ========================================

    /// <summary>
    /// 최종 데미지 계산
    /// </summary>
    public float CalculateDamage(AttackSkillData skill)
    {
        float totalDamage = 0f;

        // 1. 스킬 기본 데미지
        totalDamage += skill.baseDamage;

        // 2. 무기 데미지 (나중에)
        // if (currentWeapon != null)
        //     totalDamage += currentWeapon.damage;

        // 3. 아이템 보너스 (나중에)
        // totalDamage += GetItemBonusDamage();

        // 4. 버프 (나중에)
        // totalDamage *= buffMultiplier;

        return totalDamage;
    }


    // ========================================
    // Hitbox 제어 (애니메이션 이벤트에서 호출)
    // ========================================
    
    /// <summary>
    /// Hitbox 활성화
    /// - 애니메이션 이벤트에서 호출
    /// </summary>
    public void HitboxOn()
    {
        if (weaponHitbox == null)
        {
            Debug.LogError("WeaponHitbox가 없습니다!");
            return;
        }
        
        weaponHitbox.gameObject.SetActive(true);
        weaponHitbox.ResetHitList();
        
        Debug.Log("Hitbox ON");
    }

    /// <summary>
    /// Hitbox 비활성화
    /// - 애니메이션 이벤트에서 호출
    /// </summary>
    public void HitboxOff()
    {
        if (weaponHitbox == null)
            return;
        
        weaponHitbox.gameObject.SetActive(false);
        
        Debug.Log("Hitbox OFF");
    }
    
    /// <summary>
    /// 무기가 적과 충돌했을 때
    /// - WeaponHitbox에서 호출
    /// </summary>
    public void OnWeaponHit(Collider enemyCollider)
    {
        EnemyController enemy = enemyCollider.GetComponent<EnemyController>();
        
        if (enemy == null)
            return;
        
        // 현재 스킬 가져오기
        AttackSkillData skill = ComboSocket.GetCurrentSkill();
        
        if (skill == null)
            return;
        
        // 데미지 계산
        float damage = CalculateDamage(skill);
        float stun = skill.stunDuration;
        
        Debug.Log($"적 타격! 데미지: {damage}, 스턴: {stun}초");
        
        // 데미지 적용
        enemy.TakeDamage(damage);
    }
    

    // ========================================
    // 공격 판정
    // ========================================

        public void OnAttackHit()
    {
        // ========== 현재 스킬 가져오기 ==========
        AttackSkillData skill = comboSocket.GetCurrentSkill();
        if (skill == null)
        {
            Debug.Log("공격 스킬이 없습니다!");
            return;
        }

        // ========== 공격 범위 중심점 계산 ==========
        Vector3 attackPosition = transform.position + transform.forward * attackDistance;

        // ========== Enemy 감지 ==========
        Collider[] hitColliders = Physics.OverlapSphere(
            attackPosition,
            attackRange,
            LayerMask.GetMask("Enemy")
        );

        Debug.Log($"공격 범위 내 Enemy: {hitColliders.Length}명");

        foreach (Collider col in hitColliders)
        {
            EnemyController enemy = col.GetComponent<EnemyController>();

            if (enemy != null)
            {
                // ========== 데미지 계산 ==========
                float damage = CalculateDamage(skill);
                float stunDuration = skill.stunDuration;

                Debug.Log($"Enemy 타격! 데미지: {damage}, 스턴: {stunDuration}초");

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
        currentHealth -= damage;

        Debug.Log($"Player 피격! 데미지: {damage}, 남은 체력: {currentHealth}");
        hPBar.SetHP(currentHealth, playerData.maxHealth);

        // 사망 체크
        if (currentHealth <= 0)
        {
            currentHealth = 0;
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
            $"HP: {currentHealth:F0} / {playerData.maxHealth:F0}", labelStyle);
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
