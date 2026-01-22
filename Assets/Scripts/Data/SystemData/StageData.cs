using UnityEngine;

[CreateAssetMenu(fileName = "New Stage", menuName = "Game/Stage Data")]
public class StageData : ScriptableObject
{
    [Header("Stage Info")]
    public int stageID; // ID
    public string stageName; // 이름
    [TextArea(3, 5)]
    public string stageDescription; // 설명
    public string iconPath; // 아이콘
    public int difficulty;  // 난이도

    public string sceneName;    // 씬의 이름
    
    [Header("Boss Stage (only if isBossStage = true)")]
    public bool isBossStage;  // ✅ 보스 스테이지 여부
    [Tooltip("보스 스테이지일 때만 사용. 예: flame_titan")]
    public string bossId;     // ✅ 내부 식별자(고정키)
    [Tooltip("표시용 이름(옵션). 보스 스테이지일 때만 쓰는 걸 권장")]
    public string bossName;   // (선택) UI/로그용


    [Header("Rewards")]  // ← 새로 추가!
    public int goldReward;          // 돈
    public int levelUpPoint;        // 포인트
    public string[] itemRewards;    // 아이템
    public string skillReward;      // 스킬

    [Header("Clear Condition")]
    public string clearConditionType;
    public int targetKillCount;

    [System.NonSerialized] public ClearConditionType clearType;
    
    // 런타임에 로드될 Sprite (JSON에는 없음)
    [HideInInspector]
    public Sprite stageIcon;    // 런타임 아이콘
}

    public enum ClearConditionType
    {
        None,
        KillAll,      // 모든 적 처치
        KillTarget,   // 특정 수만큼 처치
        Destination, // 목적지 도착
        Boss          // 보스 처치
    }