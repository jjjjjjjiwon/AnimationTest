using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 소켓 관리자 - 자동으로 적절한 소켓을 찾아서 사용
/// </summary>
public class SocketManager
{
    /// <summary>보유 중인 소켓들 (최대 5개)</summary>
    private List<ComboSocket> sockets;
    
    /// <summary>현재 사용 중인 소켓 인덱스 (-1 = 콤보 진행 중 아님)</summary>
    private int activeSocketIndex;
    
    /// <summary>최대 소켓 개수</summary>
    private const int MAX_SOCKETS = 5;
    
    // ========================================
    // 생성자
    // ========================================
    
    public SocketManager(PlayerData playerData)
    {
        sockets = new List<ComboSocket>();
        activeSocketIndex = -1;
        
        // PlayerData에서 복원 시도
        if (playerData.socketSlots != null && playerData.socketSlots.Count > 0)
        {
            // TODO: 저장된 소켓 복원
            Debug.Log($"[SocketManager] 소켓 복원: {playerData.socketSlots.Count}개");
        }
        else
        {
            // 기본 소켓 1개 생성
            CreateStartingSocket();
        }
    }
    
    /// <summary>
    /// 시작 소켓 1개 생성
    /// </summary>
    private void CreateStartingSocket()
    {
        ComboSocket firstSocket = new ComboSocket();
        sockets.Add(firstSocket);
        
        Debug.Log("[SocketManager] 시작 소켓 1개 생성!");
    }
    
    // ========================================
    // 소켓 관리
    // ========================================
    
    /// <summary>
    /// 새 소켓 획득
    /// </summary>
    public ComboSocket AcquireNewSocket()
    {
        if (sockets.Count >= MAX_SOCKETS)
        {
            Debug.Log($"[SocketManager] 소켓 최대 개수! ({MAX_SOCKETS}/{MAX_SOCKETS})");
            return null;
        }
        
        ComboSocket newSocket = new ComboSocket();
        sockets.Add(newSocket);
        
        Debug.Log($"[SocketManager] 새 소켓 획득! ({sockets.Count}/{MAX_SOCKETS})");
        return newSocket;
    }
    
    /// <summary>
    /// 소켓 제거
    /// </summary>
    public void RemoveSocket(int index)
    {
        if (index < 0 || index >= sockets.Count)
        {
            Debug.LogWarning($"[SocketManager] 잘못된 인덱스: {index}");
            return;
        }
        
        // 최소 1개는 유지
        if (sockets.Count <= 1)
        {
            Debug.Log("[SocketManager] 마지막 소켓은 제거할 수 없습니다!");
            return;
        }
        
        sockets.RemoveAt(index);
        
        // 활성 소켓 조정
        if (activeSocketIndex >= sockets.Count)
        {
            activeSocketIndex = -1;
        }
        
        Debug.Log($"[SocketManager] 소켓 제거됨");
    }
    
    // ========================================
    // 소켓 자동 찾기 (핵심!)
    // ========================================
    
    /// <summary>
    /// 특정 입력키로 시작하는 소켓 찾기
    /// </summary>
    private int FindSocketByFirstInput(InputTypes input)
    {
        for (int i = 0; i < sockets.Count; i++)
        {
            ComboSocket socket = sockets[i];
            
            // 모든 슬롯이 채워져 있는지 확인
            if (!socket.IsFullyEquipped())
                continue;
            
            // 첫 번째 슬롯의 입력키 확인
            if (socket.GetSlot(0).assignedInput == input)
            {
                return i;  // 찾았음!
            }
        }
        
        return -1;  // 못 찾음
    }
    
    // ========================================
    // 콤보 진행 (자동으로 소켓 찾기!)
    // ========================================
    
    /// <summary>
    /// 콤보 시작 - 자동으로 적절한 소켓 찾기
    /// </summary>
   public bool StartCombo(InputTypes startInput)
{
    // ========== 디버그 추가 ==========
    Debug.Log($"-------------------------------------------------------------------------");

    Debug.Log($"[SocketManager] 소켓 개수: {sockets.Count}");
    for (int i = 0; i < sockets.Count; i++)
    {
        ComboSocket socket = sockets[i];
        bool isFullyEquipped = socket.IsFullyEquipped();
        InputTypes firstInput = InputTypes.None;
        if (isFullyEquipped && socket.GetSlot(0) != null)
        {
            firstInput = socket.GetSlot(0).assignedInput;
        }
        Debug.Log($"  - 소켓 {i}: {socket.socketName}, 완전장착: {isFullyEquipped}, 첫키: {firstInput}");
    }
    // ================================
    
    // 자동으로 소켓 찾기
    int socketIndex = FindSocketByFirstInput(startInput);
    
    if (socketIndex >= 0)
    {
        activeSocketIndex = socketIndex;
        sockets[activeSocketIndex].StartCombo(startInput);
        Debug.Log($"[SocketManager] 소켓 {socketIndex} 시작!");
        return true;
    }
    
    Debug.Log($"[SocketManager] '{startInput}'로 시작하는 사용 가능한 소켓 없음!");
    return false;
}

