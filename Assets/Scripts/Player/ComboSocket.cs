using UnityEngine;

/// <summary>
/// 콤보 소켓 시스템
/// - 플레이어가 공격 스킬을 소켓에 자유롭게 배치
/// - 배치된 순서대로 콤보 진행
/// </summary>
public class ComboSocket
{
    // ========================================
    // 소켓 설정
    // ========================================
    
    /// <summary>최대 소켓 개수</summary>
    private const int MAX_SOCKETS = 5;
    
    /// <summary>현재 장착된 스킬들</summary>
    private AttackSkillData[] equippedSkills = new AttackSkillData[MAX_SOCKETS];
    
    /// <summary>현재 진행 중인 소켓 인덱스 (-1 = 콤보 안 함)</summary>
    private int currentSocketIndex = -1;
    
    // ========================================
    // 생성자
    // ========================================
    
    public ComboSocket()
    {
        // 빈 소켓으로 초기화
        for (int i = 0; i < MAX_SOCKETS; i++)
        {
            equippedSkills[i] = null;
        }
    }
    
    // ========================================
    // 소켓 장착
    // ========================================
    
    /// <summary>
    /// 소켓에 스킬 장착
    /// </summary>
    public void EquipSkill(int socketIndex, AttackSkillData skill)
    {
        if (socketIndex < 0 || socketIndex >= MAX_SOCKETS)
        {
            Debug.LogWarning($"잘못된 소켓 인덱스: {socketIndex}");
            return;
        }
        
        equippedSkills[socketIndex] = skill;
        
        if (skill != null)
            Debug.Log($"소켓 {socketIndex + 1}에 [{skill.skillName}] 장착");
        else
            Debug.Log($"소켓 {socketIndex + 1} 비움");
    }
    
    /// <summary>
    /// 소켓 해제
    /// </summary>
    public void UnequipSkill(int socketIndex)
    {
        EquipSkill(socketIndex, null);
    }
    
    // ========================================
    // 콤보 진행
    // ========================================
    
    /// <summary>
    /// 콤보 시작
    /// </summary>
    public bool StartCombo()
    {
        // 1번 소켓이 비어있으면 실패
        if (equippedSkills[0] == null)
        {
            Debug.Log("1번 소켓이 비어있어 콤보 시작 불가!");
            return false;
        }
        
        // 콤보 시작
        currentSocketIndex = 0;
        Debug.Log($"콤보 시작: [{equippedSkills[0].skillName}]");
        return true;
    }
    
    /// <summary>
    /// 다음 소켓으로 진행
    /// </summary>
    public bool ProcessNext()
    {
        int nextIndex = currentSocketIndex + 1;
        
        // 범위 초과
        if (nextIndex >= MAX_SOCKETS)
        {
            Debug.Log("마지막 소켓! 더 이상 진행 불가");
            return false;
        }
        
        // 다음 소켓이 비어있음
        if (equippedSkills[nextIndex] == null)
        {
            Debug.Log($"소켓 {nextIndex + 1}이 비어있어 콤보 종료");
            return false;
        }
        
        // 다음 소켓으로 진행
        currentSocketIndex = nextIndex;
        Debug.Log($"콤보 진행: {currentSocketIndex + 1}타 [{equippedSkills[nextIndex].skillName}]");
        return true;
    }
    
    /// <summary>
    /// 콤보 리셋
    /// </summary>
    public void ResetCombo()
    {
        currentSocketIndex = -1;
        Debug.Log("콤보 리셋");
    }
    
    // ========================================
    // 정보 가져오기
    // ========================================
    
    /// <summary>
    /// 현재 스킬 가져오기
    /// </summary>
    public AttackSkillData GetCurrentSkill()
    {
        if (currentSocketIndex < 0 || currentSocketIndex >= MAX_SOCKETS)
            return null;
        
        return equippedSkills[currentSocketIndex];
    }
    
    /// <summary>
    /// 특정 소켓의 스킬 가져오기
    /// </summary>
    public AttackSkillData GetSkillAt(int socketIndex)
    {
        if (socketIndex < 0 || socketIndex >= MAX_SOCKETS)
            return null;
        
        return equippedSkills[socketIndex];
    }
    
    /// <summary>
    /// 현재 소켓 인덱스
    /// </summary>
    public int GetCurrentSocketIndex()
    {
        return currentSocketIndex;
    }
    
    /// <summary>
    /// 콤보 완료 체크
    /// </summary>
    public bool IsComboComplete()
    {
        // 콤보 진행 중이 아님
        if (currentSocketIndex < 0)
            return false;
        
        int nextIndex = currentSocketIndex + 1;
        
        // 마지막 소켓이거나 다음 소켓이 비어있으면 완료
        return nextIndex >= MAX_SOCKETS || equippedSkills[nextIndex] == null;
    }
    
    /// <summary>
    /// 장착된 스킬 개수
    /// </summary>
    public int GetEquippedCount()
    {
        int count = 0;
        for (int i = 0; i < MAX_SOCKETS; i++)
        {
            if (equippedSkills[i] != null)
                count++;
            else
                break; // 빈 칸 나오면 중단 (중간에 빈 칸 있으면 안 됨)
        }
        return count;
    }
}