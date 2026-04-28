using UnityEngine;
using System.Collections.Generic;

public class MagicObject : MonoBehaviour
{
    private MagicBase mb;
    [Header("마법 설정")]
    public bool destroyOnHit = true; // 맞으면 사라지는가? (투사체용)

    // 피아 식별을 위한 레이어 마스크 (적과 플레이어 레이어 체크)
    public LayerMask targetLayers;

    private List<GameObject> hitTargets = new List<GameObject>();

    void Start()
    {
        mb = GetComponent<MagicBase>();
        Destroy(gameObject, mb.magicLifeTime);
    }

    void Update()
    {
        // 투사체라면 앞으로 전진 (장판이면 이 코드를 빼면 됨)
        // transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }



    void OnTriggerEnter(Collider other)
    {
        // 레이어 체크 용도
        //Debug.Log($"충돌 감지됨: {other.gameObject.name} (Layer: {other.gameObject.layer})");

        // 1. [가장 먼저 체크] 아직 발사 상태가 아니라면 아예 아무것도 하지 마라!
        if (mb == null || !mb.isLaunched) return;

        // 2. 여기서부터는 "발사된 후"이므로 누구든 맞으면 터짐
        // 레이어 체크 (이미지 설정대로 Player, Enemy 등 감지)
        if (((1 << other.gameObject.layer) & targetLayers) == 0) return;

        GameObject rootObj = other.transform.root.gameObject;

        // 3. 중복 타격 방지 및 처리
        if (hitTargets.Contains(rootObj)) return;

        ProcessHit(rootObj);
        if (destroyOnHit) Destroy(gameObject);
    }

    private void ProcessHit(GameObject target)
    {
        hitTargets.Add(target);

        // [적 타격]
        if (target.TryGetComponent(out TestEnemyController enemy))
        {
            enemy.TakeDamage(mb.magicDamage, 0.1f);
            ApplyHitEffects(false); // 마법은 일반 타격 연출
            Debug.Log($"<color=purple>적이 마법에 맞음!</color>");
        }
        // [플레이어 타격]
        else if (target.TryGetComponent(out PlayerController player))
        {
            // player.TakeDamage(damage); // 플레이어용 대미지 함수 호출
            Debug.Log($"<color=red> 플레이어가 마법에 맞음!</color>");
            ApplyHitEffects(false);
        }
    }

    private void ApplyHitEffects(bool isPerfect)
    {
        if (HitEffectManager.Instance != null)
        {
            HitEffectManager.Instance.HitStop(0.02f);
            HitEffectManager.Instance.CameraShake(0.2f, 0.1f);
        }
    }

    // 분열될 때 호출해줄 초기화 함수
    public void ResetMagic()
    {
        hitTargets.Clear(); // 이전 타격 기록 삭제
    }
}