    /// <summary>
    /// 콤보 이어가기
    /// </summary>
    public bool ProcessNext(InputTypes input)
    {
        // 현재 활성 소켓이 없으면 실패
        if (activeSocketIndex == -1 || activeSocketIndex >= sockets.Count)
        {
            Debug.LogError("[SocketManager] 활성 소켓 없음!");
            return false;
        }
        
        ComboSocket socket = sockets[activeSocketIndex];
        return socket.ProcessNext(input);
    }
    
    /// <summary>
    /// 콤보 리셋
    /// </summary>
    public void ResetCombo()
    {
        if (activeSocketIndex >= 0 && activeSocketIndex < sockets.Count)
        {
            sockets[activeSocketIndex].ResetCombo();
        }
        
        activeSocketIndex = -1;  // 활성 소켓 해제
    }
    
    // ========================================
    // 정보 조회
    // ========================================
    
    /// <summary>
    /// 현재 활성화된 소켓
    /// </summary>
    public ComboSocket GetActiveSocket()
    {
        if (activeSocketIndex >= 0 && activeSocketIndex < sockets.Count)
            return sockets[activeSocketIndex];
        
        return null;
    }
    
    /// <summary>
    /// 특정 인덱스의 소켓
    /// </summary>
    public ComboSocket GetSocket(int index)
    {
        if (index >= 0 && index < sockets.Count)
            return sockets[index];
        
        return null;
    }
    
    /// <summary>
    /// 현재 스킬
    /// </summary>
    public AttackSkillData GetCurrentSkill()
    {
        ComboSocket activeSocket = GetActiveSocket();
        return activeSocket?.GetCurrentSkill();
    }
    
    /// <summary>
    /// 현재 입력키
    /// </summary>
    public InputTypes GetCurrentInput()
    {
        ComboSocket activeSocket = GetActiveSocket();
        return activeSocket?.GetCurrentInput() ?? InputTypes.None;
    }
    
    /// <summary>
    /// 콤보 완료 여부
    /// </summary>
    public bool IsComboComplete()
    {
        ComboSocket activeSocket = GetActiveSocket();
        return activeSocket?.IsComboComplete() ?? false;
    }
    
    /// <summary>
    /// 현재 단계
    /// </summary>
    public int GetCurrentStep()
    {
        ComboSocket activeSocket = GetActiveSocket();
        return activeSocket?.GetCurrentStep() ?? 0;
    }
    
    /// <summary>
    /// 슬롯 개수
    /// </summary>
    public int GetSlotCount()
    {
        ComboSocket activeSocket = GetActiveSocket();
        return activeSocket?.GetSlotCount() ?? 0;
    }
    
    /// <summary>
    /// 콤보 히스토리
    /// </summary>
    public List<AttackSkillData> GetComboHistory()
    {
        ComboSocket activeSocket = GetActiveSocket();
        return activeSocket?.GetComboHistory() ?? new List<AttackSkillData>();
    }
    
    /// <summary>
    /// 보유 소켓 개수
    /// </summary>
    public int GetSocketCount()
    {
        return sockets.Count;
    }
    
    /// <summary>
    /// 현재 활성 소켓 인덱스
    /// </summary>
    public int GetActiveSocketIndex()
    {
        return activeSocketIndex;
    }
    
    /// <summary>
    /// 소켓 최대치 도달 여부
    /// </summary>
    public bool IsFull()
    {
        return sockets.Count >= MAX_SOCKETS;
    }
    
    /// <summary>
    /// 모든 소켓 리스트
    /// </summary>
    public List<ComboSocket> GetAllSockets()
    {
        return new List<ComboSocket>(sockets);
    }
    
    /// <summary>
    /// 사용 가능한 소켓 개수 (모든 슬롯이 채워진 소켓)
    /// </summary>
    public int GetReadySocketCount()
    {
        int count = 0;
        foreach (ComboSocket socket in sockets)
        {
            if (socket.IsFullyEquipped())
                count++;
        }
        return count;
    }
}