using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public string weaponName;   // 이름
    public float damage = 25f;  // 대미지
    public float attackRange = 1.5f;    // 범위
    public float attackSpeed = 1f;      // 공속
    // 필요 시 다른 속성 추가 가능 (스턴, 크리티컬 등)
}