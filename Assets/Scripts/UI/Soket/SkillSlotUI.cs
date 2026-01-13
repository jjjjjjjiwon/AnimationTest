using UnityEngine;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;
    
    private AttackSkillData skillData;
    private SocketManagerUI manager;
    
    public void Initialize(AttackSkillData data, SocketManagerUI managerUI)
    {
        skillData = data;
        manager = managerUI;
        
        button.onClick.AddListener(OnClick);
        
        UpdateUI();
    }
    
    public void UpdateUI()
    {
        if (skillData != null && skillData.skillIcon != null)
        {
            iconImage.sprite = skillData.skillIcon;
        }
    }
    
    private void OnClick()
    {
        manager.EquipSkillToSelectedSocket(skillData);
    }
}