using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 소켓 = 5개 슬롯의 묶음 = 1개의 콤보 세트
/// 모든 슬롯의 입력키가 랜덤으로 배치됨
/// </summary>
[System.Serializable]
public class ComboSocket
{
    /// <summary>소켓 이름 (식별용)</summary>
    public string socketName;
    
    /// <summary>5개의 슬롯</summary>
    public List<ComboSlot> slots;
    
    /// <summary>현재 실행 중인 슬롯 인덱스 (-1 = 콤보 시작 전)</summary>
    private int currentSlotIndex;
    
    // ========================================
    // 생성자
    // ========================================
    
    /// <summary>
    /// 완전 랜덤 입력키로 소켓 생성
    /// 슬롯1~5 모두 랜덤 배치
    /// </summary>
    public ComboSocket(string name = "")
    {
        socketName = string.IsNullOrEmpty(name) ? $"콤보 #{Random.Range(1000, 9999)}" : name;
        slots = new List<ComboSlot>();
        currentSlotIndex = -1;
        
        // 5개 입력키 리스트
        List<InputTypes> allInputs = new List<InputTypes>
        {
            InputTypes.LeftClick,
            InputTypes.RightClick,
            InputTypes.QKey,
            InputTypes.EKey,
            InputTypes.RKey
        };
        
        // Fisher-Yates 셔플 (완전 랜덤 섞기)
        for (int i = allInputs.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            InputTypes temp = allInputs[i];
            allInputs[i] = allInputs[randomIndex];
            allInputs[randomIndex] = temp;
        }
        
        // 섞인 순서대로 슬롯 생성 (스킬은 비어있음)
        foreach (InputTypes input in allInputs)
        {
            slots.Add(new ComboSlot(input));
        }
        
        // 디버그 로그 (입력 순서 확인)
        string inputSequence = "";
        for (int i = 0; i < slots.Count; i++)
        {
            inputSequence += slots[i].assignedInput.ToString();
            if (i < slots.Count - 1)
                inputSequence += " → ";
        }
        
        Debug.Log($"[소켓] '{socketName}' 생성 (5개 빈 슬롯)");
        Debug.Log($"[소켓] 입력키 배치: {inputSequence}");
    }
    
    // ========================================
    // 스킬 장착
    // ========================================
    
