using UnityEngine;

/// <summary>
/// 개별 슬롯 - 1개 입력키 + 1개 스킬
/// </summary>
[System.Serializable]
public class ComboSlot
{
    /// <summary>이 슬롯에 할당된 입력키</summary>
    public InputTypes assignedInput;
    
    /// <summary>이 슬롯에 장착된 스킬</summary>
    public AttackSkillData equippedSkill;
    
    public ComboSlot(InputTypes input)
    {
        assignedInput = input;
        equippedSkill = null;
    }
    
    /// <summary>스킬이 장착되어 있는가?</summary>
    public bool HasSkill()
    {
        return equippedSkill != null;
    }
}