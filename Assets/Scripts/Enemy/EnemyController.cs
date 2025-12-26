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
    private bool justChangedState = false;
    private bool isStunFinished = false;


    private StateMachine stateMachine;
    private IdleState idleState;
    private ChaseState chaseState;
    private AttackState attackState;
    private DashState dashState;
    private StunState stunState;
    private DeathState deathState;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (data.animatorController != null)
        {
            animator.runtimeAnimatorController = data.animatorController;
        }

        stateMachine = new StateMachine();
        idleState = new IdleState(this);
        chaseState = new ChaseState(this);
        attackState = new AttackState(this);
        dashState = new DashState(this);  // ← 주석 해제!
        stunState = new StunState(this);
        deathState = new DeathState(this);

        stateMachine.ChangeState(idleState);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            TakeStun();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Die();
        }

        // 상태 전환 로직만
        if (justChangedState)
        {
            justChangedState = false;
            return;
        }

        HandleStateTransitions();
    }

    void FixedUpdate()
    {
        // 물리 관련 업데이트
        stateMachine.Update();
    }

    // ← 이 함수 추가!
    public void OnAttackFinished()
    {
        if (isAttackFinished) return;  // ← 이미 true면 무시
        isAttackFinished = true;
    }

    public void OnStunFinished()
    {
        isStunFinished = true;
    }

    // 기절 진입 (Health에서 호출)
    public void TakeStun()
    {
        Debug.Log("기절 당함!");

        if (stateMachine.CurrentState == stunState)
        {
            Debug.Log("이미 기절 중!");
            return;
        }

        if (stateMachine.CurrentState == attackState ||
            stateMachine.CurrentState == dashState)
        {
            Debug.Log("공격/대시 중단! 강제 기절!");
            isAttackFinished = true;

            // ========== Animator를 Movement로 강제 전환 ==========
            animator.Play("Move", 0, 0);  // Layer 0, normalizedTime 0
        }

        stateMachine.ChangeState(stunState);
        isStunFinished = false;
    }

    public void Die()
    {
        Debug.Log("사망 처리!");

        // ========== 현재 상태 강제 중단 ==========
        if (stateMachine.CurrentState == attackState ||
            stateMachine.CurrentState == dashState)
        {
            Debug.Log("공격/대시 중단! 강제 사망!");
            isAttackFinished = true;

            // Animator를 Movement로 강제 전환
            animator.Play("Move", 0, 0);
        }

        if (stateMachine.CurrentState == stunState)
        {
            Debug.Log("기절 중단! 강제 사망!");
            isStunFinished = true;

            // Animator를 Movement로 강제 전환
            animator.Play("Move", 0, 0);
        }

        // 모든 Trigger 리셋
        animator.ResetTrigger("1ATTACK");
        animator.ResetTrigger("2ATTACK");
        animator.ResetTrigger("Dash");
        animator.ResetTrigger("Stun");

        stateMachine.ChangeState(deathState);
    }

    private void HandleStateTransitions()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        State targetState = null;

        // ========== 사망 중이면 아무것도 안 함 ==========
        if (stateMachine.CurrentState == deathState)
            return;

        // ========== 기절 중이면 대기 ==========
        if (stateMachine.CurrentState == stunState && !isStunFinished)
            return;

        // 기절 끝났으면 플래그 리셋
        if (isStunFinished)
        {
            isStunFinished = false;
        }

        // 공격/대시 중이면 대기
        if ((stateMachine.CurrentState == attackState ||
             stateMachine.CurrentState == dashState) &&
            !isAttackFinished)
            return;

        // ========== 1. 공격 사거리 ==========
        if (distance <= data.attackRange)
        {
            targetState = attackState;
            isAttackFinished = false;
            dashProbability = 0f;
        }

        // ========== 2. 대쉬 사거리 ==========
        else if (data.canDash &&
                 distance > data.attackRange &&
                 distance <= data.dashRange)
        {
            dashProbability += Time.deltaTime * 0.5f;

            if (Random.value < dashProbability)
            {
                targetState = dashState;
                isAttackFinished = false;
                dashProbability = 0f;
            }
            else
            {
                targetState = chaseState;
            }
        }

        // ========== 3. 추적 ==========
        else
        {
            targetState = chaseState;
            dashProbability = 0f;
        }

        if (stateMachine.CurrentState != targetState)
        {
            stateMachine.ChangeState(targetState);
            justChangedState = true;
        }
    }


}



