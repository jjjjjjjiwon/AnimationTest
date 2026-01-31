using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [Header("Current Progress")]
    private StageData currentStage;
    private List<EnemySpawnInfo> pendingSpawns = new List<EnemySpawnInfo>(); // 소환 대기 명단
    
    private int killCount;
    private float stageTimer;
    private bool clearedOnce;
    private Portal portal;

    // ================= Unity Lifecycle =================

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        Debug.Log("[StageManager] Awake");
    }
    private void Update()
    {
        // 스테이지 진행 중이고, 아직 소환할 적이 남았다면 체크
        if (currentStage != null && !clearedOnce && pendingSpawns.Count > 0)
        {
            stageTimer += Time.deltaTime;
            CheckSpawnConditions();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 로드 시마다 포탈을 새로 찾음
        portal = Object.FindFirstObjectByType<Portal>(FindObjectsInactive.Include);

        if (portal == null)
            Debug.LogWarning("[StageManager] Portal not found in this scene");
        else
            Debug.Log("[StageManager] Portal cached");
    }

    // ================= Stage Control =================

    public void StartStage(StageData stage)
    {
        if (stage == null)
        {
            Debug.LogError("[StageManager] StartStage: stage null");
            return;
        }

        currentStage = stage;
        ResetStageProgress();
        
        // JSON에서 읽어온 문자열을 Enum으로 안전하게 변환 (원래 로직 유지)
        // 만약 JSON 로더에서 이미 처리했다면 생략 가능
        ParseClearCondition(stage);

        // ✅ 소환 리스트 복사 (원본 데이터 보호)
        pendingSpawns = new List<EnemySpawnInfo>(stage.enemySpawnList);

        Debug.Log($"[StageManager] StartStage: {stage.stageName} / ClearType: {stage.clearType}");

        // 씬 전환
        SceneManager.LoadScene(stage.sceneName);
    }

    private void ResetStageProgress()
    {
        killCount = 0;
        stageTimer = 0f;
        clearedOnce = false;
    }

    private void ParseClearCondition(StageData stage)
    {
        // 이 부분은 이미 JSON 로더에서 Enum으로 잘 들어오고 있다면 없어도 되지만,
        // 예외 처리를 위해 남겨두는 것도 좋습니다.
        if (stage.clearType == ClearConditionType.None) return;
        
        Debug.Log($"[StageManager] Clear Condition: {stage.clearType} (Target: {stage.targetKillCount})");
    }

    // ================= Spawning Logic =================

    private void CheckSpawnConditions()
    {
        // 리스트를 뒤에서부터 순회하며 조건 체크 (소환 후 제거를 위해)
        for (int i = pendingSpawns.Count - 1; i >= 0; i--)
        {
            if (IsConditionMet(pendingSpawns[i]))
            {
                SpawnEnemy(pendingSpawns[i]);
                pendingSpawns.RemoveAt(i); // 소환 성공 시 목록에서 삭제
            }
        }
    }

    private bool IsConditionMet(EnemySpawnInfo info)
    {
        switch (info.conditionType)
        {
            case SpawnConditionType.None:
                return true;
            case SpawnConditionType.TimeElapsed:
                return stageTimer >= info.conditionValue;

            case SpawnConditionType.KillsReached:
                return killCount >= (int)info.conditionValue;

            default:
                return false;
        }
    }

    private void SpawnEnemy(EnemySpawnInfo info)
    {
// 1. info 자체가 null인지 확인
    if (info == null) {
        Debug.LogError("SpawnInfo 객체가 null입니다.");
        return;
    }

    // 2. ID값이 비어있는지 확인 (여기가 null이면 에러 발생)
    if (string.IsNullOrEmpty(info.enemy_ID)) {
        Debug.LogError("JSON에서 enemy_Id를 읽어오지 못했습니다! 변수명을 확인하세요.");
        return;
    }

    Debug.Log($"[StageManager] 소환 시도 ID: {info.enemy_ID}");
    EnemyFactory.Instance.SpawnEnemy(info.enemy_ID, info.spawnPos, info.spawnRotation);
    }

    // ================= Clear Events =================

    public void NotifyEnemyKilled()
    {
        if (currentStage == null || clearedOnce) return;

        killCount++;

        // 킬 수 달성 클리어 조건 체크
        if (currentStage.clearType == ClearConditionType.KillTarget)
        {
            if (killCount >= currentStage.targetKillCount)
                OnStageCleared();
        }
        
        // 모든 적 처치(KillAll)의 경우, 리스트가 비었고 현재 씬에 적이 0명인지 체크하는 로직 필요
    }

    public void NotifyBossKilled()
    {
        if (currentStage == null || clearedOnce) return;

        if (currentStage.clearType == ClearConditionType.Boss)
            OnStageCleared();
    }

    private void OnStageCleared()
    {
        if (clearedOnce) return;
        clearedOnce = true;

        Debug.Log($"[StageManager] Stage Cleared: {currentStage.stageName}");

        // 포탈 활성화 (원래 로직 유지)
        if (portal != null)
            portal.Activate();
    }

    // ================= Accessor =================

    public StageData GetCurrentStage() => currentStage;
    public int GetKillCount() => killCount;
}