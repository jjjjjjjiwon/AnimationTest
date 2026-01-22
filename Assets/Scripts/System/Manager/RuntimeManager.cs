using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RuntimeManager : MonoBehaviour
{
    public static RuntimeManager Instance { get; private set; }

    // ================= Player =================
    public PlayerStats playerStats;
    public PlayerInventory playerInventory;
    public PlayerData playerData;
    public SocketManager socketManager;

    private bool isInitialized = false;

    // ================= Boss =================
    [Header("Boss Runtime")]
    private readonly List<BossUpgradeJsonData> selectedBossUpgrades = new(); // 강화 선택
    public List<BossUpgradeJsonData> SelectedBossUpgrades => selectedBossUpgrades;
    [SerializeField] private string currentBossId;   // ✅ bossId ONLY
    public string CurrentBossId => currentBossId;


    // ================= Other =================
    public int gold = 0;
    public int currentFloor = 1;

    // ================= Init =================
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("[RuntimeManager] 생성 완료");
    }

    void Start()
    {
        if (isInitialized) return;

        if (GameData.Instance == null || GameData.Instance.defaultPlayerData == null)
        {
            Debug.LogError("[RuntimeManager] GameData / PlayerData 없음");
            return;
        }

        Initialize(GameData.Instance.defaultPlayerData);
    }

    public void Initialize(PlayerData data)
    {
        if (isInitialized) return;

        playerData = data;
        playerStats = new PlayerStats(data);
        playerInventory = new PlayerInventory();
        socketManager = new SocketManager(data);
        gold = data.startingGold;

        isInitialized = true;
        Debug.Log("[RuntimeManager] 초기화 완료");
    }

    // ================= Boss Control =================

    /// <summary>StageManager에서만 호출</summary>
    public void SetCurrentBossId(string bossId)
    {
        currentBossId = bossId;
        selectedBossUpgrades.Clear();

        Debug.Log($"[RuntimeManager] currentBossId set = '{bossId}'");
    }

    public void AddBossUpgrade(BossUpgradeJsonData up)
    {
        if (up == null) return;
        selectedBossUpgrades.Add(up);

        Debug.Log($"[RuntimeManager] AddBossUpgrade: {up.upgradeID}");
    }

    public List<BossUpgradeJsonData> GetAvailableUpgrades()
    {
        if (string.IsNullOrEmpty(currentBossId))
        {
            Debug.LogWarning("[RuntimeManager] currentBossId 비어있음");
            return new();
        }

        var all = GameData.Instance.GetUpgradesForBoss(currentBossId);
        if (all == null) return new();

        var selectedIds = selectedBossUpgrades.Select(u => u.upgradeID).ToHashSet();

        return all.Where(u => !selectedIds.Contains(u.upgradeID)).ToList();
    }

    public List<BossUpgradeJsonData> GetRandomThreeUpgrades()
    {
        var list = GetAvailableUpgrades();


        if (list.Count <= 3) return list;

        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }

        return list.Take(3).ToList();
    }

    public string GetCurrentBossId()
{
    return currentBossId;
}


}
