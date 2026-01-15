using UnityEngine;
using System.Collections.Generic;

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
    public PlayerInventory playerInventory;

    [Header("아이템 데이터베이스")]
    public Dictionary<int, ItemData> itemDatabase;

    [Header("재화")]
    public int gold = 0;

    // ========================================
    // 초기화
    // ========================================

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            playerStats = null;
            gold = 0;

            // 초기화

            Debug.Log("[RuntimeManager] 생성 완료");
        }
        else
        {
            Destroy(gameObject);
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

        // PlayerStats 생성 (초기값 복사)
        playerStats = new PlayerStats(playerData);

        // PlayerInventory 생성 (추가!)
        playerInventory = new PlayerInventory();

        // 초기 골드
        gold = 0;

        Debug.Log($"[RuntimeManager] 초기화 완료 - 레벨: {playerStats.level}, HP: {playerStats.max_Health}");
    }

    // ========================================
    // 아이템 조회 (추가!)
    // ========================================

    /// <summary>ID로 아이템 데이터 가져오기</summary>
    public ItemData GetItem(int itemID)
    {
        if (itemDatabase.TryGetValue(itemID, out ItemData item))
            return item;

        Debug.LogWarning($"[RuntimeManager] 아이템 ID {itemID}를 찾을 수 없습니다!");
        return null;
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

        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (playerInventory != null)
            {
                playerInventory.AddItem(1, 1);
            }
        }

        // 2 키: 아이템 ID 2 추가
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (playerInventory != null)
            {
                playerInventory.AddItem(2, 1);
            }
        }
    }
}