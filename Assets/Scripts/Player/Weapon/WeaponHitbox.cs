using UnityEngine;

/// <summary>
/// 무기 Hitbox - OnTriggerEnter만 감지하고 전달
/// </summary>
public class WeaponHitbox : MonoBehaviour
{
    private PlayerController playerController;
    private Collider hitboxCollider;
    
    void Awake()
    {
        // PlayerController 찾기
        playerController = FindObjectOfType<PlayerController>();
        
        if (playerController == null)
        {
            Debug.LogError("[WeaponHitbox] PlayerController를 찾을 수 없습니다!");
        }
        
        // Collider 가져오기
        hitboxCollider = GetComponent<Collider>();
        
        if (hitboxCollider != null)
        {
            // 초기 비활성화
            hitboxCollider.enabled = false;
            Debug.Log("[WeaponHitbox] Collider 초기 비활성화");
        }
        else
        {
            Debug.LogError("[WeaponHitbox] Collider를 찾을 수 없습니다!");
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // PlayerController 체크
        if (playerController == null)
            return;
        
        // ========== PlayerController에 전달 ==========
        playerController.AddHitCollider(other);
        Debug.Log($"[WeaponHitbox] 충돌 감지: {other.name}, Layer: {other.gameObject.layer}");
    }
}