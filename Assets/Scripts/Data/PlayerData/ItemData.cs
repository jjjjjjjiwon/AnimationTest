using UnityEngine;

/// <summary>
/// 아이템 데이터 (설계도)
/// - JSON에서 로드하여 생성
/// </summary>
[CreateAssetMenu(fileName = "ItemData", menuName = "Data/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    public int itemID;
    public string itemName;
    public Sprite icon;
    
    [Header("타입")]
    public ItemType itemType;
    
    [Header("효과")]
    [Tooltip("체력 회복량")]
    public float healAmount = 0;
    
    [Tooltip("아이템 설명")]
    [TextArea(3, 5)]
    public string description;
}

/// <summary>
/// 아이템 타입
/// </summary>
public enum ItemType
{
    Consumable,  // 소모품 (포션 등)
    Equipment,   // 장비 (무기, 방어구)
    Material,    // 재료
    KeyItem      // 퀘스트 아이템
}