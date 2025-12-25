using UnityEngine;

public class EnemyController : MonoBehaviour, IEnemy
{
    [Header("References")]
    public Transform player;
    public EnemyData data;

    // 컴포넌트 캐싱
    private Animator animator;
    private Rigidbody rb;

    // IEnemy 인터페이스 구현
    public Transform Transform => transform;
    public Transform Player => player;
    public Animator Animator => animator;
    public Rigidbody Rigidbody => rb;
    public EnemyData Data => data;

    // 기존 변수들
    public bool isEnraged = false;
    private bool isAttackFinished = false;
    private float dashProbability = 0f;

    private StateMachine stateMachine;
    private IdleState idleState;
    private ChaseState chaseState;
    private AttackState attackState;
    //private DashState dashState;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // Animator Controller 교체
        if (data.animatorController != null)
        {
            animator.runtimeAnimatorController = data.animatorController;
        }

        stateMachine = new StateMachine();
        idleState = new IdleState(this);
        chaseState = new ChaseState(this);
        attackState = new AttackState(this);
        //dashState = new DashState(this);
        stateMachine.ChangeState(idleState);
    }

    private bool justChangedState = false;


    private void Update()
    {
        if (justChangedState)
        {
            justChangedState = false;
            stateMachine.Update();
            return;  // Transition 스킵
        }
        HandleStateTransitions();
        stateMachine.Update();
    }

    // ← 이 함수 추가!
    public void OnAttackFinished()
    {
        if (isAttackFinished) return;  // ← 이미 true면 무시
        isAttackFinished = true;
    }

    private void HandleStateTransitions()
    {
        Debug.Log($"[Transitions] Current: {stateMachine.CurrentState?.GetType().Name}");
        float distance = Vector3.Distance(transform.position, player.position);

        State targetState = null;

        // 공격 중이면 대기
        if (stateMachine.CurrentState == attackState && !isAttackFinished)
            return;

        // ========== 1. 공격 사거리 ==========
        if (distance <= data.attackRange)
        {
            targetState = attackState;
            isAttackFinished = false;
            dashProbability = 0f;
        }

        // ========== 2. 추적 ==========
        else
        {
            targetState = chaseState;
            dashProbability = 0f;
        }

        // ========== 상태 변경 (1번만!) ==========
        if (stateMachine.CurrentState != targetState)
        {
            Debug.Log($"[Transitions] Change: {stateMachine.CurrentState?.GetType().Name} → {targetState?.GetType().Name}");
            stateMachine.ChangeState(targetState);
        }
    }


}



