using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 개별 소켓 UI
/// </summary>
public class SocketSlotUI : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI inputText;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private Button button;
    
    [Header("색상")]
    [SerializeField] private Color emptyColor = Color.gray;
    [SerializeField] private Color filledColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;
    
    private int socketIndex;
    private ComboSocketSlot socketData;
    private SocketManagerUI manager;
    private bool isSelected = false;
    
    public void Initialize(int index, ComboSocketSlot data, SocketManagerUI managerUI)
    {
        socketIndex = index;
        socketData = data;
        manager = managerUI;
        
        button.onClick.AddListener(OnClick);
        
        UpdateUI();
    }
    
    public void UpdateUI()
    {
        if (socketData == null)
        {
            inputText.text = "none";
            skillNameText.text = "Emepty";
            iconImage.color = emptyColor;
            iconImage.sprite = null;
        }
        else
        {
            // 입력키
            inputText.text = GetInputKeyString(socketData.assignedInput);
            
            // 스킬
            if (socketData.equippedSkill != null)
            {
                skillNameText.text = socketData.equippedSkill.skillName;
                iconImage.sprite = socketData.equippedSkill.icon;
                iconImage.color = filledColor;
            }
            else
            {
                skillNameText.text = "Emepty";
                iconImage.color = emptyColor;
                iconImage.sprite = null;
            }
        }
        
        // 선택 표시
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
            backgroundImage.color = isSelected ? selectedColor : Color.white;
        }
    }
    
    private void OnClick()
    {
        if (socketData == null)
        {
            Debug.Log("빈 소켓!");
            return;
        }
        
        manager.SelectSocket(socketIndex);
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