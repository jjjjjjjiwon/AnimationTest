using System.Collections.Generic;
using UnityEngine;

using System.Linq; // 현재는 없어도 되지만, 너가 다른 곳에서 쓰면 유지해도 됨

/// <summary>
/// 게임 전역 데이터 관리 싱글톤
/// 씬 전환되어도 유지됨 (DontDestroyOnLoad)
/// </summary>
public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    [Header("Player Data")]
    public PlayerData defaultPlayerData;

    [Header("Current Session")]
    public int currentSeed;
    public int currentFloor = 1;

    [Header("Card Management")]
    public List<StageData> selectedStages = new List<StageData>();
    public List<StageData> clearedStages = new List<StageData>();

    [Header("Loaded Stage Data")]
    public List<StageData> allStageData = new List<StageData>();
    public Dictionary<int, ItemData> itemDatabase = new Dictionary<int, ItemData>();

    // =========================
    // Boss JSON 데이터 저장 (새 설계)
    // =========================
    private List<BossJsonData> bossDefs = new List<BossJsonData>();
    private List<BossUpgradeJsonData> bossUpgrades = new List<BossUpgradeJsonData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSeed();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSeed()
    {
        currentSeed = Random.Range(int.MinValue, int.MaxValue);
        Debug.Log($"[GameData] 시드 생성: {currentSeed}");
    }

    public void NextFloor()
    {
        currentFloor++;
        Debug.Log($"[GameData] 다음 층: {currentFloor}층");
    }

    public void MoveToGraveyard(int index)
    {
        if (index < 0 || index >= selectedStages.Count)
        {
            Debug.LogError($"[GameData] 잘못된 인덱스: {index}");
            return;
        }

        StageData selected = selectedStages[index];

        if (selected != null)
        {
            clearedStages.Add(selected);
            selectedStages[index] = null;
            Debug.Log($"[GameData] '{selected.stageName}' 묘지로 이동");
        }
        else
        {
            Debug.LogWarning($"[GameData] 인덱스 {index}는 이미 null입니다");
        }
    }

    public void StartNewRun()
    {
        currentFloor = 1;
        selectedStages.Clear();
        clearedStages.Clear();
        InitializeSeed();
        Debug.Log("[GameData] 새로운 런 시작!");
    }

    // ========================================
    // 아이템 조회
    // ========================================
    public ItemData GetItem(int itemID)
    {
        if (itemDatabase.TryGetValue(itemID, out ItemData item))
            return item;

        Debug.LogWarning($"[GameData] 아이템 ID {itemID}를 찾을 수 없습니다!");
        return null;
    }

    // ========================================
    // 스테이지 조회
    // ========================================
    public StageData GetStageByFloor(int floor)
    {
        if (allStageData == null)
        {
            Debug.LogWarning("[GameData] allStageData가 null입니다!");
            return null;
        }

        var result = allStageData.Find(s => s != null && s.stageID == floor);

        if (result == null)
            Debug.LogWarning($"[GameData] stageID {floor}를 찾을 수 없습니다!");

        return result;
    }

    // ========================================
    // Boss JSON 데이터 저장/조회 (새 설계)
    // ========================================

    /// <summary>BossDefinitionLoader가 호출: 보스 기본 JSON 리스트 저장</summary>
    public void SetBossJson(List<BossJsonData> defs)
    {
        bossDefs = defs ?? new List<BossJsonData>();
        Debug.Log($"[GameData] bossDefs 저장 완료: {bossDefs.Count}개");
    }

    /// <summary>BossUpgradeLoader가 호출: 보스 강화 JSON 리스트 저장</summary>
    public void SetBossUpgradeJson(List<BossUpgradeJsonData> ups)
    {
        bossUpgrades = ups ?? new List<BossUpgradeJsonData>();
        Debug.Log($"[GameData] bossUpgrades 저장 완료: {bossUpgrades.Count}개");
    }

    /// <summary>bossId로 보스 정의 찾기</summary>
    public BossJsonData GetBossById(string bossId)
    {
        if (string.IsNullOrEmpty(bossId))
            return null;

        if (bossDefs == null || bossDefs.Count == 0)
        {
            Debug.LogWarning("[GameData] bossDefs 비어있음 (BossDefinitionLoader 로드 확인)");
            return null;
        }

        return bossDefs.Find(b => b != null && b.bossId == bossId);
    }

    /// <summary>bossId로 해당 보스에 적용 가능한 강화 목록 가져오기</summary>
    public List<BossUpgradeJsonData> GetUpgradesForBoss(string bossId)
    {
        if (string.IsNullOrEmpty(bossId))
            return new List<BossUpgradeJsonData>();

        if (bossUpgrades == null || bossUpgrades.Count == 0)
        {
            Debug.LogWarning("[GameData] bossUpgrades 비어있음 (BossUpgradeLoader 로드 확인)");
            return new List<BossUpgradeJsonData>();
        }

        // targetBossId가 비어있으면 공용 강화로 취급
        return bossUpgrades.FindAll(u =>
            u != null && (string.IsNullOrEmpty(u.targetBossId) || u.targetBossId == bossId)
        );
    }
}
