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
}