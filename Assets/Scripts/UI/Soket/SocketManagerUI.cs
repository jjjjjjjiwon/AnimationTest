using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SocketManagerUI : MonoBehaviour
{

    [Header("UI 요소")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private Transform socketContainer; // 왼쪽
    [SerializeField] private Transform skillContainer;  // 오른쪽

    [Header("Prefabs")]
    [SerializeField] private GameObject socketSlotPrefab;
    [SerializeField] private GameObject skillSlotPrefab;

    [Header("테스트 스킬들")]
    [SerializeField] private List<AttackSkillData> testSkills = new List<AttackSkillData>();

    private List<SocketSlotUI> socketSlotUIs = new List<SocketSlotUI>();
    private List<SkillSlotUI> skillSlotUIs = new List<SkillSlotUI>();
    private int selectedSocketIndex = -1;
    private int selectedSlotIndex = -1;

    public static bool IsUIOpen { get; private set; } = false;

    void Start()
    {
        uiPanel.SetActive(false);
        IsUIOpen = false;

        RefreshSkillInventoryUI();

        // ⭐ 씬 전환 시 자동으로 닫기
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        // ⭐ 다른 UI가 열려있으면 무시
        if (PlayerInfoUI.IsUIOpen)
            return;
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log("Socket Open ===============================");
            ToggleUI();
        }
    }

 public void ToggleUI()
{
    // ⭐ RuntimeManager 체크 추가!
    if (RuntimeManager.Instance == null || RuntimeManager.Instance.socketManager == null)
    {
        Debug.LogWarning("[SocketUI] RuntimeManager가 아직 초기화되지 않았습니다!");
        return;
    }
    
    // ⭐ 스테이지에서만 상태 체크
    PlayerController pc = FindObjectOfType<PlayerController>();
    if (pc != null && !pc.CanOpenUI())
    {
        Debug.Log("지금은 UI를 열 수 없습니다!");
        return;
    }

    if (!IsUIOpen)
    {
        IsUIOpen = true;
        uiPanel.SetActive(true);
        RefreshSocketUI();

        // ⭐ 스테이지에서만 Idle 전환
        if (pc != null)
        {
            Debug.Log("[UI] UI 열림, 상태를 IdleState로 전환 시도");
            pc.StateMachine.ChangeState(pc.IdleState);
        }
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

        // ⭐ 스테이지에서만 커서 숨기기
        bool isLobby = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Lobby");

        if (!isLobby)
        {
            // 스테이지: 커서 숨김
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            // 로비: 커서 유지
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void RefreshSocketUI()
    {
        // 기존 UI 제거
        foreach (var ui in socketSlotUIs)
        {
            if (ui != null)
                Destroy(ui.gameObject);
        }
        socketSlotUIs.Clear();

        // ⭐ RuntimeManager에서 SocketManager 가져오기
        if (RuntimeManager.Instance == null || RuntimeManager.Instance.socketManager == null)
        {
            Debug.LogError("[SocketUI] RuntimeManager 또는 SocketManager가 없습니다!");
            return;
        }

        SocketManager socketManager = RuntimeManager.Instance.socketManager;
        List<ComboSocket> sockets = socketManager.GetAllSockets();

        // UI 생성
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
        foreach (var ui in skillSlotUIs)
        {
            if (ui != null)
                Destroy(ui.gameObject);
        }
        skillSlotUIs.Clear();

        foreach (var skill in testSkills)
        {
            if (skill == null) continue;

            GameObject slotObj = Instantiate(skillSlotPrefab, skillContainer);
            SkillSlotUI slotUI = slotObj.GetComponent<SkillSlotUI>();

            slotUI.Initialize(skill, this);
            skillSlotUIs.Add(slotUI);
        }
    }

    public void SelectSocketSlot(int socketIndex, int slotIndex)
    {
        selectedSocketIndex = socketIndex;
        selectedSlotIndex = slotIndex;

        Debug.Log($"선택: 소켓 {socketIndex}, 슬롯 {slotIndex}");
    }

    public void EquipSkillToSelectedSocket(AttackSkillData skill)
    {
        if (selectedSocketIndex < 0 || selectedSlotIndex < 0)
        {
            Debug.Log("먼저 슬롯을 선택하세요!");
            return;
        }

        // ⭐ RuntimeManager에서 가져오기
        SocketManager socketManager = RuntimeManager.Instance.socketManager;
        ComboSocket socket = socketManager.GetSocket(selectedSocketIndex);

        if (socket != null)
        {
            socket.EquipSkill(selectedSlotIndex, skill);
            RefreshSocketUI();

            Debug.Log($"'{skill.skillName}' 장착!");
        }
    }

    public void OnAddSocketClick()
    {
         SocketManager socketManager = RuntimeManager.Instance.socketManager;
    
    if (socketManager.IsFull())
    {
        Debug.Log("소켓이 최대입니다! (5/5)");
        return;
    }
    
    ComboSocket newSocket = socketManager.AcquireNewSocket();
    
    if (newSocket != null)
    {
        Debug.Log("[UI] 새 소켓 추가됨!");
        
        // ========== 디버그 추가 ==========
        Debug.Log($"[Debug] RuntimeManager.socketManager 소켓 개수: {RuntimeManager.Instance.socketManager.GetAllSockets().Count}");
        // ================================
        
        RefreshSocketUI();
    }
    }

    void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ⭐ 새 메서드 추가
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // 씬 전환 시 UI 강제로 닫기
        if (IsUIOpen)
        {
            CloseUI();
            Debug.Log("[SocketUI] 씬 전환으로 UI 자동 닫김");
        }
    }

    


}