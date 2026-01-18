using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

    [Header("진행 상태")]
    public int currentFloor = 1;                    // 현재 층
    public string currentBossName;                  // 현재 보스 이름

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
        if (playerData == null)
        {
            Debug.LogError("[RuntimeManager] playerData가 null입니다!");
            return;
        }

        // PlayerStats 생성
        playerStats = new PlayerStats(playerData);

        // PlayerInventory 생성
        playerInventory = new PlayerInventory();

        // 초기 골드 설정 ⭐
        gold = playerData.startingGold;

        Debug.Log($"[RuntimeManager] 초기화 완료 - 레벨: {playerStats.level}, 골드: {gold}G");
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




    [Header("보스 강화")]
    public List<string> selectedBossUpgrades = new List<string>();  // 선택된 강화 ID들

    // ========================================
    // 층 관리
    // ========================================

    /// <summary>층 준비 (로비 진입 시 호출)</summary>
    public void PrepareFloor(int floor)
    {
        currentFloor = floor;

        // 해당 층의 스테이지 정보 가져오기
        StageData stage = GameData.Instance.GetStageByFloor(floor);

        if (stage == null)
        {
            Debug.LogError($"[RuntimeManager] {floor}층 스테이지 없음!");
            return;
        }

        // 보스 이름 설정
        currentBossName = stage.bossName;

        // 강화 리셋
        selectedBossUpgrades.Clear();

        Debug.Log($"[RuntimeManager] {floor}층 준비 - 보스: {currentBossName}");
    }

    /// <summary>다음 층으로 이동</summary>
    public void MoveToNextFloor()
    {
        PrepareFloor(currentFloor + 1);
    }

    // ========================================
    // 보스 강화
    // ========================================

    /// <summary>사용 가능한 강화 목록 (선택 안 한 것들)</summary>
    public List<BossUpgrade> GetAvailableUpgrades()
    {
        var allUpgrades = GameData.Instance.GetUpgradesForBoss(currentBossName);

        return allUpgrades
            .Where(u => !selectedBossUpgrades.Contains(u.upgradeID))
            .ToList();
    }

    /// <summary>랜덤 3개 선택</summary>
    public List<BossUpgrade> GetRandomThreeUpgrades()
    {
        var available = GetAvailableUpgrades();

        if (available.Count <= 3)
            return available;

        // 섞고 3개 선택
        return available.OrderBy(x => Random.value).Take(3).ToList();
    }

    /// <summary>강화 선택</summary>
    public void SelectUpgrade(string upgradeID)
    {
        selectedBossUpgrades.Add(upgradeID);
        Debug.Log($"[강화 선택] {upgradeID} - 총 {selectedBossUpgrades.Count}개");
    }

    /// <summary>선택된 강화 목록 가져오기 (보스 생성 시)</summary>
    public List<BossUpgrade> GetSelectedUpgrades()
    {
        var allUpgrades = GameData.Instance.GetUpgradesForBoss(currentBossName);

        return allUpgrades
            .Where(u => selectedBossUpgrades.Contains(u.upgradeID))
            .ToList();
    }


    // ========================================
    // 보상
    // ========================================

    /// <summary>보상 지급 (골드, 스탯 포인트)</summary>
    public void GiveReward(StageData stage)
    {
        if (stage == null)
        {
            Debug.LogError("[RuntimeManager] StageData가 null입니다!");
            return;
        }

        // 골드 지급
        if (stage.goldReward > 0)
        {
            gold += stage.goldReward;
            Debug.Log($"[보상] 골드 +{stage.goldReward} → 총 {gold}G");
        }

        // 스탯 포인트 지급
        if (stage.levelUpPoint > 0)
        {
            playerStats.availablePoints += stage.levelUpPoint;
            Debug.Log($"[보상] 스탯 포인트 +{stage.levelUpPoint} → 총 {playerStats.availablePoints}p");
        }

        // UI 표시
        RewardUI.Show(stage, stage.isBossStage);
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