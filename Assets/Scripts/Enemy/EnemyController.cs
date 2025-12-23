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
    
    private StateMachine stateMachine;
    private IdleState idleState;
    private ChaseState chaseState;
    private AttackState attackState;

    private void Awake()
    {
        // 컴포넌트 캐싱
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        
        stateMachine = new StateMachine();
        idleState = new IdleState(this);
        chaseState = new ChaseState(this);
        attackState = new AttackState(this);
        stateMachine.ChangeState(idleState);
    }

    private void Update()
    {
        HandleStateTransitions();
        stateMachine.Update();
    }

    // ← 이 함수 추가!
    public void OnAttackFinished()
    {
        isAttackFinished = true;
    }

    private void HandleStateTransitions()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        
        State targetState = null;

        // 공격 중이면 대기
        if (stateMachine.CurrentState == attackState && !isAttackFinished)
            return;

        // data.attackRange 사용 (수정!)
        if (distance <= data.attackRange)
        {
            targetState = attackState;
            isAttackFinished = false;
        }
        else
        {
            targetState = chaseState;
        }

        if (stateMachine.CurrentState != targetState)
        {
            stateMachine.ChangeState(targetState);
        }
    }
}