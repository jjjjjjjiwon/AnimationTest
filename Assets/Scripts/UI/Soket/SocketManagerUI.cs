using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class SocketManagerUI : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private Transform socketContainer; // 왼쪽 무기 소켓 부모
    [SerializeField] private Transform skillContainer;  // 오른쪽 인벤토리 부모
    [SerializeField] private Button addSocketButton;    // 무기 소켓 추가 버튼

    [Header("Prefabs")]
    [SerializeField] private GameObject socketSlotPrefab;
    [SerializeField] private GameObject skillSlotPrefab;
    [SerializeField] private GameObject magicSlotPrefab;

    [Header("Book Mark (Tabs)")]
    [SerializeField] private Image meleeTabImage;
    [SerializeField] private Image magicTabImage;
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = Color.gray;

    [Header("Magic System 확장")]
    [SerializeField] private GameObject magicSlotContainer; // 마법용 고정 9개 슬롯 부모\

    // 💡 이제 이 리스트에 미리 배치된 9개의 MagicSlotUI를 넣어줍니다.
    [SerializeField] private List<MagicSlotUI> magicSlots = new List<MagicSlotUI>();


    private List<SocketSlotUI> socketSlotUIs = new List<SocketSlotUI>();
    private List<SkillSlotUI> skillSlotUIs = new List<SkillSlotUI>();

    private int selectedSocketIndex = -1; // 무기 소켓 인덱스
    private int selectedSlotIndex = -1;   // 공용 슬롯 인덱스 (무기 소켓 내 칸 혹은 마법 9칸 중 하나)
    private bool isMagicMode = false;     // 현재 마법 책갈피 상태인가?

    public static bool IsUIOpen { get; private set; } = false;



    private IEnumerator Start()
    {
        // 데이터 로딩 대기
        float timeout = 2.0f;
        while (PlayerSkillLoader.SkillDict.Count == 0 && timeout > 0)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        IsUIOpen = false;
        if (uiPanel != null) uiPanel.SetActive(false);

        // 초기화 시점에 미리 생성
        RefreshSkillInventoryUI();
        RefreshSocketUI();

        InitMagicSlots();

        // 시작 모드 설정 (무기 모드)
        SetMode(false);
    }

    void Update()
    {
        if (PlayerInfoUI.IsUIOpen) return;
        if (Input.GetKeyDown(KeyCode.F)) ToggleUI();
    }

    #region 기본 UI 제어 (열기/닫기)
    public void ToggleUI()
    {
        if (RuntimeManager.Instance == null || RuntimeManager.Instance.socketManager == null) return;

        PlayerController pc = Object.FindAnyObjectByType<PlayerController>();
        if (pc != null && !pc.CanOpenUI() && !IsUIOpen) return;

        if (!IsUIOpen)
        {
            IsUIOpen = true;
            uiPanel.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // 열 때 현재 모드에 맞춰 갱신
            SetMode(isMagicMode);

            if (pc != null) pc.StateMachine.ChangeState(pc.IdleState);
        }
        else
        {
            CloseUI();
        }
    }

    public void CloseUI()
    {
        IsUIOpen = false;
        uiPanel.SetActive(false);
        bool isLobby = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Lobby");
        if (!isLobby)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    #endregion

    #region Book Mark & Mode 전환
    public void OnMeleeTabClick() => SetMode(false);
    public void OnMagicTabClick() => SetMode(true);

    // 탭 전환 시 호출되는 핵심 함수
private void SetMode(bool magicMode)
{
    isMagicMode = magicMode;

    if (socketContainer != null) socketContainer.gameObject.SetActive(!isMagicMode);
    if (magicSlotContainer != null) magicSlotContainer.SetActive(isMagicMode);

    RefreshInventory();

    if (isMagicMode)
    {
        RefreshMagicSocketUI(); // 👈 여기서 새로 생성!
    }
    else
    {
        RefreshSocketUI();
    }
}

    public void UpdateTabVisuals()
    {
        if (meleeTabImage == null || magicTabImage == null) return;

        meleeTabImage.color = !isMagicMode ? activeColor : inactiveColor;
        magicTabImage.color = isMagicMode ? activeColor : inactiveColor;

        meleeTabImage.transform.localScale = !isMagicMode ? Vector3.one * 1.1f : Vector3.one;
        magicTabImage.transform.localScale = isMagicMode ? Vector3.one * 1.1f : Vector3.one;
    }

    // 탭을 바꿀 때나 마법을 장착했을 때 호출
    public void UpdateMagicSlotsVisuals()
    {
        foreach (var slot in magicSlots)
        {
            if (slot != null) slot.UpdateVisual();
        }
    }
    #endregion

    #region 인벤토리 및 소켓 갱신 로직
    private void RefreshInventory()
    {
        // 먼저 오른쪽 인벤토리 칸을 싹 비웁니다.
        ClearSkillSlots();

        if (isMagicMode)
        {
            // 마법 모드일 때는 마법 데이터만 생성
            RefreshMagicWordInventoryUI();
        }
        else
        {
            // 무기 모드일 때는 공격 스킬 데이터만 생성
            RefreshSkillInventoryUI();
        }
    }

    public void RefreshSocketUI()
    {
        foreach (var ui in socketSlotUIs) { if (ui != null) Destroy(ui.gameObject); }
        socketSlotUIs.Clear();

        if (RuntimeManager.Instance == null || RuntimeManager.Instance.socketManager == null) return;

        List<ComboSocket> sockets = RuntimeManager.Instance.socketManager.GetAllSockets();
        for (int i = 0; i < sockets.Count; i++)
        {
            GameObject slotObj = Instantiate(socketSlotPrefab, socketContainer);
            SocketSlotUI slotUI = slotObj.GetComponent<SocketSlotUI>();
            slotUI.Initialize(i, sockets[i], this);
            socketSlotUIs.Add(slotUI);
        }
    }

    public void RefreshSkillInventoryUI()
    {
        ClearSkillSlots();
        List<PlayerSkillData> allSkills = PlayerSkillLoader.GetAllSkills();
        foreach (var skill in allSkills)
        {
            if (skill == null) continue;
            GameObject slotObj = Instantiate(skillSlotPrefab, skillContainer);
            slotObj.GetComponent<SkillSlotUI>().Initialize(skill, this);
            skillSlotUIs.Add(slotObj.GetComponent<SkillSlotUI>());
        }
    }

    public void RefreshMagicWordInventoryUI()
    {
        ClearSkillSlots();
        List<MagicData> allMagicWords = GameData.Instance.GetMagicDataList();

        // 🔍 여기서 로그를 확인하세요!
        Debug.Log($"[UI] 인벤토리에 그릴 마법 개수: {(allMagicWords != null ? allMagicWords.Count : 0)}");

        if (allMagicWords == null || allMagicWords.Count == 0) return;

        foreach (var magic in allMagicWords)
        {
            GameObject slotObj = Instantiate(skillSlotPrefab, skillContainer);
            SkillSlotUI slotUI = slotObj.GetComponent<SkillSlotUI>();

            // 🔍 Initialize 내부에서 아이콘 로드가 되는지 확인
            slotUI.Initialize(magic, this);
            skillSlotUIs.Add(slotUI);
        }
    }

    // 인벤토리 슬롯들을 삭제하는 공용 함수
    private void ClearSkillSlots()
    {
        foreach (var ui in skillSlotUIs)
        {
            if (ui != null) Destroy(ui.gameObject);
        }
        skillSlotUIs.Clear();
    }

    #endregion

    #region 장착 로직 (복사 및 수정 완료)

    public void SelectSocketSlot(int socketIndex, int slotIndex)
    {
        selectedSocketIndex = socketIndex; // 무기 모드일 때 사용
        selectedSlotIndex = slotIndex;     // 무기/마법 공용 사용
        Debug.Log($"[선택] 모드: {(isMagicMode ? "마법" : "무기")}, 슬롯: {slotIndex}");
    }

    // 무기 스킬 장착
    public void EquipSkillToSelectedSocket(PlayerSkillData skill)
    {
        if (isMagicMode) return;
        if (selectedSocketIndex < 0 || selectedSlotIndex < 0) return;

        SocketManager socketManager = RuntimeManager.Instance.socketManager;
        ComboSocket socket = socketManager.GetSocket(selectedSocketIndex);
        if (socket != null)
        {
            socket.EquipSkill(selectedSlotIndex, skill);
            RefreshSocketUI();
            Debug.Log($"무기 스킬 '{skill.skill_Name}' 장착!");
        }
    }

// 마법 장착 (Overload)
    public void EquipSkillToSelectedSocket(MagicData magicSkill)
    {
        if (!isMagicMode) return;
        if (selectedSlotIndex < 0) return;

        // 1. [핵심 저장] 데이터의 원천인 RuntimeManager에만 저장합니다.
        if (RuntimeManager.Instance != null)
        {
            RuntimeManager.Instance.SetMagic(selectedSlotIndex, magicSkill);

            // 🔍 이전의 magicController.RegisterSkill 로직은 여기서 삭제되었습니다.
            // 플레이어는 발사할 때 RuntimeManager에서 직접 이 데이터를 꺼내 쓸 것이기 때문입니다.

            // 2. UI 비주얼만 즉시 갱신 (아이콘 등)
            UpdateMagicSlotsVisuals();
            Debug.Log($"[UI] {selectedSlotIndex}번 슬롯에 마법 '{magicSkill.magic_Name}' 장착 완료!");
        }
    }
    // Start 함수나 초기화 시점에 한 번 실행
    private void InitMagicSlots()
    {
        for (int i = 0; i < magicSlots.Count; i++)
        {
            if (magicSlots[i] != null)
            {
                // i는 인덱스, this는 매니저, 세 번째는 키 이름(생략 가능)
                magicSlots[i].Initialize(i, this, GetKeyName(i));
            }
        }
    }

    private string GetKeyName(int index)
    {
        string[] keys = { "Q", "W", "E", "R", "A", "S", "D", "F", "Space" };
        return (index >= 0 && index < keys.Length) ? keys[index] : "";
    }

    public void RefreshMagicSocketUI()
{
    // 1. 기존에 생성된 마법 슬롯들 삭제 (무기 소켓 방식과 동일)
    // 기존에 magicSlots에 담아둔 게 있다면 다 지워줍니다.
    foreach (Transform child in magicSlotContainer.transform) 
    {
        Destroy(child.gameObject);
    }
    magicSlots.Clear();

    // 2. 9개의 슬롯을 새로 생성 (Q~Space)
    string[] keyNames = { "Q", "E", "R", "F", "Space" };
    for (int i = 0; i < 9; i++)
    {
        // 프리펩 생성
        GameObject slotObj = Instantiate(magicSlotPrefab, magicSlotContainer.transform);
        MagicSlotUI slotUI = slotObj.GetComponent<MagicSlotUI>();
        
        // 초기화 (인덱스, 매니저, 키 이름)
        slotUI.Initialize(i, this, keyNames[i]);
        
        // 리스트에 보관
        magicSlots.Add(slotUI);
    }
}



    #endregion

    private KeyCode GetKeyFromIndex(int index)
    {
        KeyCode[] keys = { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.Space };
        return (index >= 0 && index < keys.Length) ? keys[index] : KeyCode.None;
    }
}