using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 스킬 슬롯 UI
/// </summary>
public class SkillSlotUI : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI damageText;
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
        if (skillData != null)
        {
            skillNameText.text = skillData.skillName;
            damageText.text = $"Damage: {skillData.baseDamage}";
            
            if (skillData.skillIcon != null)
                iconImage.sprite = skillData.skillIcon;
        }
    }
    
    private void OnClick()
    {
        manager.EquipSkillToSelectedSocket(skillData);
    }
}