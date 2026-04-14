using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ComboSocket
{
    public string socketName;
    public List<ComboSlot> slots;
    private int currentSlotIndex = -1;
    private const int SLOT_COUNT = 5;
    private float lastProcessTime = 0f;

    

    public ComboSocket(string name = "")
    {
        socketName = string.IsNullOrEmpty(name) ? $"콤보 #{Random.Range(1000, 9999)}" : name;
        slots = new List<ComboSlot>();
        currentSlotIndex = -1;

        InputTypes[] inputPool = {
            InputTypes.LeftClick,
            InputTypes.RightClick,
            InputTypes.QKey,
            InputTypes.EKey,
            InputTypes.RKey
        };

        for (int i = 0; i < SLOT_COUNT; i++)
        {
            InputTypes randomInput = inputPool[Random.Range(0, inputPool.Length)];
            slots.Add(new ComboSlot(randomInput));
        }

        string inputSequence = string.Join(" → ", slots.ConvertAll(s => s.assignedInput.ToString()));
        Debug.Log($"[소켓] '{socketName}' 생성 완료: {inputSequence}");
    }

    // [수정] 스킬 장착 (ID 기반)
public void EquipSkill(int index, PlayerSkillData skill)
{
    // 매개변수 index를 사용하도록 수정
    if (index < 0 || index >= slots.Count) return;

    // skill에서 ID를 가져와서 저장 (skillID가 아니라 skill.id 혹은 skill.skill_ID 등 정의된 이름 확인)
    slots[index].equippedSkillID = skill.player_Skill_ID; 
    
    Debug.Log($"[소켓] {socketName} 슬롯{index + 1}에 {skill.skill_Name} 장착");
}

    // [수정] 입력키로 슬롯 찾아 스킬 장착 (ID 기반으로 변경)
public void EquipSkillByInput(InputTypes input, PlayerSkillData skill)
{
    if (skill == null) return;

    for (int i = 0; i < slots.Count; i++)
    {
        if (slots[i].assignedInput == input)
        {
            // 이제 EquipSkill은 PlayerSkillData를 받으므로 객체를 그대로 넘깁니다.
            EquipSkill(i, skill); 
            return;
        }
    }
    Debug.LogWarning($"[소켓] '{input}' 입력키를 가진 슬롯이 없습니다.");
}

    public bool IsFullyEquipped()
    {
        foreach (ComboSlot slot in slots)
        {
            if (!slot.HasSkill()) return false;
        }
        return true;
    }

    public int GetEmptySlotCount()
    {
        int count = 0;
        foreach (ComboSlot slot in slots)
        {
            if (!slot.HasSkill()) count++;
        }
        return count;
    }

    // [수정] 콤보 시작 시 로그 출력 방식 변경
    public bool StartCombo(InputTypes input)
    {
        if (!IsFullyEquipped())
        {
            Debug.Log($"[소켓] 콤보 사용 불가 (빈 슬롯 존재)");
            return false;
        }

        if (slots[0].assignedInput != input) return false;

        currentSlotIndex = 0;
        
        // 데이터 로더에서 이름을 가져와서 로그 출력
        var skillData = GetCurrentSkill();
        string skillName = (skillData != null) ? skillData.skill_Name : slots[0].equippedSkillID;
        Debug.Log($"[소켓] '{socketName}' 시작! 첫 스킬: {skillName}");
        
        return true;
    }

    public bool ProcessNext(InputTypes input)
    {
        if (Time.time - lastProcessTime < 0.1f) return false;
        if (currentSlotIndex + 1 >= slots.Count) return false;

        var nextSlot = slots[currentSlotIndex + 1];

        if (nextSlot.assignedInput == input)
        {
            currentSlotIndex++;
            lastProcessTime = Time.time;
            Debug.Log($"[콤보 성공] {currentSlotIndex + 1}단계 진행");
            return true;
        }
        return false;
    }

    public void ResetCombo() => currentSlotIndex = -1;

    public PlayerSkillData GetCurrentSkill()
    {
        if (currentSlotIndex < 0 || currentSlotIndex >= slots.Count) return null;
        return PlayerSkillLoader.GetSkill(slots[currentSlotIndex].equippedSkillID);
    }

    public InputTypes GetCurrentInput()
    {
        if (currentSlotIndex < 0 || currentSlotIndex >= slots.Count) return InputTypes.None;
        return slots[currentSlotIndex].assignedInput;
    }

    public bool IsComboComplete()
    {
        if (currentSlotIndex < 0) return false;
        return currentSlotIndex + 1 >= slots.Count;
    }

    public int GetCurrentStep() => currentSlotIndex + 1;
    public ComboSlot GetSlot(int index) => (index >= 0 && index < slots.Count) ? slots[index] : null;
    public int GetSlotCount() => slots.Count;

    public List<PlayerSkillData> GetComboHistory()
    {
        List<PlayerSkillData> history = new List<PlayerSkillData>();
        for (int i = 0; i <= currentSlotIndex; i++)
        {
            if (i < slots.Count && slots[i].HasSkill())
            {
                history.Add(PlayerSkillLoader.GetSkill(slots[i].equippedSkillID));
            }
        }
        return history;
    }

    // ========================================
    // UI 지원을 위한 추가 함수들
    // ========================================

    /// <summary>
    /// UI에서 슬롯 리스트를 가져가기 위한 함수
    /// </summary>
    public List<ComboSlot> GetSlots()
    {
        return slots;
    }

    /// <summary>
    /// 특정 인덱스의 스킬 데이터를 가져옴
    /// </summary>
    public PlayerSkillData GetSkillAt(int index)
    {
        if (index < 0 || index >= slots.Count) return null;
        if (string.IsNullOrEmpty(slots[index].equippedSkillID)) return null;
        
        // 유저님의 프로젝트 구조에 맞게 PlayerSkillLoader를 사용합니다.
        return PlayerSkillLoader.GetSkill(slots[index].equippedSkillID);
    }
}