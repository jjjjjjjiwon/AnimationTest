using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 콤보 소켓 시스템
/// </summary>
public class ComboSocket
{
    // ========================================
    // 상수
    // ========================================
    
    private const int MAX_SOCKETS = 5;
    
    // ========================================
    // 변수
    // ========================================
    
    private List<ComboSocketSlot> socketSlots;
    private int currentSocketIndex;
    
    // ========================================
    // 생성자
    // ========================================
    
    public ComboSocket(PlayerData playerData)
    {
        socketSlots = new List<ComboSocketSlot>();
        currentSocketIndex = -1;
        
        // PlayerData에서 복원
        if (playerData.socketSlots != null && playerData.socketSlots.Count > 0)
        {
            foreach (var data in playerData.socketSlots)
            {
                ComboSocketSlot slot = new ComboSocketSlot(data.assignedInput);
                slot.equippedSkill = data.equippedSkill;
                socketSlots.Add(slot);
            }
            Debug.Log($"소켓 복원: {socketSlots.Count}개");
        }
        else
        {
            CreateDefaultSocket();
        }
    }
    
    private void CreateDefaultSocket()
    {
        ComboSocketSlot defaultSocket = new ComboSocketSlot(InputTypes.LeftClick);
        socketSlots.Add(defaultSocket);
        Debug.Log("기본 소켓 생성: 좌클릭");
    }
    
    // ========================================
    // 소켓 관리
    // ========================================
    
    public ComboSocketSlot AcquireNewSocket()
    {
        InputTypes randomInput = GetRandomInput();
        ComboSocketSlot newSocket = new ComboSocketSlot(randomInput);
        
        Debug.Log($"새 소켓 획득! 입력: {randomInput}");
        
        if (socketSlots.Count < MAX_SOCKETS)
        {
            socketSlots.Add(newSocket);
            Debug.Log($"소켓 추가됨! ({socketSlots.Count}/{MAX_SOCKETS})");
        }
        else
        {
            Debug.Log("소켓 최대! UI에서 교체 필요");
        }
        
        return newSocket;
    }
    
    public void ReplaceSocket(int removeIndex, ComboSocketSlot newSocket)
    {
        if (removeIndex < 0 || removeIndex >= socketSlots.Count)
        {
            Debug.LogWarning($"잘못된 인덱스: {removeIndex}");
            return;
        }
        
        InputTypes oldInput = socketSlots[removeIndex].assignedInput;
        socketSlots[removeIndex] = newSocket;
        
        Debug.Log($"소켓 교체! {oldInput} → {newSocket.assignedInput}");
    }
    
    public void RemoveSocket(int index)
    {
        if (index < 0 || index >= socketSlots.Count)
        {
            Debug.LogWarning($"잘못된 인덱스: {index}");
            return;
        }
        
        InputTypes removed = socketSlots[index].assignedInput;
        socketSlots.RemoveAt(index);
        
        Debug.Log($"소켓 제거! {removed}");
    }
    
    // ========================================
    // 스킬 장착
    // ========================================
    
    public void EquipSkill(int socketIndex, AttackSkillData skill)
    {
        if (socketIndex < 0 || socketIndex >= socketSlots.Count)
        {
            Debug.LogWarning($"잘못된 소켓 인덱스: {socketIndex}");
            return;
        }
        
        socketSlots[socketIndex].equippedSkill = skill;
        
        string skillName = skill != null ? skill.skillName : "없음";
        Debug.Log($"소켓{socketIndex + 1} ({socketSlots[socketIndex].assignedInput}): [{skillName}]");
    }
    
    // ========================================
    // 콤보 진행
    // ========================================
    
    public bool StartCombo(InputTypes input)
    {
        if (socketSlots.Count == 0)
        {
            Debug.Log("소켓이 없습니다!");
            return false;
        }
        
        if (socketSlots[0].assignedInput != input)
        {
            Debug.Log($"틀린 입력! [필요: {socketSlots[0].assignedInput}] [입력: {input}]");
            return false;
        }
        
        if (socketSlots[0].equippedSkill == null)
        {
            Debug.Log("소켓1에 스킬 없음!");
            return false;
        }
        
        currentSocketIndex = 0;
        Debug.Log($"콤보 시작! [{socketSlots[0].equippedSkill.skillName}]");
        return true;
    }
    
    public bool ProcessNext(InputTypes input)
    {
        int nextIndex = currentSocketIndex + 1;
        
        if (nextIndex >= socketSlots.Count)
        {
            Debug.Log("마지막 소켓!");
            return false;
        }
        
        if (socketSlots[nextIndex].assignedInput != input)
        {
            Debug.Log($"틀린 입력! [필요: {socketSlots[nextIndex].assignedInput}] [입력: {input}]");
            return false;
        }
        
        if (socketSlots[nextIndex].equippedSkill == null)
        {
            Debug.Log($"소켓{nextIndex + 1}에 스킬 없음!");
            return false;
        }
        
        currentSocketIndex = nextIndex;
        Debug.Log($"콤보 {currentSocketIndex + 1}타! [{socketSlots[nextIndex].equippedSkill.skillName}]");
        return true;
    }
    
    public void ResetCombo()
    {
        currentSocketIndex = -1;
    }
    
    // ========================================
    // 정보 조회
    // ========================================
    
    public AttackSkillData GetCurrentSkill()
    {
        if (currentSocketIndex < 0 || currentSocketIndex >= socketSlots.Count)
            return null;
        
        return socketSlots[currentSocketIndex].equippedSkill;
    }
    
    public InputTypes GetCurrentInput()
    {
        if (currentSocketIndex < 0 || currentSocketIndex >= socketSlots.Count)
            return InputTypes.None;
        
        return socketSlots[currentSocketIndex].assignedInput;
    }
    
    public bool IsComboComplete()
    {
        if (currentSocketIndex < 0)
            return false;
        
        return currentSocketIndex + 1 >= socketSlots.Count;
    }
    
    public int GetSocketCount()
    {
        return socketSlots.Count;
    }
    
    public bool IsFull()
    {
        return socketSlots.Count >= MAX_SOCKETS;
    }
    
    public ComboSocketSlot GetSocket(int index)
    {
        if (index < 0 || index >= socketSlots.Count)
            return null;
        
        return socketSlots[index];
    }
    
    public List<ComboSocketSlot> GetAllSockets()
    {
        return new List<ComboSocketSlot>(socketSlots);
    }
    
    public int GetCurrentStep()
    {
        return currentSocketIndex + 1;
    }
    
    // ========================================
    // Private
    // ========================================
    
    private InputTypes GetRandomInput()
    {
        // Random.Range: 0 ~ 4 (5개 중 랜덤)
        int randomValue = Random.Range(0, 5);
        
        switch (randomValue)
        {
            case 0: return InputTypes.LeftClick;
            case 1: return InputTypes.RightClick;
            case 2: return InputTypes.QKey;
            case 3: return InputTypes.EKey;
            case 4: return InputTypes.RKey;
            default: return InputTypes.LeftClick;
        }
    }
}