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
    public PlayerData playerData;
    public SocketManager socketManager;
    private bool isInitialized = false; 

    

    [Header("아이템 데이터베이스")]
    public Dictionary<int, ItemData> itemDatabase;

    [Header("재화")]
    public int gold = 0;

    [Header("진행 상태")]
    public int currentFloor = 1;                    // 현재 층
    public string currentBossName;                  // 현재 보스 이름

    public void SetCurrentBossName(string bossName)
{
    currentBossName = bossName;
}
public string GetCurrentBossName() => currentBossName;

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

    void Start()
{
    // ⭐ 이미 초기화되었으면 스킵
    if (isInitialized)
    {
        Debug.Log("[RuntimeManager] 이미 초기화됨");
        return;
    }
    
    // ⭐ GameData에서 PlayerData 가져오기
    if (GameData.Instance == null)
    {
        Debug.LogError("[RuntimeManager] GameData.Instance가 없습니다!");
        return;
    }
    
    PlayerData defaultData = GameData.Instance.defaultPlayerData;
    
    if (defaultData == null)
    {
        Debug.LogError("[RuntimeManager] GameData.defaultPlayerData가 없습니다!");
        return;
    }
    
    // ⭐ 자동으로 초기화!
    Initialize(defaultData);
}

   public void Initialize(PlayerData playerData)
{
    if (playerData == null)
    {
        Debug.LogError("[RuntimeManager] playerData가 null입니다!");
        return;
    }
    
    // ⭐ 이미 초기화되었으면 스킵
    if (isInitialized)
    {
        Debug.Log("[RuntimeManager] 이미 초기화됨 - 스킵");
        return;
    }

    this.playerData = playerData;
    playerStats = new PlayerStats(playerData);
    playerInventory = new PlayerInventory();
    socketManager = new SocketManager(playerData);
    gold = playerData.startingGold;
    
    isInitialized = true;  // ← 플래그 설정

    Debug.Log($"[RuntimeManager] 초기화 완료 - 이름: {playerData.playerName}, 소켓: {socketManager.GetSocketCount()}개");
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
        string bossName = GetCurrentBossName();
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

        // ⭐ Fisher-Yates Shuffle (제대로 된 랜덤)
        for (int i = available.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = available[i];
            available[i] = available[j];
            available[j] = temp;
        }

        return available.Take(3).ToList();
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

/// <summary>
/// 보상 지급 및 UI 표시
/// StageManager.OnStageCleared()에서 호출됨
/// </summary>
public void GiveReward(StageData stage)
{
    if (stage == null)
    {
        Debug.LogError("[RuntimeManager] StageData가 null입니다!");
        return;
    }

    Debug.Log($"[RuntimeManager] 보상 지급 - {stage.stageName}");

    // UIManager를 통해 보상 UI 표시
    if (UIManager.Instance != null)
    {
        UIManager.Instance.ShowRewardUI(stage, stage.isBossStage);
    }
    else
    {
        Debug.LogError("[RuntimeManager] UIManager.Instance가 null입니다!");
    }
}

    // 원래 private
    public void TestBossUpgrade()
    {
        Debug.Log("=== 보스 강화 테스트 ===");

        // ⭐ PrepareFloor() 제거 (리셋하지 않음)
        // PrepareFloor(1);

        // ⭐ 보스 이름이 없으면 초기화
        if (string.IsNullOrEmpty(currentBossName))
        {
            currentBossName = "Flame Titan";
            currentFloor = 1;
            selectedBossUpgrades.Clear();
            Debug.Log("[테스트] 초기화 완료");
        }

        Debug.Log($"[테스트] 현재 층: {currentFloor}, 보스: {currentBossName}");

        // 사용 가능한 강화 목록
        var available = GetAvailableUpgrades();
        Debug.Log($"사용 가능한 강화: {available.Count}개");

        if (available.Count == 0)
        {
            Debug.Log("더 이상 선택할 강화가 없습니다!");
            return;
        }

        // 랜덤 3개
        var random3 = GetRandomThreeUpgrades();
        Debug.Log($"랜덤 3개:");
        foreach (var upgrade in random3)
        {
            Debug.Log($"  - {upgrade.upgradeName}: {upgrade.upgradeDescription}");
        }

        // 하나 선택
        if (random3.Count > 0)
        {
            SelectUpgrade(random3[0].upgradeID);

            // 선택된 강화 확인
            var selected = GetSelectedUpgrades();
            Debug.Log($"선택된 강화: {selected.Count}개");
            foreach (var s in selected)
            {
                Debug.Log($"  - {s.upgradeName}");
            }
        }
    }


}