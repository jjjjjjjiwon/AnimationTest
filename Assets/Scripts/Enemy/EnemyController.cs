using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy 제어 클래스
/// State Machine 관리 및 State 전환 조건 판단
/// IEnemy 인터페이스를 구현하여 State들에게 필요한 기능 제공
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour, IEnemy
{
    // ========== Inspector 설정 ==========

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private EnemyData data;

    // ========== 컴포넌트 (자동 할당) ==========

    private Rigidbody rb;
    private Animator animator;

    // ========== State Machine ==========

    private StateMachine stateMachine;

    // ========== States ==========

    private IdleState idleState;
    private ChaseState chaseState;
    private AttackState attackState;
    private DashState dashState;
    private StunState stunState;
    private DeathState deathState;

    // ========== State 전환 관리 ==========

    /// <summary>
    /// State별 탈출 조건 체크 함수를 저장
    /// Dictionary로 관리하여 확장성 향상
    /// </summary>
    private Dictionary<State, System.Action> stateExitCheckers;

    /// <summary>
    /// 현재 Player와의 거리 (매 프레임 계산, 재사용)
    /// </summary>
    private float currentDistanceToPlayer;

    /// <summary>
    /// Dash 확률 누적값
    /// dashRange 안에서 시간이 지날수록 증가
    /// </summary>
    private float dashProbability = 0f;

    // ========== IEnemy 구현 ==========

    public Transform Transform => transform;
    public Transform Player => player;
    public Rigidbody Rigidbody => rb;
    public Animator Animator => animator;
    public EnemyData Data => data;

    public void ChangeToIdle()
    {
        stateMachine.ChangeState(idleState);
    }

    // ========== Unity 생명주기 ==========

    void Awake()
    {
        // ========== 컴포넌트 자동 할당 ==========
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // ========== State Machine 초기화 ==========
        stateMachine = new StateMachine();

        // ========== State 생성 ==========
        idleState = new IdleState(this);
        chaseState = new ChaseState(this);
        attackState = new AttackState(this);
        dashState = new DashState(this);
        stunState = new StunState(this);
        deathState = new DeathState(this);

        // ========== State 탈출 조건 Dictionary 설정 ==========
        stateExitCheckers = new Dictionary<State, System.Action>
        {
            { chaseState, CheckChaseExit }
            // 새 State 추가 시 여기에 등록
            // 예: { patrolState, CheckPatrolExit }
        };

        // ========== 초기 State 설정 ==========
        stateMachine.ChangeState(idleState);
    }

    void Update()
    {
        // ========== 테스트용 입력 (나중에 제거) ==========
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeStun();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Die();
        }

        // ========== 거리 계산 (한 번만) ==========
        currentDistanceToPlayer = Vector3.Distance(transform.position, player.position);

        // ========== State별 조건 체크 ==========

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
        // 다른 State (Attack, Dash, Stun, Death): 알아서 완료 후 IdleState로
    }

    void FixedUpdate()
    {
        // ========== State Machine Update (Execute 실행) ==========
        stateMachine.Update();
    }

    // ========== State 선택 로직 ==========

    /// <summary>
    /// IdleState에서 다음 State 선택
    /// 거리와 조건에 따라 Attack, Dash, Chase 중 선택
    /// </summary>
    private void SelectNextState()
    {
        // 공격 범위 안: 공격
        if (currentDistanceToPlayer <= data.attackRange)
        {
            stateMachine.ChangeState(attackState);
        }
        // Dash 범위 안: Dash 또는 Chase
        else if (data.canDash &&
                 currentDistanceToPlayer > data.attackRange &&
                 currentDistanceToPlayer <= data.dashRange)
        {
            // Dash 확률 증가 (시간에 따라)
            dashProbability += Time.deltaTime * 10f;

            // 확률적으로 Dash 선택
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
        // 그 외: 추격
        else
        {
            stateMachine.ChangeState(chaseState);
            dashProbability = 0f; // 확률 리셋
        }
    }

    /// <summary>
    /// ChaseState 탈출 조건 체크
    /// 공격 범위 진입 시 IdleState로 복귀
    /// </summary>
    private void CheckChaseExit()
    {
        // 공격 범위 진입: Idle로 (다음 프레임에 SelectNextState에서 Attack 선택됨)
        if (currentDistanceToPlayer <= data.attackRange)
        {
            stateMachine.ChangeState(idleState);
        }
    }

    // ========== 외부 호출 메서드 ==========

    /// <summary>
    /// 기절 효과 적용
    /// 어떤 State에서든 즉시 StunState로 전환
    /// </summary>
    public void TakeStun()
    {
        // 사망 상태면 무시
        if (stateMachine.CurrentState == deathState)
            return;

        // 진행 중인 애니메이션 중단
        animator.Play(AnimationConstants.MOVE_STATE, 0, 0);

        // 모든 Trigger 리셋
        foreach (string attackTrigger in data.enabledAttacks)
        {
            animator.ResetTrigger(attackTrigger);
        }
        animator.ResetTrigger(AnimationConstants.DASH_TRIGGER);
        animator.ResetTrigger(AnimationConstants.STUN_TRIGGER);

        // 기절 상태로 강제 전환
        stateMachine.ChangeState(stunState);

        Debug.Log("Enemy Stunned!");
    }

    /// <summary>
    /// 사망 처리
    /// 모든 행동 중단하고 DeathState로 전환
    /// </summary>
    public void Die()
    {
        // 이미 사망 상태면 무시
        if (stateMachine.CurrentState == deathState)
            return;

        // 진행 중인 애니메이션 중단
        animator.Play(AnimationConstants.MOVE_STATE, 0, 0);

        // 모든 Trigger 리셋
        foreach (string attackTrigger in data.enabledAttacks)
        {
            animator.ResetTrigger(attackTrigger);
        }
        animator.ResetTrigger(AnimationConstants.DASH_TRIGGER);
        animator.ResetTrigger(AnimationConstants.STUN_TRIGGER);
        animator.ResetTrigger(AnimationConstants.DEATH_TRIGGER);

        // 사망 상태로 강제 전환
        stateMachine.ChangeState(deathState);

        Debug.Log("Enemy Died!");
    }
}