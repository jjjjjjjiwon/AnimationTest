using UnityEngine;

/// <summary>
/// 개별 소켓 슬롯
/// - 입력키 + 장착된 스킬
/// </summary>
[System.Serializable]
public class ComboSocketSlot
{
    /// <summary>이 소켓에 할당된 입력키</summary>
    public InputTypes assignedInput;
    
    /// <summary>이 소켓에 장착된 스킬</summary>
    public AttackSkillData equippedSkill;
    
    public ComboSocketSlot(InputTypes input)
    {
        assignedInput = input;
        equippedSkill = null;
    }
}