    /// <summary>
    /// 특정 슬롯에 스킬 장착
    /// </summary>
    public void EquipSkill(int slotIndex, AttackSkillData skill)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count)
        {
            Debug.LogWarning($"[소켓] 잘못된 슬롯 인덱스: {slotIndex}");
            return;
        }
        
        slots[slotIndex].equippedSkill = skill;
        
        string skillName = skill != null ? skill.skillName : "없음";
        Debug.Log($"[소켓] '{socketName}' 슬롯{slotIndex + 1} ({slots[slotIndex].assignedInput}): [{skillName}]");
    }
    
    /// <summary>
    /// 입력키로 슬롯 찾아서 스킬 장착
    /// </summary>
    public void EquipSkillByInput(InputTypes input, AttackSkillData skill)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].assignedInput == input)
            {
                EquipSkill(i, skill);
                return;
            }
        }
        
        Debug.LogWarning($"[소켓] '{input}' 입력키를 가진 슬롯 없음!");
    }
    
    // ========================================
    // 슬롯 체크
    // ========================================
    
    /// <summary>
    /// 모든 슬롯이 스킬로 채워져 있는가?
    /// </summary>
    public bool IsFullyEquipped()
    {
        foreach (ComboSlot slot in slots)
        {
            if (!slot.HasSkill())
            {
                return false;
            }
        }
        return true;
    }
    
    /// <summary>
    /// 비어있는 슬롯 개수
    /// </summary>
    public int GetEmptySlotCount()
    {
        int count = 0;
        foreach (ComboSlot slot in slots)
        {
            if (!slot.HasSkill())
                count++;
        }
        return count;
    }
    
    // ========================================
    // 콤보 진행
    // ========================================
    
    /// <summary>
    /// 콤보 시작
    /// </summary>
    public bool StartCombo(InputTypes input)
    {
        // 모든 슬롯 채워져 있는지 체크
        if (!IsFullyEquipped())
        {
            int emptyCount = GetEmptySlotCount();
            Debug.Log($"[소켓] 콤보 사용 불가! (빈 슬롯: {emptyCount}개)");
            return false;
        }
        
        // 첫 번째 슬롯 확인
        if (slots[0].assignedInput != input)
        {
            Debug.Log($"[소켓] 틀린 입력! [필요: {slots[0].assignedInput}] [입력: {input}]");
            return false;
        }
        
        currentSlotIndex = 0;
        Debug.Log($"[소켓] '{socketName}' 콤보 시작! [{slots[0].equippedSkill.skillName}]");
        return true;
    }
    
    /// <summary>
    /// 콤보 이어가기
    /// </summary>
    public bool ProcessNext(InputTypes input)
    {
        int nextIndex = currentSlotIndex + 1;
        
        // 마지막 슬롯 체크
        if (nextIndex >= slots.Count)
        {
            Debug.Log($"[소켓] 마지막 슬롯!");
            return false;
        }
        
        // 입력키 확인
        if (slots[nextIndex].assignedInput != input)
        {
            Debug.Log($"[소켓] 틀린 입력! [필요: {slots[nextIndex].assignedInput}] [입력: {input}]");
            return false;
        }
        
        currentSlotIndex = nextIndex;
        Debug.Log($"[소켓] '{socketName}' 콤보 {currentSlotIndex + 1}타! [{slots[nextIndex].equippedSkill.skillName}]");
        return true;
    }
    
    /// <summary>
    /// 콤보 리셋
    /// </summary>
    public void ResetCombo()
    {
        currentSlotIndex = -1;
    }
    
    // ========================================
    // 정보 조회
    // ========================================
    
    /// <summary>
    /// 현재 슬롯의 스킬
    /// </summary>
    public AttackSkillData GetCurrentSkill()
    {
        if (currentSlotIndex < 0 || currentSlotIndex >= slots.Count)
            return null;
        
        return slots[currentSlotIndex].equippedSkill;
    }
    
    /// <summary>
    /// 현재 슬롯의 입력키
    /// </summary>
    public InputTypes GetCurrentInput()
    {
        if (currentSlotIndex < 0 || currentSlotIndex >= slots.Count)
            return InputTypes.None;
        
        return slots[currentSlotIndex].assignedInput;
    }
    
    /// <summary>
    /// 콤보 완료 여부 (마지막 슬롯까지 실행)
    /// </summary>
    public bool IsComboComplete()
    {
        if (currentSlotIndex < 0)
            return false;
        
        return currentSlotIndex + 1 >= slots.Count;
    }
    
    /// <summary>
    /// 현재 단계 (1부터 시작)
    /// </summary>
    public int GetCurrentStep()
    {
        return currentSlotIndex + 1;
    }
    
    /// <summary>
    /// 특정 슬롯 가져오기
    /// </summary>
    public ComboSlot GetSlot(int index)
    {
        if (index < 0 || index >= slots.Count)
            return null;
        
        return slots[index];
    }
    
    /// <summary>
    /// 슬롯 개수
    /// </summary>
    public int GetSlotCount()
    {
        return slots.Count;
    }
    
    /// <summary>
    /// 현재까지 사용한 스킬 히스토리
    /// </summary>
    public List<AttackSkillData> GetComboHistory()
    {
        List<AttackSkillData> history = new List<AttackSkillData>();
        
        for (int i = 0; i <= currentSlotIndex; i++)
        {
            if (i < slots.Count && slots[i].HasSkill())
            {
                history.Add(slots[i].equippedSkill);
            }
        }
        
        return history;
    }
}