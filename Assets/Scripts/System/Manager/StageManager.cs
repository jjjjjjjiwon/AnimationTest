using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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

    private int activeEnemyCount; // 현재 씬에 살아있는 적의 수

    public StageData GetCurrentStage() => currentStage;
    public int GetKillCount() => killCount;


#region 1. 시스템 초기화 및 관리

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

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 로드 시마다 포탈을 새로 찾음
        portal = Object.FindFirstObjectByType<Portal>(FindObjectsInactive.Include);

        if (portal == null)
            Debug.LogWarning("[StageManager] Portal not found in this scene");
        else
            Debug.Log("[StageManager] Portal cached");
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion

    #region 2. 스테이지 시작 및 흐름 컨트롤

    private void Update()
    {
        // 스테이지 진행 중이고, 아직 소환할 적이 남았다면 체크
        if (currentStage != null && !clearedOnce && pendingSpawns.Count > 0)
        {
            stageTimer += Time.deltaTime;
            CheckSpawnConditions();
        }
    }

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
        pendingSpawns = new List<EnemySpawnInfo>(stage.enemy_SpawnList);

        Debug.Log($"[StageManager] StartStage: {stage.stage_Name} / ClearType: {stage.clear_Type}");

        // 씬 전환
        SceneManager.LoadScene(stage.scene_Name);
    }

    private void ResetStageProgress()
    {
        killCount = 0;
        stageTimer = 0f;
        clearedOnce = false;
    }

#endregion

#region 3. 적 소환 시스템

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
        switch (info.condition_Type)
        {
            case SpawnConditionType.None:
                return true;
            case SpawnConditionType.TimeElapsed:
                return stageTimer >= info.condition_Value;

            case SpawnConditionType.KillsReached:
                return killCount >= (int)info.condition_Value;

            default:
                return false;
        }
    }

    private void SpawnEnemy(EnemySpawnInfo info)
    {
        if (info == null) return;

        string targetID = string.Empty;

        // 1. ID 결정 로직
        if (info.isBoss)
        {
            // currentStage 혹은 currentStageData (변수명 확인 필요)에서 보스 ID 가져오기
            if (currentStage != null)
            {
                targetID = currentStage.boss_ID;
            }
        }
        else
        {
            // 일반 적 ID 사용
            targetID = info.enemy_ID;
        }

        // 2. 최종 방어막: ID가 여전히 비어있다면 Factory를 호출하지 않음
        if (string.IsNullOrEmpty(targetID))
        {
            Debug.LogError($"[StageManager] ID가 누락되었습니다! (isBoss: {info.isBoss}, info.enemy_ID: {info.enemy_ID})");
            return;
        }

        // 3. 이제 안전하게 소환
        Debug.Log($"[StageManager] 소환 시작: {targetID} (isBoss: {info.isBoss})");
        EnemyFactory.Instance.Spawn(targetID, info.spawn_Pos, info.spawn_Rotation);

        activeEnemyCount++;
    }

    #endregion

    #region 4. 클리어 판정 및 연출

    public void NotifyEnemyKilled()
    {
        // 스테이지 데이터가 없거나 이미 클리어된 경우 중복 실행 방지
        if (currentStage == null || clearedOnce) return;

        // 1. 카운트 업데이트
        killCount++;         // 전체 누적 킬 수 (UI 표시용)
        activeEnemyCount--;  // 현재 필드에 남아있는 적 수 (KillAll 판정용)

        // 2. 클리어 조건 체크
        switch (currentStage.clear_Type)
        {
            // 조건 A: 목표 킬 수 달성
            case ClearConditionType.KillTarget:
                if (killCount >= currentStage.target_KillCount)
                {
                    OnStageCleared();
                }
                break;

            // 조건 B: 모든 적 처치 (소환 대기 명단 + 필드 적 모두 0)
            case ClearConditionType.KillAll:
                if (pendingSpawns.Count == 0 && activeEnemyCount <= 0)
                {
                    OnStageCleared();
                }
                break;

                // 보스 처치는 별도의 NotifyBossKilled에서 처리하거나 
                // 여기서 Boss 태그 등을 확인하여 처리할 수 있습니다.
        }

        Debug.Log($"[StageManager] Enemy Killed. (Total Kills: {killCount}, Active: {activeEnemyCount}, Pending: {pendingSpawns.Count})");
    }

    public void NotifyBossKilled()
    {
        if (currentStage == null || clearedOnce) return;

        if (currentStage.clear_Type == ClearConditionType.Boss)
            OnStageCleared();
    }
private void OnStageCleared()
    {
        if (clearedOnce) return;
        clearedOnce = true;

        Debug.Log($"[StageManager] 1. OnStageCleared 진입 완료!"); // 확인용

        StartCoroutine(ClearSequenceRoutine());
    }

    private IEnumerator ClearSequenceRoutine()
    {
        Debug.Log("[StageManager] 2. 코루틴 시작!"); // 확인용

        Time.timeScale = 0.2f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; 

        yield return new WaitForSecondsRealtime(1.5f);

        Debug.Log("[StageManager] 3. 대기 시간 종료!"); // 확인용

        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = 0.02f;

        if (portal != null)
        {
            Debug.Log("[StageManager] 4. 포탈 Activate 호출 직전!"); // 확인용
            portal.Activate();
            Debug.Log("[StageManager] 5. 모든 로직 완료!"); // 확인용
        }
        else
        {
            Debug.LogError("[StageManager] 에러: 포탈을 찾을 수 없습니다!");
        }
    }



#endregion

#region 기타

    private void ParseClearCondition(StageData stage)
    {
        // 이 부분은 이미 JSON 로더에서 Enum으로 잘 들어오고 있다면 없어도 되지만,
        // 예외 처리를 위해 남겨두는 것도 좋습니다.
        if (stage.clear_Type == ClearConditionType.None) return;

        Debug.Log($"[StageManager] Clear Condition: {stage.clear_Type} (Target: {stage.target_KillCount})");
    }

#endregion





}