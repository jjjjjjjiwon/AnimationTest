using UnityEngine;

/// <summary>
/// 무기 데이터
/// </summary>
[CreateAssetMenu(fileName = "NewWeapon", menuName = "Combat/Weapon")]
public class WeaponData : ScriptableObject
{
    [Header("기본 정보")]
    public string weaponName;
    public Sprite icon;
    
    [Header("능력치")]
    [Tooltip("무기 데미지")]
    public float damage = 50f;
    
    // 나중에 추가:
    // public float attackSpeed;
    // public float range;
    // public WeaponType type;
}