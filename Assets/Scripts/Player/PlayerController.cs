using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    // ========================================
    // Components & Data
    // ========================================
    [Header("Components")]
    [SerializeField] private PlayerData playerData;
    private Animator animator;
    private Rigidbody rb;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    [Header("Systems")]
    private PlayerStateMachine stateMachine;
    private SocketManager socketManager;
    public SocketManager SocketManager => socketManager;

    [SerializeField] private HpBarUi hPBar;

    [Header("Attack Settings")]
    [SerializeField] private WeaponHitbox weapon; // 인스펙터에서 무기 오브젝트 할당

    [Header("Hitbox")]
    [SerializeField] private Collider hitboxCollider;

    [Header("UI Reference")]
    [SerializeField] private ComboGaugeUI comboUI;
    public ComboGaugeUI ComboUI => comboUI;

    // ========================================
    // States & Properties
    // ========================================
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }
    public PlayerFinisherState FinisherState { get; private set; }
    public PlayerDodgeState DodgeState { get; private set; }
    public PlayerHitState HitState { get; private set; }
    public PlayerDeadState DeadState { get; private set; }

    public PlayerData Data => playerData;
    public PlayerStateMachine StateMachine => stateMachine;
    public Animator Animator => animator;
    public Rigidbody Rigidbody => rb;
    public Transform CameraTransform => cameraTransform;
    public Transform Transform => transform;

    public Vector3 MoveInput => new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

    public bool IsPerfectTiming { get; set; } // 퍼펙트 판정 여부를 저장하는 변수


    /// <summary>
    /// Magic
    /// </summary>

    public enum PlayerMode { Melee, Magic }
    public PlayerMode currentMode = PlayerMode.Melee;

    [Header("UI Reference")]
    public MagicUIManager uiManager; // UI를 제어할 스크립트 참조


    [Header("모드 설정")]
    public bool isMagicMode = false; // 전역적으로 참조할 모드 변수

    // 만약 다른 스크립트에서 참조하기 쉽게 하려면 프로퍼티 사용
    public bool IsMagicMode => isMagicMode;

    [Header("Magic Settings")]
    [SerializeField] private Transform firePoint; // 마법이 나갈 총구 위치

    #region 초기화 (Initialization)

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (RuntimeManager.Instance == null || RuntimeManager.Instance.socketManager == null)
        {
            StartCoroutine(WaitForRuntimeManager());
            return;
        }
        InitializePlayer();
    }

    private IEnumerator WaitForRuntimeManager()
    {
        while (RuntimeManager.Instance == null || RuntimeManager.Instance.socketManager == null)
            yield return null;
        InitializePlayer();
    }

    private void InitializePlayer()
    {
        stateMachine = new PlayerStateMachine();
        socketManager = RuntimeManager.Instance.socketManager;

        IdleState = new PlayerIdleState(this);
        MoveState = new PlayerMoveState(this);
        AttackState = new PlayerAttackState(this);
        FinisherState = new PlayerFinisherState(this);
        DodgeState = new PlayerDodgeState(this);
        HitState = new PlayerHitState(this);
        DeadState = new PlayerDeadState(this);

        stateMachine.ChangeState(IdleState);

        // [복구] 콤보 UI 초기화
        if (comboUI != null)
            comboUI.Init(Animator, SocketManager);
    }

    #endregion

    #region 업데이트 및 상태 제어 (Update & Flow)

    void Update()
    {
        // UI가 열려있을 때는 로직 중단
        if (SocketManagerUI.IsUIOpen) return;

        // 모드 전환 체크 (탭 키)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMode();
        }

        // 모드에 따른 입력 처리 분리
        if (currentMode == PlayerMode.Magic)
        {
            HandleMagicInput(); // 마법 모드 입력
        }
        else
        {
            HandleInput();      // 근접 모드 입력 (기존 로직)
        }

        
    }

    void FixedUpdate() => stateMachine?.Update();

    public bool CanOpenUI()
    {
        if (stateMachine == null) return false;
        PlayerState currentState = stateMachine.CurrentState;
        return currentState == IdleState || currentState == MoveState;
    }

    #endregion

    #region 입력 관리 (Input Handling)

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space)) { TryDodge(); return; }

        InputTypes inputType = GetInputType();
        if (inputType != InputTypes.None) TryAttack(inputType);
    }

    private InputTypes GetInputType()
    {
        if (Input.GetMouseButtonDown(0)) return InputTypes.LeftClick;
        if (Input.GetMouseButtonDown(1)) return InputTypes.RightClick;
        if (Input.GetKeyDown(KeyCode.Q)) return InputTypes.QKey;
        if (Input.GetKeyDown(KeyCode.E)) return InputTypes.EKey;
        if (Input.GetKeyDown(KeyCode.R)) return InputTypes.RKey;
        return InputTypes.None;
    }

    #endregion

    #region 전투 시스템 (Combat - Attack)

    private void TryAttack(InputTypes inputType)
    {
        if (stateMachine.CurrentState == AttackState)
        {
            // [에러 방지] 현재 상태가 AttackState일 때만 다음 소켓 확인
            if (AttackState.CanInputCombo())
            {
                // 소켓 매니저에서 다음 기술이 있는지 확인하고 있으면 예약
                if (socketManager.ProcessNext(inputType))
                {
                    AttackState.RegisterInput();
                }
            }
        }
        else if (stateMachine.CurrentState == IdleState || stateMachine.CurrentState == MoveState)
        {
            if (socketManager.StartCombo(inputType))
            {
                stateMachine.ChangeState(AttackState);
            }
        }
    }

    public void HitboxOn()
    {
        // 1. 현재 콤보 단계의 스킬 데이터를 가져옴
        var currentSkill = RuntimeManager.Instance.socketManager.GetCurrentSkill();

        if (weapon != null)
        {
            // 2. 무기에 스킬 데이터 주입 및 콜라이더 활성화
            weapon.EnableHitbox(currentSkill);
            // Debug.Log($"[Event] Hitbox On: {currentSkill?.skill_Name}");
        }
    }

    // 애니메이션 이벤트: HitboxOff (공격 판정 종료)
    public void HitboxOff()
    {
        if (weapon != null)
        {
            weapon.DisableHitbox();
            // Debug.Log("[Event] Hitbox Off");
        }
    }

    /// <summary>
    /// 강제로 히트박스 끄기 (피격 시 호출)
    /// </summary>
    public void ForceDisableHitbox()
    {
        if (hitboxCollider != null)
        {
            hitboxCollider.enabled = false;
        }
    }

    #endregion


    #region 피격 및 사망 (Health & Damage)

    public void TakeDamage(float damage)
    {
        PlayerStats stats = RuntimeManager.Instance.playerStats;
        stats.current_Health -= damage;
        hPBar?.SetHP(stats.current_Health, stats.max_Health);
        if (stats.current_Health <= 0) Die();
        else StateMachine.ChangeState(HitState);
    }

    public void Die() => StateMachine.ChangeState(DeadState);

    public void TakeStunDamage(float stunDamage)
    {
        // TODO: 스탯 시스템과 연동
        Debug.Log($"Player 스턴 데미지: {stunDamage}");
    }

    public void RecoverStunGauge(float amount)
    {
        // PlayerHitState 등에서 호출됨
        Debug.Log($"스턴 게이지 회복 시도: {amount}");
    }

    #endregion



    #region 특수 액션 (Special Actions)

    private float lastDodgeTime = -999f;
    private bool CanDodge() => Time.time >= lastDodgeTime + playerData.dodgeCooldown;

    private void TryDodge()
    {
        if (CanDodge() && (stateMachine.CurrentState == IdleState || stateMachine.CurrentState == MoveState || stateMachine.CurrentState == AttackState))
        {
            lastDodgeTime = Time.time;
            stateMachine.ChangeState(DodgeState);
        }
    }

    #endregion

    #region 기타 데이터 (Utility)

    public void AddStatPoints(int amount)
    {
        RuntimeManager.Instance.playerStats.AddPoints(amount);
        Debug.Log($"[Stat] 포인트 추가: {amount}");
    }

    #endregion



    #region 마법

    void ToggleMode()
    {
        currentMode = (currentMode == PlayerMode.Melee) ? PlayerMode.Magic : PlayerMode.Melee;
        isMagicMode = (currentMode == PlayerMode.Magic); // 전역 변수 동기화

        if (uiManager != null)
            uiManager.UpdateModeUI(currentMode);

        Debug.Log($"현재 모드: {currentMode}");
    }

    private void HandleMagicInput()
    {
        // Q,E, R 입력을 받아서 해당 슬롯 번호를 실행 함수로 넘깁니다.
        if (Input.GetKeyDown(KeyCode.Q)) ExecuteSlot(0);
        if (Input.GetKeyDown(KeyCode.E)) ExecuteSlot(1);
        if (Input.GetKeyDown(KeyCode.R)) ExecuteSlot(2);
        if (Input.GetKeyDown(KeyCode.T)) ExecuteSlot(3);
        if (Input.GetKeyDown(KeyCode.LeftShift)) ExecuteSlot(4);
        if (Input.GetKeyDown(KeyCode.LeftControl)) ExecuteSlot(5);
    }

    private void ExecuteSlot(int slotIndex)
    {
        // 1. RuntimeManager가 있는지 확인
        if (RuntimeManager.Instance == null)
        {
            Debug.LogError("RuntimeManager가 씬에 없습니다!");
            return;
        }

        // 2. 실시간으로 딕셔너리에서 데이터를 직접 확인
        // UI에서 세팅한 데이터가 여기로 들어오는지 체크하는 로그를 넣습니다.
        if (RuntimeManager.Instance.EquipedMagics.TryGetValue(slotIndex, out MagicData data))
        {
            if (data != null && !string.IsNullOrEmpty(data.magic_Name))
            {
                Debug.Log($"[Magic] {slotIndex}번 슬롯 실행: {data.magic_Name}");

                // 3. [핵심] 라이브러리에 이름 전달
                MagicLibrary.Execute(data.magic_Name, transform);
            }
            else
            {
                Debug.LogWarning($"{slotIndex}번 슬롯에 데이터는 있으나 이름이 비어있습니다.");
            }
        }
        else
        {
            // 4. 이 로그가 찍힌다면 UI에서 저장한 곳과 여기가 서로 다른 곳을 보고 있는 것입니다.
            Debug.Log($"{slotIndex}번 슬롯에 장착된 마법 데이터가 없습니다. (현재 딕셔너리 개수: {RuntimeManager.Instance.EquipedMagics.Count})");
        }
    }

    #endregion

}