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
    [SerializeField] private Image iconImage;   // 슬롯의 아이콘 이미지 컴포넌트
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

    for (int i = 0; i < slotButtons.Count; i++)
    {
        int slotIdx = i;
        if (slotButtons[i] != null)
        {
            // ⭐ 중요: 기존에 달린 이벤트를 지우지 않으면 클릭 한 번에 여러 번 실행됨
            slotButtons[i].onClick.RemoveAllListeners(); 
            slotButtons[i].onClick.AddListener(() => OnSlotClick(slotIdx));
        }
    }

    UpdateUI();
}

    public void UpdateUI()
{
    if (socketData == null) return;

    // 소켓 안에 있는 5개의 슬롯 정보를 가져옵니다.
    var slots = socketData.GetSlots(); 

    for (int i = 0; i < slotImages.Count; i++)
    {
        if (i < slots.Count)
        {
            // 1. 입력키 텍스트 업데이트 (Q, E, R, Left 등)
            if (i < keyTexts.Count && keyTexts[i] != null)
            {
                keyTexts[i].text = GetInputKeyString(slots[i].assignedInput);
            }

            // 2. 스킬 아이콘 로드 및 색상 설정
            string skillID = slots[i].equippedSkillID;
            
            if (!string.IsNullOrEmpty(skillID))
            {
                // ID를 이용해 데이터 매니저 등에서 SkillData를 가져오는 로직이 필요할 수 있습니다.
                // 여기서는 소켓 데이터가 이미 PlayerSkillData를 알고 있다고 가정하거나 
                // RuntimeManager를 통해 데이터를 가져와야 합니다.
                PlayerSkillData skillData = RuntimeManager.Instance.socketManager.GetSkillData(skillID);

                if (skillData != null && !string.IsNullOrEmpty(skillData.skill_Icon_Path))
                {
                    Sprite loadedSprite = Resources.Load<Sprite>(skillData.skill_Icon_Path);
                    if (loadedSprite != null)
                    {
                        slotImages[i].sprite = loadedSprite;
                        slotImages[i].color = (i == selectedSlotIndex) ? selectedColor : filledColor;
                    }
                }
            }
            else
            {
                // 비어있는 슬롯
                slotImages[i].sprite = null;
                slotImages[i].color = (i == selectedSlotIndex) ? selectedColor : emptyColor;
            }
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

    // 마법용 비주얼 업데이트 (아이콘 로드 포함)
public void InitializeForMagic(MagicData data)
{
    if (data == null) return;

    // 1. JSON에 적힌 경로로 아이콘 로드 (예: "Icons/Magic/Fire")
    Sprite loadedIcon = Resources.Load<Sprite>(data.Icon_Path);
    
    if (loadedIcon != null)
    {
        iconImage.sprite = loadedIcon;
        iconImage.enabled = true; // 아이콘 보이기
    }
    else
    {
        Debug.LogWarning($"[UI] 아이콘을 찾을 수 없습니다: {data.Icon_Path}");
        iconImage.enabled = false; // 못 찾으면 숨기기
    }
}


}