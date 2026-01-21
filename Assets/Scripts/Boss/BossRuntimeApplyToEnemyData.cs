using UnityEngine;
using System.Collections.Generic;

public class BossRuntimeApplyToEnemyData : MonoBehaviour
{
    [Header("Boss EnemyController")]
    [SerializeField] private EnemyController enemy; // 보스에 붙은 EnemyController

    [Header("Optional: Apply on Awake instead of Start")]
    [SerializeField] private bool applyOnAwake = false;

    private void Awake()
    {
        if (enemy == null)
            enemy = GetComponent<EnemyController>();

        if (applyOnAwake)
            Apply();
    }

    private void Start()
    {
        if (!applyOnAwake)
            Apply();
    }

    private void Apply()
    {
        if (RuntimeManager.Instance == null)
        {
            Debug.LogError("[BossApply] RuntimeManager.Instance null");
            return;
        }

        BossRuntime runtime = RuntimeManager.Instance.GetCurrentBossRuntime();
        if (runtime == null)
        {
            Debug.LogError("[BossApply] currentBossRuntime null");
            return;
        }

        if (enemy == null)
        {
            Debug.LogError("[BossApply] EnemyController null (보스 오브젝트에 EnemyController가 있어야 함)");
            return;
        }

        EnemyData baseData = enemy.Data;
        if (baseData == null)
        {
            Debug.LogError("[BossApply] enemy.Data null");
            return;
        }

        // 1) EnemyData 런타임 복사본 생성
        EnemyData runtimeData = Instantiate(baseData);

        // 2) BossRuntime의 기본 스탯 반영
        // ⚠️ 아래 필드명은 네 EnemyData에 맞춰야 함.
        // 일단 가장 흔한 형태로 작성해둠:
        // runtimeData.baseHealth = runtime.MaxHp;
        // runtimeData.baseDamage = runtime.Damage;

        // ---- 임시 로그 ----
        Debug.Log($"[BossApply] Apply runtime to EnemyData: boss={runtime.BossName}, upgrades={runtime.upgrades.Count}");

        // 3) 강화 목록 반영
        ApplyUpgrades(runtime, runtimeData);

        // 4) EnemyController에 적용
        // ⚠️ EnemyController의 Data 프로퍼티가 읽기 전용이면, enemy에 SetData 메서드를 추가해야 함.
        // 현재 EnemyController는 "public EnemyData Data => data;"라서 직접 교체 불가.
        // 따라서 EnemyController에 SetEnemyData(EnemyData newData) 메서드를 만들어야 한다.
        enemy.SetEnemyData(runtimeData);

        Debug.Log("[BossApply] 완료: EnemyData 런타임 적용");
    }

    private void ApplyUpgrades(BossRuntime runtime, EnemyData runtimeData)
    {
        if (runtime.upgrades == null) return;

        foreach (var up in runtime.upgrades)
        {
            if (up == null) continue;

            // statType 기준 예시: health/damage/speed
            if (up.upgradeType == "stat")
            {
                switch (up.statType)
                {
                    case "health":
                        // runtimeData.baseHealth *= (1f + up.value);
                        break;

                    case "damage":
                        // runtimeData.baseDamage *= (1f + up.value);
                        break;

                    case "speed":
                        // runtimeData.moveSpeed *= (1f + up.value);
                        break;
                }
            }
        }
    }
}
