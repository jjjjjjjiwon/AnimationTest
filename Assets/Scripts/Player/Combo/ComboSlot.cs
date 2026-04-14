using UnityEngine;

/// <summary>
/// 개별 슬롯 - 1개 입력키 + 1개 스킬
/// </summary>
[System.Serializable]
public class ComboSlot
{
    public InputTypes assignedInput;
    public string equippedSkillID; // [수정] 스크립터블 오브젝트 대신 ID 저장

    public ComboSlot(InputTypes input)
    {
        assignedInput = input;
        equippedSkillID = ""; 
    }

    public bool HasSkill() => !string.IsNullOrEmpty(equippedSkillID);
}