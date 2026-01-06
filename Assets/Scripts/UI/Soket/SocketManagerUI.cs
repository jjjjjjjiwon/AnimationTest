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
    
    // ========== UI 상태 (Static으로 전역 접근) ==========
    public static bool IsUIOpen { get; private set; } = false;
    
    void Start()
    {
        addSocketButton.onClick.AddListener(OnAddSocketClick);
        
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseUI);
        
        uiPanel.SetActive(false);
        IsUIOpen = false;  // ← 초기화
        
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
        IsUIOpen = !IsUIOpen;  // ← 상태 업데이트
        uiPanel.SetActive(IsUIOpen);
        
        if (IsUIOpen)
        {
            RefreshSocketUI();
            Debug.Log("소켓 UI 열림 - 게임 입력 차단");
            
            // ========== 커서 표시 ==========
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Debug.Log("소켓 UI 닫힘 - 게임 입력 재개");
            
            // ========== 커서 숨김 ==========
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    
    public void CloseUI()
    {
        IsUIOpen = false;  // ← 상태 업데이트
        uiPanel.SetActive(false);
        Debug.Log("소켓 UI 닫힘 - 게임 입력 재개");
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    // ... 나머지 함수들 동일 ...
    
    public void RefreshSocketUI()
    {
        foreach (var ui in socketSlotUIs)
        {
            if (ui != null)
                Destroy(ui.gameObject);
        }
        socketSlotUIs.Clear();
        
        ComboSocket comboSocket = playerController.ComboSocket;
        List<ComboSocketSlot> sockets = comboSocket.GetAllSockets();
        
        for (int i = 0; i < sockets.Count; i++)
        {
            GameObject slotObj = Instantiate(socketSlotPrefab, socketContainer);
            SocketSlotUI slotUI = slotObj.GetComponent<SocketSlotUI>();
            
            slotUI.Initialize(i, sockets[i], this);
            socketSlotUIs.Add(slotUI);
        }
        
        Debug.Log($"소켓 UI 갱신: {sockets.Count}개");
    }
    
    public void SelectSocket(int index)
    {
        selectedSocketIndex = index;
        
        foreach (var ui in socketSlotUIs)
        {
            ui.SetSelected(false);
        }
        
        if (index >= 0 && index < socketSlotUIs.Count)
        {
            socketSlotUIs[index].SetSelected(true);
        }
        
        Debug.Log($"소켓 {index + 1} 선택됨!");
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
            
            GameObject slotObj = Instantiate(skillSlotPrefab, skillInventoryContainer);
            SkillSlotUI slotUI = slotObj.GetComponent<SkillSlotUI>();
            
            slotUI.Initialize(skill, this);
            skillSlotUIs.Add(slotUI);
        }
        
        Debug.Log($"스킬 인벤토리 갱신: {skillSlotUIs.Count}개");
    }
    
    public void EquipSkillToSelectedSocket(AttackSkillData skill)
    {
        if (selectedSocketIndex < 0)
        {
            Debug.Log("소켓을 먼저 선택하세요!");
            return;
        }
        
        ComboSocket comboSocket = playerController.ComboSocket;
        comboSocket.EquipSkill(selectedSocketIndex, skill);
        
        Debug.Log($"소켓 {selectedSocketIndex + 1}에 [{skill.skillName}] 장착!");
        
        RefreshSocketUI();
    }
    
    public void OnAddSocketClick()
    {
        ComboSocket comboSocket = playerController.ComboSocket;
        
        if (comboSocket.IsFull())
        {
            Debug.Log("소켓이 최대입니다! (5/5)");
            return;
        }
        
        ComboSocketSlot newSocket = comboSocket.AcquireNewSocket();
        
        Debug.Log($"새 소켓 추가! 입력키: {newSocket.assignedInput}");
        
        RefreshSocketUI();
    }
}