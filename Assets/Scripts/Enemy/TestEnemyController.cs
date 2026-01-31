using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestEnemyController : MonoBehaviour, IEnemy
{
    private StateMachine stateMachine;
    private Transform playerTransform; // 플레이어 위치 저장용
    private EnemyDataPackage runtimePackage;

    // Json
    private EnemyJsonData runtimeData;

    // IEnemy 인터페이스 구현 (프로퍼티)
    public Animator EnemyAnimator => animator;
    public Transform EnemyTransform => this.transform;
    public Rigidbody EnemyRigidbody => rb;
    public Transform Player => playerTransform;
    public EnemyDataPackage DataPackage => runtimePackage; // 상태들이 데이터를 꺼내 쓰는 통로
    public bool IsDead => dead;

    // 컴포넌트
    private Rigidbody rb;
    private Animator animator;
    private IdleState idleState;
    private ChaseState chaseState;
    private bool dead;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        stateMachine = new StateMachine();
    }

    void Start()
    {
        Debug.Log("컨트롤러 작동 시작!");
    }

    public void Setup(EnemyDataPackage package)
    {
        // 1. 패키지 저장 (이게 있어야 DataPackage 프로퍼티가 정상 작동합니다)
        this.runtimePackage = package;
        this.runtimeData = package.baseData;

        // 2. 플레이어 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerTransform = playerObj.transform;

        // ★ 3. Chase 상태 생성 (이게 빠져있었습니다!)
        if (package.chaseData != null)
        {
            this.chaseState = new ChaseState(this, package.chaseData);
        }
        else
        {
            Debug.LogError("패키지에 Chase 데이터가 없습니다!");
        }

        // 3. 상태 생성 및 시작
        this.idleState = new IdleState(this, package.idleData);
        stateMachine.ChangeState(idleState);
    }

    void Update()
    {
        stateMachine.Update();
        SelectNextState();
    }

    // IEnemy 인터페이스 구현 (로직)
    public void SelectNextState()
    {
        if (playerTransform == null) return; // 플레이어가 없으면 판단 중지

        // 거리 계산
        float distance = Vector3.Distance(transform.position, playerTransform.position);


        // // 1. 공격 판단 (AttackState가 있고, 사거리 안일 때)
        // if (attackState != null && distance <= runtimeData.base_Attack_Range)
        // {
        //     stateMachine.ChangeState(attackState);
        //     return;
        // }

        // 2. 추격 판단 (ChaseState가 있고, 감지 범위 안일 때)
        if (chaseState != null && distance > 3)
        {
            stateMachine.ChangeState(chaseState);
            Debug.Log("chasechasechasechasechasechasechasechasechasechasechasechasechasechasechasechasechasechasechasechasechasechasechasechase");
            return;
        }

        // 5. 아무 조건도 없으면 Idle 유지 (통로 역할 수행 중)
        stateMachine.ChangeState(idleState);
        //Debug.Log("Idle IdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdleIdle");

    }



}