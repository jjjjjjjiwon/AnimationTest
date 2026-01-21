using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private Transform spawnPoint;

    private void Start()
    {
        if (RuntimeManager.Instance != null && string.IsNullOrEmpty(RuntimeManager.Instance.GetCurrentBossName()))
    RuntimeManager.Instance.SetCurrentBossName("Flame Titan");

        if (RuntimeManager.Instance == null)
        {
            Debug.LogError("[BossSpawner] RuntimeManager.Instance null");
            return;
        }

        if (GameData.Instance == null)
        {
            Debug.LogError("[BossSpawner] GameData.Instance null");
            return;
        }

        string bossName = RuntimeManager.Instance.GetCurrentBoss();
        if (string.IsNullOrEmpty(bossName))
        {
            Debug.LogError("[BossSpawner] currentBossName empty (RuntimeManager.SetCurrentBoss를 먼저 해야 함)");
            return;
        }

        BossDefinition def = GameData.Instance.GetBossDefinitionByName(bossName);
        if (def == null)
        {
            Debug.LogError($"[BossSpawner] BossDefinition not found: '{bossName}'");
            return;
        }

        if (def.prefab == null)
        {
            Debug.LogError($"[BossSpawner] BossDefinition.prefab null: '{bossName}'");
            return;
        }

        var upgrades = RuntimeManager.Instance.GetBossUpgradesCopy();
        var runtime = new BossRuntime(def, upgrades);

        Vector3 pos = spawnPoint != null ? spawnPoint.position : Vector3.zero;
        Quaternion rot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        GameObject bossObj = Instantiate(def.prefab, pos, rot);

        EnemyController enemy = bossObj.GetComponent<EnemyController>();
        if (enemy == null)
        {
            Debug.LogError("[BossSpawner] spawned prefab에 EnemyController가 없음");
            return;
        }

        enemy.ApplyBossRuntime(runtime);

        Debug.Log($"[BossSpawner] Spawn OK boss={runtime.BossName} hp={runtime.MaxHp} dmg={runtime.Damage} ups={runtime.upgrades.Count}");
    }
}
