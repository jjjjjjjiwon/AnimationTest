using UnityEngine;

public class BossRuntimeBinder : MonoBehaviour
{
    [SerializeField] private EnemyController enemy; // 보스 컨트롤러

    private void Awake()
    {
        if (enemy == null) enemy = GetComponent<EnemyController>();
    }

    private void Start()
    {
        if (RuntimeManager.Instance == null)
        {
            Debug.LogError("[BossRuntimeBinder] RuntimeManager.Instance null");
            return;
        }

        BossRuntime rt = RuntimeManager.Instance.GetCurrentBossRuntime();
        if (rt == null)
        {
            Debug.LogError("[BossRuntimeBinder] currentBossRuntime null");
            return;
        }

        Apply(rt);
    }

    private void Apply(BossRuntime rt)
    {
        if (enemy == null || enemy.Data == null)
        {
            Debug.LogError("[BossRuntimeBinder] EnemyController/Data null");
            return;
        }

        // EnemyData에 반영 (EnemyController가 data.baseHealth를 쓰는 구조라 여기 바꾸는 게 제일 단순)
        enemy.Data.baseHealth = rt.MaxHp;
        // enemy.Data.baseDamage 같은 필드가 있으면 같이 반영
        // enemy.Data.moveSpeed 같은 필드가 있으면 같이 반영

        Debug.Log($"[BossRuntimeBinder] 적용 완료: HP={rt.MaxHp}, DMG={rt.Damage}, SPD={rt.MoveSpeed}");
    }
}
