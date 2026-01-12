using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SocketManagerUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    
    [Header("UI 요소")]
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private Transform socketContainer;
    [SerializeField] private Transform skillInventoryContainer;
    [SerializeField] private Button addSocketButton;
    [SerializeField] private Button closeButton;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject socketSlotPrefab;
    [SerializeField] private GameObject skillSlotPrefab;
    
    [Header("테스트 스킬들")]
    [SerializeField] private List<AttackSkillData> testSkills = new List<AttackSkillData>();
    
    private List<SocketSlotUI> socketSlotUIs = new List<SocketSlotUI>();
    private List<SkillSlotUI> skillSlotUIs = new List<SkillSlotUI>();
    private int selectedSocketIndex = -1;
    private int selectedSlotIndex = -1;  // ← 추가! (어느 슬롯에 스킬 장착할지)
    
    public static bool IsUIOpen { get; private set; } = false;
    
    void Start()
    {
        addSocketButton.onClick.AddListener(OnAddSocketClick);
        
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseUI);
        
        uiPanel.SetActive(false);
        IsUIOpen = false;
        
        RefreshSkillInventoryUI();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleUI();
        }
    }
    
    public void ToggleUI()
    {
        // ========== UI 열기 ==========
        if (!IsUIOpen)
        {
            // Idle 또는 Move 상태에서만 열기
            if (!CanOpenUI())
            {
                Debug.Log("전투 중에는 소켓 창을 열 수 없습니다!");
                return;
            }
            
            IsUIOpen = true;
            uiPanel.SetActive(true);
            
            RefreshSocketUI();
            Debug.Log("소켓 UI 열림");
            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            
            // Idle로 전환
            playerController.StateMachine.ChangeState(playerController.IdleState);
        }
        // ========== UI 닫기 ==========
        else
        {
            CloseUI();
        }
    }
    
    /// <summary>
    /// UI를 열 수 있는 상태인지 체크
    /// - Idle 또는 Move 상태에서만 가능
    /// </summary>
    private bool CanOpenUI()
    {
        var currentState = playerController.StateMachine.CurrentState;
        
        return currentState is PlayerIdleState or PlayerMoveState;
    }
    
    public void CloseUI()
    {
        IsUIOpen = false;
        uiPanel.SetActive(false);
        
        Debug.Log("소켓 UI 닫힘");
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        // Idle로 전환
        playerController.StateMachine.ChangeState(playerController.IdleState);
    }
    
    /// <summary>
    /// 소켓 UI 새로고침
    /// </summary>
    public void RefreshSocketUI()
    {
        // 기존 UI 제거
        foreach (var ui in socketSlotUIs)
        {
            if (ui != null)
                Destroy(ui.gameObject);
        }
        socketSlotUIs.Clear();
        
        // SocketManager에서 모든 소켓 가져오기
        SocketManager socketManager = playerController.SocketManager;  // ← 변경!
        List<ComboSocket> sockets = socketManager.GetAllSockets();  // ← 변경!
        
        // 각 소켓에 대해 UI 생성
        for (int i = 0; i < sockets.Count; i++)
        {
            GameObject slotObj = Instantiate(socketSlotPrefab, socketContainer);
            SocketSlotUI slotUI = slotObj.GetComponent<SocketSlotUI>();
            
            slotUI.Initialize(i, sockets[i], this);  // ← ComboSocket 전달
            socketSlotUIs.Add(slotUI);
        }
    }
    
    /// <summary>
    /// 소켓 선택 (어느 소켓의 어느 슬롯에 스킬을 장착할지)
    /// </summary>
    public void SelectSocketSlot(int socketIndex, int slotIndex)  // ← 변경!
    {
        selectedSocketIndex = socketIndex;
        selectedSlotIndex = slotIndex;  // ← 추가!
        
        // 모든 소켓 UI 선택 해제
        foreach (var ui in socketSlotUIs)
        {
            ui.SetSelected(false);
        }
        
        // 선택된 소켓만 하이라이트
        if (socketIndex >= 0 && socketIndex < socketSlotUIs.Count)
        {
            socketSlotUIs[socketIndex].SetSelected(true);
        }
        
        Debug.Log($"[UI] 소켓 #{socketIndex}, 슬롯 #{slotIndex} 선택됨");
    }
    
    /// <summary>
    /// 스킬 인벤토리 UI 새로고침
    /// </summary>
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
            
            GameObject slotObj = Instantiate(skillSlotPrefab, skillInventoryContainer);
            SkillSlotUI slotUI = slotObj.GetComponent<SkillSlotUI>();
            
            slotUI.Initialize(skill, this);
            skillSlotUIs.Add(slotUI);
        }
    }
    
    /// <summary>
    /// 선택된 슬롯에 스킬 장착
    /// </summary>
    public void EquipSkillToSelectedSocket(AttackSkillData skill)
    {
        if (selectedSocketIndex < 0 || selectedSlotIndex < 0)  // ← 변경!
        {
            Debug.Log("소켓의 슬롯을 먼저 선택하세요!");
            return;
        }
        
        // SocketManager에서 해당 소켓 가져오기
        SocketManager socketManager = playerController.SocketManager;  // ← 변경!
        ComboSocket socket = socketManager.GetSocket(selectedSocketIndex);  // ← 변경!
        
        if (socket == null)
        {
            Debug.LogError("소켓을 찾을 수 없습니다!");
            return;
        }
        
        // 스킬 장착
        socket.EquipSkill(selectedSlotIndex, skill);  // ← 변경!
        
        Debug.Log($"[UI] 소켓 #{selectedSocketIndex}, 슬롯 #{selectedSlotIndex}에 '{skill.skillName}' 장착!");
        
        // UI 새로고침
        RefreshSocketUI();
    }
    
    /// <summary>
    /// 소켓 추가 버튼
    /// </summary>
    public void OnAddSocketClick()
    {
        SocketManager socketManager = playerController.SocketManager;  // ← 변경!
        
        if (socketManager.IsFull())  // ← 변경!
        {
            Debug.Log("소켓이 최대입니다! (5/5)");
            return;
        }
        
        ComboSocket newSocket = socketManager.AcquireNewSocket();  // ← 변경!
        
        if (newSocket != null)
        {
            Debug.Log("[UI] 새 소켓 추가됨!");
            RefreshSocketUI();
        }
    }
}