using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player;          // 플레이어 위치
    public float attackRange = 2f;    // 공격 사거리
    public float dashRange = 5f;      // 돌진 공격 가능 거리

    public bool isLowHP = false;      // 체력 낮음 상태
    public bool isEnraged = false;    // 분노 상태

    private StateMachine stateMachine;

    // 상태 객체
    private IdleState idleState;
    private ChaseState chaseState;
    private AttackState attackState;
    //private DashAttackState dashAttackState;

    private void Awake()
    {
        stateMachine = new StateMachine();

        // 상태 생성
        idleState = new IdleState(this);
        chaseState = new ChaseState(this);
        attackState = new AttackState(this);
        //dashAttackState = new DashAttackState(this);

        // 시작 상태
        stateMachine.ChangeState(idleState);
    }

    private void Update()
    {
        HandleStateTransitions();
        stateMachine.Update();
    }

    void HandleStateTransitions()
{
    float distance = Vector3.Distance(transform.position, player.position);
    
    State targetState = null;

    if (distance <= attackRange)
        targetState = attackState;
    else if (distance <= dashRange && CanDashAttack())
        {
        //targetState = dashAttackState;
            
        }
    else
        targetState = chaseState;

    // 상태가 실제로 바뀔 때만 전환
    if (stateMachine.CurrentState != targetState)
    {
        stateMachine.ChangeState(targetState);
    }
}
    private bool CanDashAttack()
    {
        // 예시: 분노 상태일 때만 돌진 가능
        return isEnraged; 
    }
}
