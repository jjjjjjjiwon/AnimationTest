using UnityEngine;

/// <summary>
/// 인벤토리 슬롯 (한 칸)
/// - itemID로 아이템 식별
/// - count로 개수 관리
/// </summary>
[System.Serializable]
public class InventorySlot
{
    public int itemID = -1;  // -1 = 빈 슬롯
    public int count = 0;
    
    public bool IsEmpty => itemID == -1 || count <= 0;
}