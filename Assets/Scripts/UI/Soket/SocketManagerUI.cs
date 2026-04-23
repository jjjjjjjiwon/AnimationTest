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

        // "UI를 여는 키가 눌렸는가?" 지도에 물어봄
        if (InputManager.GetUIOpenInput())
        {
            ToggleUI();
        }

        // UI가 열려있을 때 닫기 키 체크
        if (IsUIOpen && InputManager.GetExitInput())
        {
            CloseUI();
        }
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
        ClearSkillSlots(); // 기존 UI 삭제

        // GameData에서 리스트를 가져옴
        List<MagicData> allMagicWords = GameData.Instance.GetMagicDataList();

        // 🔍 5개가 들어오는지 콘솔창에서 꼭 확인하세요!
        Debug.Log($"[MagicUI] 불러온 마법 데이터 총 개수: {(allMagicWords != null ? allMagicWords.Count : 0)}");

        if (allMagicWords == null || allMagicWords.Count == 0) return;

        foreach (var magic in allMagicWords)
        {
            if (magic == null) continue;

            GameObject slotObj = Instantiate(skillSlotPrefab, skillContainer);
            SkillSlotUI slotUI = slotObj.GetComponent<SkillSlotUI>();

            // 여기서 실제로 5번 도는지 확인
            Debug.Log($"[MagicUI] 슬롯 생성 중: {magic.magicName}");

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
            Debug.Log($"[UI] {selectedSlotIndex}번 슬롯에 마법 '{magicSkill.magicName}' 장착 완료!");
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
        foreach (Transform child in magicSlotContainer.transform)
        {
            Destroy(child.gameObject);
        }
        magicSlots.Clear();

        // 9개 슬롯에 대응하는 키 이름 (9개를 맞춰주세요)
        string[] keyNames = { "Q", "E", "R", "T", "Shift", "Ctrl", "Z", "X", "C" };

        for (int i = 0; i < 9; i++)
        {
            GameObject slotObj = Instantiate(magicSlotPrefab, magicSlotContainer.transform);
            MagicSlotUI slotUI = slotObj.GetComponent<MagicSlotUI>();

            // keyNames 배열 범위를 벗어나지 않도록 방어 코드 추가
            string kName = (i < keyNames.Length) ? keyNames[i] : "";
            slotUI.Initialize(i, this, kName);

            magicSlots.Add(slotUI);
        }
    }
    public void OnAddSocketClick()
    {
        // 1. 데이터 소스(RuntimeManager -> SocketManager) 확인
        if (RuntimeManager.Instance == null || RuntimeManager.Instance.socketManager == null) return;

        // 2. 소켓 추가 시도 및 결과 확인
        bool isAdded = RuntimeManager.Instance.socketManager.AddSocket();

        if (isAdded)
        {
            // 3. 성공 시 UI 리스트 갱신
            RefreshSocketUI();
            Debug.Log("[SocketUI] 새로운 콤보 소켓이 해금되었습니다.");
        }
        else
        {
            // 최대 개수(5개) 초과 시 알림
            Debug.LogWarning("[SocketUI] 소켓을 더 이상 추가할 수 없습니다 (최대 5개).");
        }
    }



    #endregion

    private KeyCode GetKeyFromIndex(int index)
    {
        KeyCode[] keys = { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.R, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.F, KeyCode.Space };
        return (index >= 0 && index < keys.Length) ? keys[index] : KeyCode.None;
    }
}