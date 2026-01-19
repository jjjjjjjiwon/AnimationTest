using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 전역 데이터 관리 싱글톤
/// 씬 전환되어도 유지됨 (DontDestroyOnLoad)
/// </summary>
public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    [Header("Current Session")]
    public int currentSeed;
    public int currentFloor = 1;

    [Header("Card Management")]
    public List<StageData> selectedStages = new List<StageData>();
    public List<StageData> clearedStages = new List<StageData>();

    [Header("Loaded Stage Data")]
    public List<StageData> allStageData = new List<StageData>();  // ← 추가: JSON에서 로드된 전체 스테이지
    public Dictionary<int, ItemData> itemDatabase = new Dictionary<int, ItemData>(); // ← 추가!

    [Header("보스 강화")]
    public Dictionary<string, List<BossUpgrade>> bossUpgradesDB;


    void Awake()
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

    void InitializeSeed()
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
    // 아이템 조회 (추가!)
    // ========================================

    /// <summary>ID로 아이템 데이터 가져오기</summary>
    public ItemData GetItem(int itemID)
    {
        if (itemDatabase.TryGetValue(itemID, out ItemData item))
            return item;

        Debug.LogWarning($"[GameData] 아이템 ID {itemID}를 찾을 수 없습니다!");
        return null;
    }


    // ========================================
    // 보스 강화 조회
    // ========================================

    /// <summary>보스 이름으로 강화 목록 가져오기</summary>
    public List<BossUpgrade> GetUpgradesForBoss(string bossName)
    {
        if (bossUpgradesDB == null)
        {
            Debug.LogWarning("[GameData] bossUpgradesDB가 null입니다!");
            return new List<BossUpgrade>();
        }

        if (bossUpgradesDB.ContainsKey(bossName))
        {
            return bossUpgradesDB[bossName];
        }
        
        Debug.Log($"boss name : {bossName}");
        Debug.LogWarning($"[GameData] 보스 '{bossName}'의 강화 데이터가 없습니다!");
        return new List<BossUpgrade>();
    }

    /// <summary>층 번호로 스테이지 찾기</summary>
public StageData GetStageByFloor(int floor)
{

    if (allStageData == null)
    {
        Debug.LogWarning("[GameData] allStageData가 null입니다!");
        return null;
    }
    
    Debug.Log($"[GameData] 스테이지 검색: floor={floor}, 총 {allStageData.Count}개");
    
    foreach (var stage in allStageData)
    {
        Debug.Log($"  - stageID: {stage.stageID}, name: {stage.stageName}");
    }
    
    var result = allStageData.Find(s => s.stageID == floor);
    
    if (result == null)
    {
        Debug.LogWarning($"[GameData] stageID {floor}를 찾을 수 없습니다!");
    }
    
    return result;
}


}