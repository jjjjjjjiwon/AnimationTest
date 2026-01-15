using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 플레이어 인벤토리 (3x3 = 9칸)
/// </summary>
[System.Serializable]
public class PlayerInventory
{
    private InventorySlot[] slots = new InventorySlot[9];
    
    // ========================================
    // 생성자
    // ========================================
    
    public PlayerInventory()
    {
        // 9개 빈 슬롯 초기화
        for (int i = 0; i < 9; i++)
        {
            slots[i] = new InventorySlot();
        }
        
        Debug.Log("[인벤토리] 초기화 완료 - 9개 슬롯");
    }
    
    // ========================================
    // 아이템 추가
    // ========================================
    
    /// <summary>아이템 추가 (ID로)</summary>
    public bool AddItem(int itemID, int count = 1)
    {
        // 1. 이미 있는 슬롯에 추가
        foreach (var slot in slots)
        {
            if (slot.itemID == itemID)
            {
                slot.count += count;
                Debug.Log($"[인벤토리] 아이템 추가 (기존) - ID:{itemID}, 개수:{slot.count}");
                return true;
            }
        }
        
        // 2. 빈 슬롯에 추가
        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                slot.itemID = itemID;
                slot.count = count;
                Debug.Log($"[인벤토리] 아이템 추가 (새로) - ID:{itemID}, 개수:{count}");
                return true;
            }
        }
        
        Debug.Log("[인벤토리] 가득 참!");
        return false;
    }
    
    // ========================================
    // 아이템 제거
    // ========================================
    
    /// <summary>아이템 제거</summary>
    public bool RemoveItem(int itemID, int count = 1)
    {
        foreach (var slot in slots)
        {
            if (slot.itemID == itemID)
            {
                slot.count -= count;
                
                if (slot.count <= 0)
                {
                    // 슬롯 비우기
                    slot.itemID = -1;
                    slot.count = 0;
                }
                
                Debug.Log($"[인벤토리] 아이템 제거 - ID:{itemID}");
                return true;
            }
        }
        
        return false;
    }
    
    // ========================================
    // 조회
    // ========================================
    
    /// <summary>슬롯 가져오기</summary>
    public InventorySlot GetSlot(int index)
    {
        if (index >= 0 && index < slots.Length)
            return slots[index];
        
        return null;
    }
    
    /// <summary>총 슬롯 수</summary>
    public int GetSlotCount()
    {
        return slots.Length;
    }
}