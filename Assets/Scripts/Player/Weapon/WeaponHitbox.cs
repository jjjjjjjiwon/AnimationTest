using UnityEngine;
using System.Collections.Generic;

public class WeaponHitbox : MonoBehaviour
{
    private PlayerController playerController;
    private Collider hitboxCollider;
    
    private List<GameObject> hitTargets = new List<GameObject>();
    private PlayerSkillData currentSkill;

    void Awake()
    {
        playerController = FindObjectOfType<PlayerController>();
        hitboxCollider = GetComponent<Collider>();
        
        if (hitboxCollider != null)
            hitboxCollider.enabled = false;
    }

    public void EnableHitbox(PlayerSkillData skillData)
    {
        currentSkill = skillData;
        hitTargets.Clear(); 
        if (hitboxCollider != null) hitboxCollider.enabled = true;
    }

    public void DisableHitbox()
    {
        if (hitboxCollider != null) hitboxCollider.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        // 1. 레이어 체크 (14번)
        if (other.gameObject.layer != 14) return;
        
        // 2. 적의 루트 오브젝트 찾기
        GameObject rootObj = other.transform.root.gameObject;
        if (hitTargets.Contains(rootObj)) return;

        // 3. TestEnemyController를 찾아서 대미지 전달
        if (rootObj.TryGetComponent(out TestEnemyController enemy))
        {
            hitTargets.Add(rootObj);
            
            // 데이터 추출
            float damage = (currentSkill != null) ? currentSkill.skill_Damage : 10f;
            float stunAdd = (currentSkill != null) ? currentSkill.perfect_Stun_Add : 0.1f;

            // --- [여기가 추가된 타격 연출 부분] ---
            if (HitEffectManager.Instance != null)
            {
                // PlayerController에 IsPerfectTiming이 있다고 가정
                bool isPerfect = playerController != null && playerController.IsPerfectTiming; 
                
                float stopDuration = isPerfect ? 0.06f : 0.02f;
                HitEffectManager.Instance.HitStop(stopDuration);
                // 카메라 흔들림은 나중에 구현하더라도 일단 호출
                HitEffectManager.Instance.CameraShake(isPerfect ? 0.5f : 0.2f, 0.1f);
            }
            // ------------------------------------

            // 적에게 최종 전달
            enemy.TakeDamage(damage, stunAdd);

            Debug.Log($"[Hit!] {currentSkill?.skill_Name} -> {rootObj.name} (Perfect: {playerController.IsPerfectTiming})");
        }
    }
}