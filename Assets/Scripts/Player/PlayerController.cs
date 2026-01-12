using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Player 전체 제어 컨트롤러
/// </summary>
public class PlayerController : MonoBehaviour
{
    // ========================================
    // Components
    // ========================================

    [Header("Components")]
    [SerializeField] private PlayerData playerData;

    private Animator animator;
    private Rigidbody rb;


    [Header("Camera")]
    [Tooltip("카메라 Transform (이동 방향 계산용)")]
    [SerializeField] private Transform cameraTransform;

    [Header("Systems")]
    private PlayerStateMachine stateMachine;
    private ComboSystem comboSystem;
    private ComboSocket comboSocket;
    public ComboSocket ComboSocket => comboSocket;




    [SerializeField] private HpBarUi hPBar;

    [Header("플레이어 정보")]
    [SerializeField] private float currentHealth;

    [Header("Hitbox")]
    [SerializeField] private Collider hitboxCollider;
    private List<Collider> hitColliders = new List<Collider>();
    private WeaponHitbox weaponHitbox;


    // ========================================
    // States
    // ========================================

    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }
    public PlayerFinisherState FinisherState { get; private set; }
    public PlayerDodgeState DodgeState { get; private set; }
    public PlayerHitState HitState { get; private set; }
    public PlayerDeadState DeadState { get; private set; }

    // ========================================
    // Properties
    // ========================================

    public PlayerData PlayerData => playerData;
    public PlayerData Data => playerData;  // MoveState에서 사용
    public PlayerStateMachine StateMachine => stateMachine;
    public ComboSystem ComboSystem => comboSystem;
    public Animator Animator => animator;
    public Rigidbody Rigidbody => rb;
    public Transform CameraTransform => cameraTransform;
    public Transform Transform => transform;



    #region Start

    void Start()
    {
        currentHealth = playerData.maxHealth;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        rb.constraints =
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationY |
            RigidbodyConstraints.FreezeRotationZ;

        stateMachine = new PlayerStateMachine();

        // ========== ComboSocket 생성 ==========
        // PlayerData 전달 → 복원 OR 기본 소켓 생성
        comboSocket = new ComboSocket(playerData);

        // State 생성
        IdleState = new PlayerIdleState(this);
        MoveState = new PlayerMoveState(this);
        AttackState = new PlayerAttackState(this);
        FinisherState = new PlayerFinisherState(this);
        DodgeState = new PlayerDodgeState(this);
        HitState = new PlayerHitState(this);
        DeadState = new PlayerDeadState(this);

        stateMachine.ChangeState(IdleState);
    }

    #endregion

    void Update()
    {
        HandleInput();

        // 테스트용: T키로 공격 판정
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10);
        }

    }

    void FixedUpdate()
    {
        stateMachine.Update();
    }

    // ========================================
    // 입력 처리
    // ========================================

    void HandleInput()
    {
        // ========== UI 열려있으면 게임 입력 차단! ==========
        if (SocketManagerUI.IsUIOpen)
            return;

        // 회피 입력
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryDodge();
            return;
        }

        // 공격 입력
        InputTypes inputType = GetInputType();
        if (inputType != InputTypes.None)
        {
            TryAttack(inputType);
        }
    }

    /// <summary>
    /// 현재 프레임 입력을 InputType으로 변환
    /// </summary>
    private InputTypes GetInputType()
    {
        // ========== UI 열려있으면 None 반환 ==========
        if (SocketManagerUI.IsUIOpen)
            return InputTypes.None;

        if (Input.GetMouseButtonDown(0))
            return InputTypes.LeftClick;

        if (Input.GetMouseButtonDown(1))
            return InputTypes.RightClick;

        if (Input.GetKeyDown(KeyCode.Q))
            return InputTypes.QKey;

        if (Input.GetKeyDown(KeyCode.E))
            return InputTypes.EKey;

        if (Input.GetKeyDown(KeyCode.R))
            return InputTypes.RKey;

        return InputTypes.None;
    }




    #region Attack


    /// <summary>
    /// 최종 데미지 계산
    /// </summary>
    public float CalculateDamage(AttackSkillData skill)
    {
        float totalDamage = 0f;

        // 1. 스킬 기본 데미지
        totalDamage += skill.baseDamage;

        // 2. 무기 데미지 (나중에)
        // if (currentWeapon != null)
        //     totalDamage += currentWeapon.damage;

        // 3. 아이템 보너스 (나중에)
        // totalDamage += GetItemBonusDamage();

        // 4. 버프 (나중에)
        // totalDamage *= buffMultiplier;

        return totalDamage;
    }

    /// <summary>
    /// Player가 데미지 받기
    /// </summary>
    public void TakeDamage(float damage)
    {
        // 체력 감소
        currentHealth -= damage;

        Debug.Log($"Player 피격! 데미지: {damage}, 남은 체력: {currentHealth}");
        hPBar.SetHP(currentHealth, playerData.maxHealth);

        // 사망 체크
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
            return;
        }

        // HitState로 전환
        StateMachine.ChangeState(HitState);
    }

    /// <summary>
    /// 공격 시도
    /// </summary>
    void TryAttack(InputTypes inputType)
    {
        PlayerState currentState = stateMachine.CurrentState;

        // Idle 또는 Move에서 시작
        if (currentState == IdleState || currentState == MoveState)
        {
            bool success = comboSocket.StartCombo(inputType);

            if (success)
            {
                stateMachine.ChangeState(AttackState);
            }
            else
            {
                Debug.Log("콤보 시작 실패!");
            }
        }
        // Attack 중
        else if (currentState == AttackState)
        {
            bool success = comboSocket.ProcessNext(inputType);

            if (success)
            {
                AttackState.PlayNextStep();
            }
            else
            {
                AttackState.OnComboFailed();
            }
        }
    }

    /// <summary>
    /// 현재 공격의 데미지 반환
    /// 
    /// 호출:
    /// - Enemy.OnTriggerEnter()에서 호출
    /// 
    /// 반환:
    /// - 현재 콤보 스킬의 baseDamage
    /// - 스킬 없으면 기본 데미지 5
    /// </summary>
    public float GetCurrentAttackDamage()
    {
        AttackSkillData skill = ComboSocket.GetCurrentSkill();

        if (skill != null)
        {
            return skill.baseDamage;
        }

        return 5f;  // 기본 데미지
    }

    #endregion



    #region Dodge

    /// <summary>마지막 회피 시간</summary>
    private float lastDodgeTime = -999f;

    /// <summary>
    /// 회피 가능 여부 체크
    /// - 쿨타임 경과했는지 확인
    /// </summary>
    private bool CanDodge()
    {
        return Time.time >= lastDodgeTime + playerData.dodgeCooldown;
    }

    /// <summary>회피 시도</summary>
    void TryDodge()
    {
        if (stateMachine == null)
            return;

        PlayerState currentState = stateMachine.CurrentState;

        // 회피 가능한 State 체크
        if (currentState == IdleState ||
            currentState == MoveState ||
            currentState == AttackState)
        {
            // 쿨타임 체크
            if (!CanDodge())
            {
                float remaining = playerData.dodgeCooldown - (Time.time - lastDodgeTime);
                Debug.Log($"회피 쿨타임! {remaining:F1}초 남음");
                return;
            }

            // 회피 실행
            lastDodgeTime = Time.time;
            stateMachine.ChangeState(DodgeState);
        }
        else
        {
            Debug.Log("현재 상태에서는 회피할 수 없습니다.");
        }
    }

    #endregion



    #region Stun

    /// <summary>
    /// 스턴 게이지 회복
    /// PlayerHitState에서 호출
    /// </summary>
    public void RecoverStunGauge(float amount)
    {
        // TODO: 구현
        Debug.Log($"스턴 게이지 회복: {amount}");
    }

    /// <summary>
    /// Player가 스턴 데미지 받기
    /// </summary>
    public void TakeStunDamage(float stunDamage)
    {
        // TODO: 구현
        Debug.Log($"Player 스턴 데미지: {stunDamage}");
    }

    #endregion



    #region Die

    /// <summary>
    /// Player 사망 처리
    /// </summary>
    public void Die()
    {
        Debug.Log("Player 사망!");

        // DeadState로 전환
        StateMachine.ChangeState(DeadState);
    }

    #endregion


    #region Hitbox

    public void HitboxOn()
    {
        if (hitboxCollider == null)
        {
            Debug.LogError("[Player] hitboxCollider가 null!");
            return;
        }

        hitColliders.Clear();
        hitboxCollider.enabled = true;
        Debug.Log("[Player] Hitbox ON");
    }

    public void HitboxOff()
    {
        if (hitboxCollider == null)
            return;

        hitboxCollider.enabled = false;
        Debug.Log($"[Player] Hitbox OFF - {hitColliders.Count}개 타격");

        foreach (Collider col in hitColliders)
        {
            if (col == null)
                continue;

            if (col.gameObject.layer != 14)
                continue;

            Transform root = col.transform.root;
            EnemyController enemy = root.GetComponent<EnemyController>();

            if (enemy == null)
                continue;

            float damage = GetCurrentAttackDamage();
            Debug.Log($"[Player] {enemy.name} 타격! 데미지: {damage}");
            enemy.TakeDamage(damage);
        }

        hitColliders.Clear();
    }

    public void ForceDisableHitbox()
    {
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = false;
        }
        hitColliders.Clear();
    }

    public void AddHitCollider(Collider col)
    {
        if (!hitColliders.Contains(col))
        {
            hitColliders.Add(col);
            Debug.Log($"[Player] Collider 추가: {col.name}");
        }
    }

    #endregion


    #region Debug

    /// <summary>
    /// 디버그 정보 표시 (좌측 상단)
    /// - 현재 State
    /// - HP
    /// - 콤보 단계
    /// - 회피 쿨타임
    /// </summary>
    void OnGUI()
    {
        // 배경 스타일
        GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
        boxStyle.alignment = TextAnchor.UpperLeft;
        boxStyle.fontSize = 16;
        boxStyle.normal.textColor = Color.white;

        // 텍스트 스타일
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = 14;
        labelStyle.normal.textColor = Color.white;

        // 배경 박스
        GUI.Box(new Rect(10, 10, 250, 120), "Player Debug Info", boxStyle);

        int yPos = 35;
        int lineHeight = 20;

        // State
        string stateName = StateMachine?.CurrentState?.GetType().Name ?? "None";
        stateName = stateName.Replace("Player", "").Replace("State", "");
        GUI.Label(new Rect(20, yPos, 230, 20), $"State: {stateName}", labelStyle);
        yPos += lineHeight;

        // HP
        GUI.Label(new Rect(20, yPos, 230, 20),
            $"HP: {currentHealth:F0} / {playerData.maxHealth:F0}", labelStyle);
        yPos += lineHeight;

        // 콤보 단계
        int currentStep = ComboSocket?.GetCurrentStep() ?? 0;
        int socketCount = ComboSocket?.GetSocketCount() ?? 0;
        GUI.Label(new Rect(20, yPos, 230, 20),
            $"Combo: {currentStep} / {socketCount}", labelStyle);
        yPos += lineHeight;

        // 회피 쿨타임
        bool canDodge = CanDodge();
        float cooldownRemaining = canDodge ? 0f :
            playerData.dodgeCooldown - (Time.time - lastDodgeTime);

        string dodgeText = canDodge ? "Ready!" : $"{cooldownRemaining:F1}s";
        GUI.Label(new Rect(20, yPos, 230, 20),
            $"Dodge: {dodgeText}", labelStyle);
    }

    #endregion


}
