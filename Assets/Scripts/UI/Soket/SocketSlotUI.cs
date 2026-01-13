using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 소켓 UI - 1줄에 5개 슬롯
/// </summary>
public class SocketSlotUI : MonoBehaviour
{
    [Header("5개 슬롯 이미지")]
    [SerializeField] private List<Image> slotImages = new List<Image>();
    [SerializeField] private List<Button> slotButtons = new List<Button>();
    [SerializeField] private List<TextMeshProUGUI> keyTexts = new List<TextMeshProUGUI>(); // ← 추가!
    
    [Header("색상")]
    [SerializeField] private Color emptyColor = Color.gray;
    [SerializeField] private Color filledColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(1f, 1f, 0.5f, 1f);
    
    private int socketIndex;
    private ComboSocket socketData;
    private SocketManagerUI manager;
    private int selectedSlotIndex = -1;
    
    public void Initialize(int index, ComboSocket data, SocketManagerUI managerUI)
    {
        socketIndex = index;
        socketData = data;
        manager = managerUI;
        
        // 버튼 이벤트 연결
        for (int i = 0; i < slotButtons.Count; i++)
        {
            int slotIdx = i;
            if (slotButtons[i] != null)
            {
                slotButtons[i].onClick.AddListener(() => OnSlotClick(slotIdx));
            }
        }
        
        UpdateUI();
    }
    
    public void UpdateUI()
    {
        if (socketData == null || socketData.slots == null)
            return;
        
        // 5개 슬롯 업데이트
        for (int i = 0; i < 5 && i < socketData.slots.Count && i < slotImages.Count; i++)
        {
            ComboSlot slot = socketData.slots[i];
            
            // ========== 입력키 텍스트 표시 ========== 
            if (i < keyTexts.Count && keyTexts[i] != null)
            {
                keyTexts[i].text = GetInputKeyString(slot.assignedInput);
            }
            
            // 스킬 아이콘 표시
            if (slot.equippedSkill != null && slot.equippedSkill.skillIcon != null)
            {
                // 스킬 있음 - 아이콘 표시
                slotImages[i].sprite = slot.equippedSkill.skillIcon;
                slotImages[i].color = filledColor;
            }
            else
            {
                // 스킬 없음 - 회색
                slotImages[i].sprite = null;
                slotImages[i].color = emptyColor;
            }
        }
    }
    
    public void SetSelected(bool selected)
    {
        // 선택 표시
    }
    
    private void OnSlotClick(int slotIndex)
    {
        selectedSlotIndex = slotIndex;
        manager.SelectSocketSlot(socketIndex, slotIndex);
        Debug.Log($"소켓 {socketIndex}, 슬롯 {slotIndex} 클릭!");
    }
    
    // ========== 입력키 → 텍스트 변환 ========== 
    private string GetInputKeyString(InputTypes input)
    {
        switch (input)
        {
            case InputTypes.LeftClick: return "Left";
            case InputTypes.RightClick: return "Rifht";
            case InputTypes.QKey: return "Q";
            case InputTypes.EKey: return "E";
            case InputTypes.RKey: return "R";
            default: return "?";
        }
    }
}