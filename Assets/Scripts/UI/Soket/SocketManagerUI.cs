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
        
        RefreshSocketUI();
    }
}