using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy 제어 클래스
/// 
/// 역할:
/// - State Machine 관리 및 State 전환
/// - Player 거리 기반 행동 선택 (Idle, Chase, Attack, Dash)
/// - 외부 효과 처리 (데미지, 스턴, 사망)
/// - IEnemy 인터페이스 구현으로 State들에게 기능 제공
/// 
/// 구조:
/// - Dictionary로 State 탈출 조건 관리 (확장성)
/// - 매 프레임 거리 계산 후 재사용 (최적화)
/// - 확률적 Dash 선택 (시간 기반 확률 증가)
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour, IEnemy
{
    // ========================================
    // Inspector 설정
    // ========================================

    [Header("References")]
    [Tooltip("추적할 Player Transform")]
    [SerializeField] private Transform player;

    [Tooltip("Enemy 데이터 (체력, 속도, 공격력 등)")]
    [SerializeField] private EnemyData data;

    // ========================================
    // 컴포넌트 (자동 할당)
    // ========================================

    private Rigidbody rb;
    private Animator animator;

    // ========================================
    // State Machine
    // ========================================

    private StateMachine stateMachine;


    // HP Bar
    public EnemyHpBacUI enemyHpBacUI;

    // ========================================
    // States
    // ========================================

    private IdleState idleState;
    private ChaseState chaseState;
    private AttackState attackState;
    private DashState dashState;
    private StunState stunState;
    private DeathState deathState;

    // ========================================
    // State 전환 관리
    // ========================================

    /// <summary>
    /// State별 탈출 조건 체크 함수 저장
    /// 
    /// 사용:
    /// - Dictionary로 관리하여 확장성 향상
    /// - 새 State 추가 시 여기만 등록하면 됨
    /// 
    /// 예시:
    /// - ChaseState: Player 공격 범위 진입 시 Idle로
    /// - PatrolState: Player 발견 시 Chase로 (나중에)
    /// </summary>
    private Dictionary<State, System.Action> stateExitCheckers;

    /// <summary>
    /// 현재 Player와의 거리
    /// 
    /// 용도:
    /// - Update()에서 한 번만 계산
    /// - 여러 곳에서 재사용 (최적화)
    /// </summary>
    private float currentDistanceToPlayer;

    /// <summary>
    /// Dash 확률 누적값
    /// 
    /// 동작:
    /// - dashRange 안에서 시간이 지날수록 증가
    /// - Dash 실행 또는 범위 벗어나면 0으로 리셋
    /// - 시간 기반 확률로 자연스러운 Dash 타이밍
    /// </summary>
    private float dashProbability = 0f;

    // ========================================
    // 전투 상태
    // ========================================

    /// <summary>
    /// 현재 체력
    /// 
    /// 관리:
    /// - TakeDamage()로 감소
    /// - 0 이하 시 사망
    /// - EnemyData.maxHealth로 초기화
    /// </summary>
    private float currentHealth;

    // ========================================
    // IEnemy 인터페이스 구현
    // ========================================

    public Transform Transform => transform;
    public Transform Player => player;
    public Rigidbody Rigidbody => rb;
    public Animator Animator => animator;
    public EnemyData Data => data;

    /// <summary>
    /// IdleState로 복귀
    /// State들이 완료 후 호출
    /// </summary>
    public void ChangeToIdle()
    {
        stateMachine.ChangeState(idleState);
    }

    // ========================================
    // Unity 생명주기
    // ========================================

    void Awake()
    {
        // 컴포넌트 자동 할당
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // State Machine 초기화
        stateMachine = new StateMachine();

        // State 인스턴스 생성
        idleState = new IdleState(this);
        chaseState = new ChaseState(this);
        attackState = new AttackState(this);
        dashState = new DashState(this);
        stunState = new StunState(this);
        deathState = new DeathState(this);

        // ========================================
        // State 탈출 조건 Dictionary 설정
        // ========================================
        // 새 State 추가 시 여기에 등록
        // 예: { patrolState, CheckPatrolExit }
        stateExitCheckers = new Dictionary<State, System.Action>
        {
            { chaseState, CheckChaseExit }
        };

        // ========================================
        // 체력 초기화
        // ========================================
        currentHealth = data.baseHealth;

        // 초기 State 설정
        stateMachine.ChangeState(idleState);
    }

    void Start()
{
    // ... 기존 코드 ...
    
    // ========== Layer 확인 ==========
    Debug.Log("=== Layer 정보 ===");
    for (int i = 0; i < 32; i++)
    {
        string layerName = LayerMask.LayerToName(i);
        if (!string.IsNullOrEmpty(layerName))
        {
            Debug.Log($"Layer {i}: {layerName}");
        }
    }
    Debug.Log("==================");
}



    void Update()
    {

        // L키: 적 죽음
        if (Input.GetKeyDown(KeyCode.L))
        {
           Die();
        }
        // ========================================
        // 거리 계산 (한 번만)
        // ========================================
        // Update()에서 한 번 계산 후
        // 여러 메서드에서 재사용 (최적화)
        currentDistanceToPlayer = Vector3.Distance(transform.position, player.position);

        // ========================================
        // State별 조건 체크
        // ========================================

        // IdleState: 다음 State 선택
        if (stateMachine.CurrentState == idleState)
        {
            SelectNextState();
        }
        // 등록된 State: 탈출 조건 체크
        else if (stateExitCheckers.TryGetValue(stateMachine.CurrentState, out var exitChecker))
        {
            exitChecker();
        }
        // 다른 State (Attack, Dash, Stun, Death):
        // - 알아서 완료 후 IdleState로 복귀
        // - 별도 체크 불필요
    }

    void FixedUpdate()
    {
        // ========================================
        // State Machine Update
        // ========================================
        // 현재 State의 Execute() 실행
        // FixedUpdate에서 호출 (물리 처리와 동기화)
        stateMachine.Update();
    }

    // ========================================
    // State 선택 로직
    // ========================================

    /// <summary>
    /// IdleState에서 다음 State 선택
    /// 
    /// 우선순위:
    /// 1. 공격 범위 안 → Attack
    /// 2. Dash 범위 안 → Dash (확률적) 또는 Chase
    /// 3. 그 외 → Chase
    /// 
    /// Dash 확률:
    /// - dashRange 안에서 시간 지남에 따라 증가
    /// - 자연스러운 Dash 타이밍 연출
    /// </summary>
    private void SelectNextState()
    {
        // ========================================
        // 1. 공격 범위 안: 공격
        // ========================================
        if (currentDistanceToPlayer <= data.attackRange)
        {
            stateMachine.ChangeState(attackState);
        }
        // ========================================
        // 2. Dash 범위 안: Dash 또는 Chase
        // ========================================
        else if (data.canDash &&
                 currentDistanceToPlayer > data.attackRange &&
                 currentDistanceToPlayer <= data.dashRange)
        {
            // Dash 확률 증가 (시간에 따라)
            // 100f = 초당 100% 증가율
            dashProbability += Time.deltaTime * 100f;

            // 확률적으로 Dash 선택
            // Random.value: 0.0 ~ 1.0
            if (Random.value < dashProbability)
            {
                stateMachine.ChangeState(dashState);
                dashProbability = 0f; // 확률 리셋
            }
            else
            {
                stateMachine.ChangeState(chaseState);
            }
        }
        // ========================================
        // 3. 그 외: 추격
        // ========================================
        else
        {
            stateMachine.ChangeState(chaseState);
            dashProbability = 0f; // 확률 리셋
        }
    }

    /// <summary>
    /// ChaseState 탈출 조건 체크
    /// 
    /// 조건:
    /// - Player가 공격 범위 안에 들어옴
    /// 
    /// 동작:
    /// - IdleState로 복귀
    /// - 다음 프레임 SelectNextState()에서 Attack 선택됨
    /// </summary>
    private void CheckChaseExit()
    {
        // 공격 범위 진입: Idle로
        if (currentDistanceToPlayer <= data.attackRange)
        {
            stateMachine.ChangeState(idleState);
        }
    }

    // ========================================
    // 외부 호출 메서드 - 전투
    // ========================================

    /// <summary>
    /// 데미지 받기
    /// </summary>
    /// <param name="damage">받을 데미지 양</param>
    public void TakeDamage(float damage)
    {
        // 이미 사망 상태면 무시
        if (stateMachine.CurrentState == deathState)
            return;

        // 체력 감소
        currentHealth -= damage;

        Debug.Log($"{gameObject.name} 피격! 데미지: {damage}, 남은 체력: {currentHealth}/{data.baseHealth}");
        enemyHpBacUI.SetHP(currentHealth, data.baseHealth);

        // 사망 체크
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    /// <summary>
    /// 스턴 적용
    /// 
    /// 호출:
    /// - Player 공격 시 PlayerController.OnAttackHit()에서 호출
    /// 
    /// 동작:
    /// - TakeStun() 호출
    /// - StunState로 강제 전환
    /// 
    /// 파라미터:
    /// - duration: 현재 사용 안 함 (StunState에서 애니메이션 길이로 자동 계산)
    /// - 나중에 필요하면 StunState에 전달 가능
    /// </summary>
    /// <param name="duration">스턴 지속 시간 (현재 미사용)</param>
    public void ApplyStun(float duration)
    {
        // TakeStun() 호출
        // duration은 나중에 StunState 개선 시 사용 가능
        TakeStun();

        Debug.Log($"{gameObject.name} 스턴 적용! (지속: {duration}초)");
    }

    /// <summary>
    /// 기절 효과 적용
    /// 
    /// 동작:
    /// 1. 진행 중인 애니메이션 강제 중단
    /// 2. 모든 Trigger 리셋
    /// 3. StunState로 강제 전환
    /// 
    /// 특징:
    /// - 어떤 State에서든 즉시 전환
    /// - 사망 상태만 예외 (무시)
    /// </summary>
    public void TakeStun()
    {
        // 사망 상태면 무시
        if (stateMachine.CurrentState == deathState)
            return;

        // ========================================
        // 진행 중인 애니메이션 중단
        // ========================================
        // MOVE_STATE로 강제 전환 (기본 상태)
        // normalizedTime: 0 (시작 지점)
        animator.Play(AnimationConstants.HIT, 0, 0);

        // ========================================
        // 모든 Trigger 리셋
        // ========================================
        // 공격 Trigger들
        foreach (string attackTrigger in data.enabledAttacks)
        {
            animator.ResetTrigger(attackTrigger);
        }
        // 기타 Trigger들
        animator.ResetTrigger(AnimationConstants.DASH_TRIGGER);
        animator.ResetTrigger(AnimationConstants.STUN_TRIGGER);

        // ========================================
        // 기절 상태로 강제 전환
        // ========================================
        stateMachine.ChangeState(stunState);

        Debug.Log($"{gameObject.name} 기절!");
    }


/// <summary>
/// 사망 처리
/// 
/// 호출:
/// - TakeDamage()에서 체력 0 시 자동 호출
/// - 또는 직접 호출 (즉사 기믹 등)
/// 
/// 동작:
/// 1. 진행 중인 애니메이션 강제 중단
/// 2. 모든 Trigger 리셋
/// 3. DeathState로 강제 전환
/// 4. StageManager에 사망 알림
/// 
/// 특징:
/// - 이미 사망 상태면 무시 (중복 방지)
/// - 보스: 포탈 바로 활성화
/// - 일반 몹: 처치 카운트 증가
/// </summary>
public void Die()
{
    // 이미 사망 상태면 무시
    if (stateMachine.CurrentState == deathState)
        return;

    // 진행 중인 애니메이션 중단
    animator.Play(AnimationConstants.HIT, 0, 0);

    // 모든 Trigger 리셋
    foreach (string attackTrigger in data.enabledAttacks)
        animator.ResetTrigger(attackTrigger);

    animator.ResetTrigger(AnimationConstants.DASH_TRIGGER);
    animator.ResetTrigger(AnimationConstants.STUN_TRIGGER);
    animator.ResetTrigger(AnimationConstants.DEATH_TRIGGER);

    // 사망 상태로 전환
    stateMachine.ChangeState(deathState);

    Debug.Log($"{gameObject.name} 사망!");

    // StageManager 알림
    if (StageManager.Instance == null)
    {
        Debug.LogWarning("[EnemyController] StageManager.Instance null");
        return;
    }

    // ✅ 보스 / 일반몹 분기 (중복 호출 금지)
    if (data != null && data.enemyType == EnemyType.Boss)
    {
        Debug.Log($"[{gameObject.name}] 보스 처치 → NotifyBossKilled()");
        StageManager.Instance.NotifyBossKilled();
    }
    else
    {
        Debug.Log($"[{gameObject.name}] 일반 몹 처치 → NotifyEnemyKilled()");
        StageManager.Instance.NotifyEnemyKilled();
    }
}


public void SetEnemyData(EnemyData newData)
{
    if (newData == null)
    {
        Debug.LogError("[EnemyController] SetEnemyData: newData null");
        return;
    }

    data = newData;

    // 체력 재초기화(EnemyController 내부 변수명 기준)
    currentHealth = data.baseHealth;

    Debug.Log($"[EnemyController] EnemyData 교체됨: hp={data.baseHealth}");
}



}