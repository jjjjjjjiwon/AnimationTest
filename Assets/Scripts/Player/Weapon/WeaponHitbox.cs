using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 무기 Hitbox
/// - OnTriggerEnter로 타격 감지
/// - 중복 타격 방지
/// </summary>
public class WeaponHitbox : MonoBehaviour
{
    private PlayerController playerController;
    private HashSet<Collider> hitEnemies = new HashSet<Collider>();
    
    void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Enemy 레이어 체크
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemy"))
            return;
        
        // 중복 타격 방지
        if (hitEnemies.Contains(other))
            return;
        
        // 타격 기록
        hitEnemies.Add(other);
        
        // PlayerController에 알림
        playerController.OnWeaponHit(other);
    }
    
    /// <summary>공격 시작 시 호출 (중복 리스트 초기화)</summary>
    public void ResetHitList()
    {
        hitEnemies.Clear();
    }
}