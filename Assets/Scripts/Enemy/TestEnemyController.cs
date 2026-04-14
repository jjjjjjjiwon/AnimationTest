using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyController : MonoBehaviour, IEnemy
{
    protected StateMachine stateMachine;
    private Transform playerTransform;
    private EnemyDataPackage runtimePackage;
    private EnemyJsonData runtimeData;

    // IEnemy 인터페이스 구현
    public Animator EnemyAnimator => animator;
    public Transform EnemyTransform => this.transform;
    public Rigidbody EnemyRigidbody => rb;
    public Transform Player => playerTransform;
    public EnemyDataPackage DataPackage => runtimePackage;
    public bool IsDead => isDead;

    // 컴포넌트
    private Rigidbody rb;
    private Animator animator;

    // 상태들
    protected IdleState idleState;
    protected ChaseState chaseState;
    protected DashState dashState;
    protected TeleportState teleportState;
    protected AttackState attackState;
    protected StunState stunState;
    protected DeathState deathState;

    private float stunTimer = 0f;
    protected float currentHP;
    protected bool isDead = false;

    // --- [추가: 텔레포트 제어 변수] ---
    protected float teleportCooldownTimer = 0f;

    // 공격 받을때 이펙트
    private SkinnedMeshRenderer[] renderers; // 적의 몸뚱아리들
    private Color[] originalColors;         // 원래 색상들을 저장할 배열

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        stateMachine = new StateMachine();
        renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
originalColors = new Color[renderers.Length];

    for (int i = 0; i < renderers.Length; i++) 
    {
    originalColors[i] = renderers[i].material.color; // 원래 색 저장
    }
    }

    public virtual void Setup(EnemyDataPackage package)
    {
        if (package == null) return;

        this.runtimePackage = package;
        IEnemyData commonData = (package.bossData != null) ? (IEnemyData)package.bossData : (IEnemyData)package.baseData;

        this.currentHP = commonData.base_Health;
        this.runtimeData = package.baseData;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;
        if (animator == null) animator = GetComponentInChildren<Animator>();

        // 상태 생성
        if (package.idleData != null) this.idleState = new IdleState(this, package.idleData);
        if (package.chaseData != null) this.chaseState = new ChaseState(this, package.chaseData);

        // 텔레포트 초기 쿨타임 설정
        if (package.teleportData != null)
            teleportCooldownTimer = package.teleportData.cooldown;

        if (idleState != null) stateMachine.ChangeState(idleState);
    }

    void Update()
    {
        if (isDead)
        {
            stateMachine.CurrentState?.Execute();
            return;
        }

        // 1. 텔레포트 쿨타임 실시간 감소
        if (teleportCooldownTimer > 0)
            teleportCooldownTimer -= Time.deltaTime;

        // 2. 스턴 처리
        if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
            stateMachine.CurrentState?.Execute();
            if (stunTimer <= 0) SelectNextState();
            return;
        }

        // 3. 평상시 로직 실행
        stateMachine.CurrentState?.Execute();
        SelectNextState();
    }

    public void SelectNextState()
    {
        if (isDead) return;
        // 1. 공격 상태 방어막 (기존에 유저님이 만드신 것)
        if (stateMachine.CurrentState is AttackState attack && !attack.IsExiting) return;

        // 2. 텔레포트 상태 방어막 (새로 추가)
        // 현재 상태가 텔포인데, 아직 끝날 때(IsExiting)가 아니면 리턴!
        if (stateMachine.CurrentState is TeleportState teleport && !teleport.IsExiting) return;

        if (playerTransform == null) return;
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // --- [핵심 추가: 텔레포트 판단] ---
        if (runtimePackage.teleportData != null && teleportCooldownTimer <= 0)
        {
            var tData = runtimePackage.teleportData;
            // JSON 설정값(min, max) 범위 안에 플레이어가 있다면 실행
            if (distance >= tData.min_Teleport_Distance && distance <= tData.max_Trigger_Distance)
            {
                teleportCooldownTimer = tData.cooldown; // 쿨타임 리셋
                stateMachine.ChangeState(new TeleportState(this, tData));
                return;
            }
        }

        // --- [기존: 공격 판단 루프] ---
        List<EnemyComboJsonData> possibleCombos = new List<EnemyComboJsonData>();
        foreach (var combo in runtimePackage.comboList)
        {
            if (combo.motion_Steps_ID.Count > 0)
            {
                if (runtimePackage.motionDic.TryGetValue(combo.motion_Steps_ID[0], out var firstMotionData))
                {
                    if (distance <= firstMotionData.combo_Start_Range) possibleCombos.Add(combo);
                }
            }
        }

        if (possibleCombos.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, possibleCombos.Count);
            List<EnemyAttackMotionJsonData> motionSteps = new List<EnemyAttackMotionJsonData>();
            foreach (string mID in possibleCombos[randomIndex].motion_Steps_ID)
            {
                if (runtimePackage.motionDic.TryGetValue(mID, out var motionData)) motionSteps.Add(motionData);
            }
            stateMachine.ChangeState(new AttackState(this, motionSteps));
            return;
        }

        // --- [기존: 추격 및 대기] ---
        if (distance > 0.5f) // 거리를 좀 더 넉넉히 잡고
        {
            //Debug.Log($"<color=lime>[AI Decision]</color> 현재 거리 {distance:F1} -> ChaseState로 전환");
            stateMachine.ChangeState(chaseState);
        }
        else
        {
            //Debug.Log($"<color=orange>[AI Decision]</color> 현재 거리 {distance:F1} -> IdleState로 전환");
            stateMachine.ChangeState(idleState);
        }
    }

    // 외부(BossController 등)에서 쿨타임을 깎을 수 있게 열어둠
    public void AdjustTeleportCooldown(float amount)
    {
        teleportCooldownTimer = Mathf.Max(0, teleportCooldownTimer + amount);
    }

    #region TakeDamage & Death (기존 로직 유지)
    public void TakeDamage(float damage, float motionAddTime)
    {
        if (isDead) return;
        currentHP -= damage;

        if (currentHP <= 0) { currentHP = 0; Death(); return; }

        if (stunTimer > 0)
        {
            stunTimer += motionAddTime;
            if (stateMachine.CurrentState is StunState s) s.Enter();
        }
        else
        {
            stunTimer = runtimePackage.stunData.stun_Timer;
            stateMachine.ChangeState(new StunState(this, stunTimer));
        }
        StartCoroutine(HitFlashRoutine());
    }

    private IEnumerator HitFlashRoutine() 
    {
    // 1. 하얗게 만들기
    foreach (var r in renderers) r.material.color = Color.white;
    Debug.Log("Take Damage Take DamageTake DamageTake DamageTake DamageTake DamageTake DamageTake DamageTake DamageTake DamageTake DamageTake DamageTake DamageTake DamageTake Damage");
    
    // 2. 아주 짧게 대기 (역경직 시간과 비슷하게!)
    yield return new WaitForSeconds(0.1f);
    
    // 3. 원래 색으로 복구
    for (int i = 0; i < renderers.Length; i++) {
        renderers[i].material.color = originalColors[i];
    }
    }

    protected virtual void Death()
    {
        if (isDead) return;
        isDead = true;
        stunTimer = 0;
        if (StageManager.Instance != null) StageManager.Instance.NotifyEnemyKilled();
        if (rb != null) { rb.velocity = Vector3.zero; rb.isKinematic = true; }
        var col = GetComponent<Collider>();
        if (col != null) col.enabled = false;
        stateMachine.ChangeState(new DeathState(this));
    }
    #endregion
}