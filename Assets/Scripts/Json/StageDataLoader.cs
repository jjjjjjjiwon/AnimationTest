using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// JSON에서 스테이지 데이터를 읽어 ScriptableObject로 변환
/// 게임 시작 시 자동 로드
/// </summary>
public class StageDataLoader : MonoBehaviour
{



    void Start()
    {
        LoadStagesFromJSON();
    }

    /// <summary>
    /// JSON 파일을 읽어서 StageData들을 생성
    /// </summary>
    void LoadStagesFromJSON()
    {
        // 1. JSON 파일 읽기
        TextAsset jsonFile = Resources.Load<TextAsset>("Json/stages");
        

        if (jsonFile == null)
        {
            Debug.LogError("[StageDataLoader] stages.json 파일을 찾을 수 없습니다!");
            return;
        }

        // ⭐ JSON 내용 출력
        Debug.Log($"[StageDataLoader] JSON 내용:\n{jsonFile.text}");

        StageDataList dataList = JsonUtility.FromJson<StageDataList>(jsonFile.text);

        if (dataList == null || dataList.stages == null)
        {
            Debug.LogError("[StageDataLoader] JSON 파싱 실패!");
            return;
        }

        // 3. ScriptableObject 생성 및 저장
        List<StageData> loadedStages = new List<StageData>();

        foreach (StageJsonData jsonData in dataList.stages)
        {
            // ScriptableObject 생성
            StageData stageData = ScriptableObject.CreateInstance<StageData>();

            // 기본 데이터 복사
            stageData.stageID = jsonData.stageID;
            stageData.stageName = jsonData.stageName;
            stageData.stageDescription = jsonData.stageDescription;
            stageData.iconPath = jsonData.iconPath;
            stageData.difficulty = jsonData.difficulty;
            stageData.sceneName = jsonData.sceneName;

            // 보상 데이터 복사
            stageData.goldReward = jsonData.goldReward;
            stageData.levelUpPoint = jsonData.levelUpPoint;
            stageData.itemRewards = jsonData.itemRewards;
            stageData.skillReward = jsonData.skillReward;

            stageData.clearConditionType = jsonData.clearConditionType;
            stageData.targetKillCount = jsonData.targetKillCount;


            // ⭐ 이 3줄 추가!
            stageData.isBossStage = jsonData.isBossStage;
            stageData.bossName = jsonData.bossName;

            // Sprite 로드
            stageData.stageIcon = Resources.Load<Sprite>(jsonData.iconPath);

            if (stageData.stageIcon == null)
            {
                Debug.LogWarning($"[StageDataLoader] '{jsonData.iconPath}' 이미지를 찾을 수 없습니다!");
            }
            Debug.Log($"[StageLoad] {stageData.stageName} clear='{stageData.clearConditionType}' target={stageData.targetKillCount}");

            loadedStages.Add(stageData);

            Debug.Log($"[StageDataLoader] {stageData.stageName} 로드 - " +
                      $"Difficulty: {stageData.difficulty}, " +
                      $"Reward: {stageData.goldReward}G / {stageData.levelUpPoint}p");
        }

        // 4. GameData에 저장
        if (GameData.Instance != null)
        {
            GameData.Instance.allStageData = loadedStages;
            Debug.Log($"[StageDataLoader] {loadedStages.Count}개 스테이지 로드 완료!");
        }
        else
        {
            Debug.LogError("[StageDataLoader] GameData.Instance가 null입니다!");
        }
    }
}

/// <summary>
/// JSON 최상위 구조 (stages 배열 포함)
/// </summary>
[System.Serializable]
public class StageDataList
{
    public List<StageJsonData> stages;
}

/// <summary>
/// JSON의 개별 스테이지 데이터 구조
/// </summary>
[System.Serializable]
public class StageJsonData
{
    public int stageID;
    public string stageName;
    public string stageDescription;
    public string iconPath;
    public int difficulty;
    public string sceneName;

    // 보상 정보
    public int goldReward;
    public int levelUpPoint;
    public string[] itemRewards;
    public string skillReward;


    public string clearConditionType;
    public int targetKillCount;

    // ⭐ 추가
    public bool isBossStage;
    public string bossName;
}