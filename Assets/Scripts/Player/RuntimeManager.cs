using UnityEngine;

/// <summary>
/// 게임 런타임 데이터 관리
/// - PlayerStats (현재 스탯)
/// - PlayerInventory (현재 인벤토리)
/// - 씬 전환 시에도 유지
/// </summary>
public class RuntimeManager : MonoBehaviour
{
    // ========================================
    // 싱글톤
    // ========================================

    public static RuntimeManager Instance { get; private set; }

    // ========================================
    // 런타임 데이터
    // ========================================

    [Header("플레이어 데이터")]
    public PlayerStats playerStats;

    [Header("재화")]
    public int gold = 0;

    // ========================================
    // 초기화
    // ========================================

    void Awake()
    {
        // 싱글톤 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("[RuntimeManager] 생성 완료");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("[RuntimeManager] 중복 제거");
        }
    }

    /// <summary>
    /// 런타임 데이터 초기화 (게임 시작 시 호출)
    /// </summary>
    public void Initialize(PlayerData playerData)
    {
        // ⭐ null 체크 추가
        if (playerData == null)
        {
            Debug.LogError("[RuntimeManager] playerData가 null입니다!");
            return;
        }

        Debug.Log($"[RuntimeManager] PlayerData 확인 - totalLevel: {playerData.total_Level}, base_Health: {playerData.base_Health_Level}");

        // PlayerStats 생성 (초기값 복사)
        playerStats = new PlayerStats(playerData);

        // 초기 골드
        gold = 0;

        Debug.Log($"[RuntimeManager] 초기화 완료 - 레벨: {playerStats.level}, HP: {playerStats.max_Health}");
    }

    // ========================================
    // 디버그 (테스트용)
    // ========================================

    void Update()
    {
        // G 키: 골드 추가 테스트
        if (Input.GetKeyDown(KeyCode.G))
        {
            gold += 100;
            Debug.Log($"[골드] +100 → 총 {gold}G");
        }

        // L 키: 스탯 출력 테스트
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (playerStats != null)
            {
                playerStats.PrintStats();
            }
            else
            {
                Debug.Log("[RuntimeManager] playerStats가 null입니다!");
            }
        }
    }
}