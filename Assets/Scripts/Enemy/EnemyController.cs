using UnityEngine;

public class EnemyController : MonoBehaviour, IEnemy
{
    [SerializeField] private EnemyData data;
    [SerializeField] private Transform player;

    private Animator animator;
    private Rigidbody rb;

    private StateMachine stateMachine;

    private IdleState idleState;
    private ChaseState chaseState;
    private AttackState attackState;
    private DashState dashState;
    private StunState stunState;
    private DeathState deathState;

    private float dashProbability = 0f; // 대시 확률

    // ========== IEnemy 인터페이스 구현 ==========
    public Transform Transform => transform;
    public Transform Player => player;
    public Rigidbody Rigidbody => rb;
    public Animator Animator => animator;
    public EnemyData Data => data;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // EnemyData에서 설정한 Animator Controller 적용
        if (data.animatorController != null)
        {
            animator.runtimeAnimatorController = data.animatorController;
        }

        stateMachine = new StateMachine();

        // 모든 Enemy가 가지는 필수 State 초기화
        idleState = new IdleState(this);
        chaseState = new ChaseState(this);
        attackState = new AttackState(this);
        stunState = new StunState(this);
        deathState = new DeathState(this);

        // EnemyData 설정에 따라 선택적으로 활성화되는 State
        if (data.canDash)
        {
            dashState = new DashState(this);
        }

        // 초기 상태를 Idle로 설정
        stateMachine.ChangeState(idleState);
    }

    void Update()
    {

        // 테스트용: K키로 기절 테스트
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeStun();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Die();
        }
        if (stateMachine.CurrentState.IsFinished)
        {
            HandleStateTransitions();
        }

        Debug.Log($"[{gameObject.name}] Current State: {stateMachine.CurrentState.GetType().Name}, IsFinished: {stateMachine.CurrentState.IsFinished}");
    }

    void FixedUpdate()
    {
        // 물리 업데이트 주기에 맞춰 현재 State의 Execute 실행
        stateMachine.Update();
    }

    private void HandleStateTransitions()
    {

        float distance = Vector3.Distance(transform.position, player.position);
        State targetState = null;
        Debug.Log($"[{gameObject.name}] Distance: {distance}, attackRange: {data.attackRange}, dashRange: {data.dashRange}");
        // 사망 상태에서는 더 이상 전환 없음
        if (stateMachine.CurrentState == deathState)
            return;

        // ========== 거리 기반 상태 전환 로직 ==========

        // 공격 범위 안: 공격 상태
        if (distance <= data.attackRange)
        {
            targetState = attackState;
            dashProbability = 0f;
        }
        // 대시 범위 안: 확률적으로 대시 또는 추적
        else if (data.canDash &&
                 distance > data.attackRange &&
                 distance <= data.dashRange)
        {
            // 시간이 지날수록 대시 확률 증가
            dashProbability += Time.deltaTime * 0.5f;

            if (Random.value < dashProbability)
            {
                targetState = dashState;
                dashProbability = 0f;
            }
            else
            {
                targetState = chaseState;
            }
        }
        // 그 외: 추적 상태
        else
        {
            targetState = chaseState;
            dashProbability = 0f;
        }

        // 다른 State로 전환하거나, 같은 State여도 완료되었으면 재진입
        if (stateMachine.CurrentState != targetState)
        {
            stateMachine.ChangeState(targetState);
        }
    }

    // ========== 외부에서 호출: 기절 상태로 강제 전환 ==========
    public void TakeStun()
    {
        // 이미 기절 중이면 무시
        if (stateMachine.CurrentState == stunState)
            return;

        // 공격/대시 중이었다면 애니메이션 강제 중단
        if (stateMachine.CurrentState == attackState ||
            (data.canDash && stateMachine.CurrentState == dashState))
        {
            animator.Play(AnimationConstants.MOVE_STATE, 0, 0);
        }

        // 설정된 모든 Trigger 리셋
        foreach (string attackTrigger in data.enabledAttacks)
        {
            animator.ResetTrigger(attackTrigger);
        }
        animator.ResetTrigger(AnimationConstants.DASH_TRIGGER);

        // 기절 상태로 전환
        stateMachine.ChangeState(stunState);
    }

    // ========== 외부에서 호출: 사망 상태로 강제 전환 ==========
    public void Die()
    {
        // 진행 중인 애니메이션 강제 중단
        if (stateMachine.CurrentState == attackState ||
            (data.canDash && stateMachine.CurrentState == dashState))
        {
            animator.Play(AnimationConstants.MOVE_STATE, 0, 0);
        }

        if (stateMachine.CurrentState == stunState)
        {
            animator.Play(AnimationConstants.MOVE_STATE, 0, 0);
        }

        // 모든 Trigger 리셋
        foreach (string attackTrigger in data.enabledAttacks)
        {
            animator.ResetTrigger(attackTrigger);
        }
        animator.ResetTrigger(AnimationConstants.DASH_TRIGGER);
        animator.ResetTrigger(AnimationConstants.STUN_TRIGGER);

        // 사망 상태로 전환
        stateMachine.ChangeState(deathState);
    }
}