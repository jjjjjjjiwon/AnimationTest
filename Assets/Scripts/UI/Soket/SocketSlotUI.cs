using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 개별 소켓 UI (1개 소켓 = 5개 슬롯)
/// </summary>
public class SocketSlotUI : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI socketNameText;
    [SerializeField] private Transform slotContainer;  // ← 5개 슬롯을 담을 컨테이너
    [SerializeField] private GameObject slotItemPrefab;  // ← 개별 슬롯 UI Prefab
    
    [Header("색상")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;
    
    private int socketIndex;
    private ComboSocket socketData;  // ← 변경!
    private SocketManagerUI manager;
    private bool isSelected = false;
    
    private List<SlotItemUI> slotItemUIs = new List<SlotItemUI>();  // ← 5개 슬롯 UI
    
    public void Initialize(int index, ComboSocket data, SocketManagerUI managerUI)  // ← 변경!
    {
        socketIndex = index;
        socketData = data;
        manager = managerUI;
        
        UpdateUI();
    }
    
    public void UpdateUI()
    {
        if (socketData == null)
        {
            if (socketNameText != null)
                socketNameText.text = "Empty Socket";
            return;
        }
        
        // 소켓 이름 표시
        if (socketNameText != null)
            socketNameText.text = socketData.socketName;  // ← 변경!
        
        // 기존 슬롯 UI 제거
        foreach (var slotUI in slotItemUIs)
        {
            if (slotUI != null)
                Destroy(slotUI.gameObject);
        }
        slotItemUIs.Clear();
        
        // 5개 슬롯 UI 생성
        List<ComboSlot> slots = socketData.slots;  // ← 변경!
        for (int i = 0; i < slots.Count; i++)
        {
            GameObject slotObj = Instantiate(slotItemPrefab, slotContainer);
            SlotItemUI slotItemUI = slotObj.GetComponent<SlotItemUI>();
            
            if (slotItemUI != null)
            {
                slotItemUI.Initialize(socketIndex, i, slots[i], manager);
                slotItemUIs.Add(slotItemUI);
            }
        }
        
        // 선택 표시 업데이트
        UpdateSelection();
    }
    
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateSelection();
    }
    
    private void UpdateSelection()
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = isSelected ? selectedColor : normalColor;
        }
    }
}

/// <summary>
/// 개별 슬롯 아이템 UI (1개 입력키 + 1개 스킬)
/// </summary>
public class SlotItemUI : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI inputText;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private Button button;
    
    [Header("색상")]
    [SerializeField] private Color emptyColor = Color.gray;
    [SerializeField] private Color filledColor = Color.white;
    
    private int socketIndex;
    private int slotIndex;
    private ComboSlot slotData;  // ← 변경!
    private SocketManagerUI manager;
    
    public void Initialize(int sockIdx, int slIdx, ComboSlot data, SocketManagerUI managerUI)  // ← 변경!
    {
        socketIndex = sockIdx;
        slotIndex = slIdx;
        slotData = data;
        manager = managerUI;
        
        if (button != null)
            button.onClick.AddListener(OnClick);
        
        UpdateUI();
    }
    
    public void UpdateUI()
    {
        if (slotData == null)
        {
            if (inputText != null)
                inputText.text = "none";
            if (skillNameText != null)
                skillNameText.text = "Empty";
            if (iconImage != null)
            {
                iconImage.color = emptyColor;
                iconImage.sprite = null;
            }
            return;
        }
        
        // 입력키 표시
        if (inputText != null)
            inputText.text = GetInputKeyString(slotData.assignedInput);  // ← 변경!
        
        // 스킬 표시
        if (slotData.equippedSkill != null)  // ← 변경!
        {
            if (skillNameText != null)
                skillNameText.text = slotData.equippedSkill.skillName;
            
            if (iconImage != null)
            {
                iconImage.sprite = slotData.equippedSkill.skillIcon;
                iconImage.color = filledColor;
            }
        }
        else
        {
            if (skillNameText != null)
                skillNameText.text = "Empty";
            
            if (iconImage != null)
            {
                iconImage.color = emptyColor;
                iconImage.sprite = null;
            }
        }
    }
    
    private void OnClick()
    {
        if (slotData == null)
        {
            Debug.Log("빈 슬롯!");
            return;
        }
        
        // 이 슬롯 선택
        manager.SelectSocketSlot(socketIndex, slotIndex);
    }
    
    private string GetInputKeyString(InputTypes input)
    {
        switch (input)
        {
            case InputTypes.LeftClick: return "Left";
            case InputTypes.RightClick: return "Right";
            case InputTypes.QKey: return "Q";
            case InputTypes.EKey: return "E";
            case InputTypes.RKey: return "R";
            default: return "없음";
        }
    }
}