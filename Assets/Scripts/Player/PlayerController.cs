using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player 메인 컨트롤러
/// 입력 처리, State 관리, 리소스 관리
/// </summary>
public class PlayerController : MonoBehaviour
{
    // ========== 컴포넌트 ==========

    public Rigidbody Rigidbody { get; private set; }
    public Animator Animator { get; private set; }
    public Transform CameraTransform { get; private set; }

    // ========== 데이터 ==========

    [Header("Data")]
    [SerializeField] private PlayerData data;
    [SerializeField] private List<ComboData> comboDatas;

    public PlayerData Data => data;

    // ========== 시스템 ==========

    public PlayerStateMachine StateMachine { get; private set; }
    public ComboSystem ComboSystem { get; private set; }

    // ========== States ==========

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }
    public PlayerFinisherState FinisherState { get; private set; }
    public PlayerDodgeState DodgeState { get; private set; }
    public PlayerHitState HitState { get; private set; }
    public PlayerDeadState DeadState { get; private set; }

    // ========== 리소스 ==========

    [Header("Resources")]
    private float currentHP;
    private float currentStunGauge;

    public float CurrentHP => currentHP;
    public float MaxHP => data.maxHP;
    public float CurrentStunGauge => currentStunGauge;
    public float MaxStunGauge => data.maxStunGauge;

    // ========== 회피 ==========

    private float lastDodgeTime = -999f;

    public bool CanDodge => Time.time - lastDodgeTime >= data.dodgeCooldown;

    // ========== 초기화 ==========

    void Awake()
    {
        // 컴포넌트
        Rigidbody = GetComponent<Rigidbody>();
        Animator = GetComponent<Animator>();

        // 카메라 찾기
        CameraTransform = Camera.main.transform;

        // 시스템
        StateMachine = new PlayerStateMachine();
        ComboSystem = new ComboSystem(comboDatas);

        // State 생성
        IdleState = new PlayerIdleState(this);
        MoveState = new PlayerMoveState(this);
        AttackState = new PlayerAttackState(this);
        FinisherState = new PlayerFinisherState(this);
        DodgeState = new PlayerDodgeState(this);
        HitState = new PlayerHitState(this);
        DeadState = new PlayerDeadState(this);

        // 리소스 초기화
        currentHP = data.maxHP;
        currentStunGauge = data.maxStunGauge;
    }

    void Start()
    {
        // 시작 State
        StateMachine.ChangeState(IdleState);
    }

    // ========== Update ==========

    void Update()
    {
        // 입력 처리
        HandleInput();
    }

    void FixedUpdate()
    {
        // State 실행
        StateMachine.Update();
    }

    // ========== 입력 처리 ==========

    private void HandleInput()
    {
        // 죽었으면 입력 무시
        if (StateMachine.CurrentState == DeadState)
            return;

        // 스턴 중이면 입력 무시
        if (StateMachine.CurrentState == HitState)
            return;

        // 공격 입력
        if (Input.GetMouseButtonDown(0))
        {
            TryAttack(InputType.LeftClick);
        }

        if (Input.GetMouseButtonDown(1))
        {
            TryAttack(InputType.RightClick);
        }

        // 회피 입력
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryDodge();
        }
    }

    // ========== 공격 ==========

    private void TryAttack(InputType input)
    {
        // 이미 공격 중
        if (StateMachine.CurrentState == AttackState)
        {
            // 콤보 진행
            bool success = ComboSystem.ProcessInput(input);

            if (success)
            {
                // 성공! AttackState가 다음 타 재생
                AttackState.PlayNextStep();
            }
            else
            {
                // 실패! AttackState가 실패 처리
                AttackState.OnComboFailed();
            }
        }
        // 공격 시작 가능
        else if (StateMachine.CurrentState == IdleState ||
                 StateMachine.CurrentState == MoveState)
        {
            // 새 콤보 시작
            bool success = ComboSystem.StartCombo(input);

            if (success)
            {
                StateMachine.ChangeState(AttackState);
            }
        }
    }

    // ========== 회피 ==========

    private void TryDodge()
    {
        // 쿨타임 체크
        if (!CanDodge)
        {
            Debug.Log("회피 쿨타임 중!");
            return;
        }

        // 공격 전: 언제든 회피 가능
        if (StateMachine.CurrentState == IdleState ||
            StateMachine.CurrentState == MoveState)
        {
            StateMachine.ChangeState(DodgeState);
            lastDodgeTime = Time.time;
            return;
        }

        // 공격 중: Perfect 타이밍만 회피 가능
        if (StateMachine.CurrentState == AttackState)
        {
            if (ComboSystem.IsPerfectWindow())
            {
                // Perfect 타이밍! 회피 가능
                StateMachine.ChangeState(DodgeState);
                lastDodgeTime = Time.time;

                // 콤보 리셋
                ComboSystem.ResetCombo();

                Debug.Log("Perfect 회피!");
            }
            else
            {
                Debug.Log("Perfect 타이밍 아님! 회피 불가");
            }
        }
    }

    // ========== 리소스 관리 ==========

    public void TakeDamage(float damage)
    {
        if (StateMachine.CurrentState == DeadState)
            return;

        currentHP -= damage;
        currentHP = Mathf.Max(0, currentHP);

        Debug.Log($"HP: {currentHP}/{data.maxHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    public void TakeStunDamage(float damage)
    {
        if (StateMachine.CurrentState == DeadState)
            return;

        if (StateMachine.CurrentState == HitState)
            return;

        currentStunGauge -= damage;
        currentStunGauge = Mathf.Max(0, currentStunGauge);

        Debug.Log($"스턴 게이지: {currentStunGauge}/{data.maxStunGauge}");

        if (currentStunGauge <= 0)
        {
            // 스턴!
            StateMachine.ChangeState(HitState);
        }
    }

    public void Heal(float amount)
    {
        currentHP += amount;
        currentHP = Mathf.Min(currentHP, data.maxHP);

        Debug.Log($"HP 회복: {currentHP}/{data.maxHP}");
    }

    /// <summary>
    /// 스턴 게이지 완전 회복
    /// HitState 종료 시 호출
    /// </summary>
    public void RecoverStunGauge()
    {
        currentStunGauge = data.maxStunGauge;
        Debug.Log($"스턴 게이지 회복: {currentStunGauge}/{data.maxStunGauge}");
    }

    private void Die()
    {
        Debug.Log("Player 사망!");
        StateMachine.ChangeState(DeadState);
    }

    // ========== 애니메이션 이벤트 ==========

    /// <summary>
    /// Perfect 타이밍 구간 시작
    /// 애니메이션 이벤트에서 호출
    /// </summary>
    public void OnPerfectWindowStart()
    {
        ComboSystem.OnPerfectWindowStart();
    }

    /// <summary>
    /// Perfect 타이밍 구간 종료
    /// 애니메이션 이벤트에서 호출
    /// </summary>
    public void OnPerfectWindowEnd()
    {
        ComboSystem.OnPerfectWindowEnd();
    }

    /// <summary>
    /// 공격 타격 지점
    /// 애니메이션 이벤트에서 호출
    /// Enemy에게 데미지
    /// </summary>
    public void OnAttackHit()
    {
        // TODO: Enemy 감지 및 데미지
        float damage = ComboSystem.GetCurrentDamage();
        float stunDuration = ComboSystem.GetCurrentStunDuration();

        Debug.Log($"공격! 데미지: {damage}, 스턴: {stunDuration}초");

        // Enemy 찾기 및 데미지 (나중에 구현)
        // FindEnemiesInRange()
        // enemy.TakeDamage(damage)
        // enemy.TakeStun(stunDuration)
    }

    // ========== 디버그 ==========

    void OnGUI()
    {
        GUILayout.Label($"State: {StateMachine.CurrentState?.GetType().Name}");
        GUILayout.Label($"HP: {currentHP:F0}/{data.maxHP}");
        GUILayout.Label($"스턴 게이지: {currentStunGauge:F0}/{data.maxStunGauge}");
        GUILayout.Label($"콤보 단계: {ComboSystem.GetCurrentStep() + 1}/5");
        GUILayout.Label($"Perfect: {ComboSystem.GetPerfectCount()}");
        GUILayout.Label($"회피 쿨타임: {CanDodge}");
    }
}