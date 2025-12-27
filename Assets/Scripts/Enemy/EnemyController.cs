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

    private float dashProbability = 0f;

    // ========== IEnemy 구현 ==========
    public Transform Transform => transform;
    public Transform Player => player;
    public Rigidbody Rigidbody => rb;
    public Animator Animator => animator;
    public EnemyData Data => data;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (data.animatorController != null)
        {
            animator.runtimeAnimatorController = data.animatorController;
        }

        stateMachine = new StateMachine();

        // 필수 State
        idleState = new IdleState(this);
        chaseState = new ChaseState(this);
        attackState = new AttackState(this);
        stunState = new StunState(this);
        deathState = new DeathState(this);

        // 선택 State
        if (data.canDash)
        {
            dashState = new DashState(this);
            Debug.Log($"{data.enemyName}: DashState 활성화!");
        }

        stateMachine.ChangeState(idleState);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeStun();
        }

        Debug.Log($"Current State: {stateMachine.CurrentState.GetType().Name}, IsFinished: {stateMachine.CurrentState.IsFinished}");

        HandleStateTransitions();
    }

    void FixedUpdate()
    {
        stateMachine.Update();
    }

    private void HandleStateTransitions()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        State targetState = null;

        // ========== 사망 중이면 대기 ==========
        if (stateMachine.CurrentState == deathState)
            return;

        // ========== 현재 State가 완료 안 됐으면 대기 ==========
        if (!stateMachine.CurrentState.IsFinished)
        {
            Debug.Log("State not finished, waiting...");
            return;
        }

        // ========== 상태 전환 로직 ==========
        if (distance <= data.attackRange)
        {
            targetState = attackState;
            dashProbability = 0f;
        }
        else if (data.canDash &&
                 distance > data.attackRange &&
                 distance <= data.dashRange)
        {
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
        else
        {
            targetState = chaseState;
            dashProbability = 0f;
        }

        if (stateMachine.CurrentState != targetState)
        {
            stateMachine.ChangeState(targetState);
        }
    }

    // ========== 기절 진입 ==========
    public void TakeStun()
    {
        Debug.Log("기절 당함!");

        if (stateMachine.CurrentState == stunState)
        {
            Debug.Log("이미 기절 중!");
            return;
        }

        if (stateMachine.CurrentState == attackState ||
            (data.canDash && stateMachine.CurrentState == dashState))
        {
            Debug.Log("공격/대시 중단! 강제 기절!");
            animator.Play(AnimationConstants.MOVE_STATE, 0, 0);
        }

        foreach (string attackTrigger in data.enabledAttacks)
        {
            animator.ResetTrigger(attackTrigger);
        }
        animator.ResetTrigger(AnimationConstants.DASH_TRIGGER);

        stateMachine.ChangeState(stunState);
    }

    // ========== 사망 진입 ==========
    public void Die()
    {
        Debug.Log("사망 처리!");

        if (stateMachine.CurrentState == attackState ||
            (data.canDash && stateMachine.CurrentState == dashState))
        {
            Debug.Log("공격/대시 중단! 강제 사망!");
            animator.Play(AnimationConstants.MOVE_STATE, 0, 0);
        }

        if (stateMachine.CurrentState == stunState)
        {
            Debug.Log("기절 중단! 강제 사망!");
            animator.Play(AnimationConstants.MOVE_STATE, 0, 0);
        }

        foreach (string attackTrigger in data.enabledAttacks)
        {
            animator.ResetTrigger(attackTrigger);
        }
        animator.ResetTrigger(AnimationConstants.DASH_TRIGGER);
        animator.ResetTrigger(AnimationConstants.STUN_TRIGGER);

        stateMachine.ChangeState(deathState);
    }
}