using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 스테이지 진행/클리어 관리
/// - 현재 StageData 보관
/// - KillTarget 클리어 조건 카운팅
/// - 클리어 시 RuntimeManager → UIManager로 보상 흐름 연결
/// </summary>
public class StageManager : MonoBehaviour
{
    // ========================================
    // Singleton
    // ========================================
    public static StageManager Instance { get; private set; }

    // ========================================
    // Current Stage State
    // ========================================
    private StageData currentStage;
    private int killCount;
    private bool clearedOnce;

    private Portal portal;

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Stage 씬 들어오면 포탈 재탐색
        portal = Object.FindFirstObjectByType<Portal>(FindObjectsInactive.Include);

        if (portal == null)
            Debug.LogWarning("[StageManager] Portal을 씬에서 찾지 못했습니다.");
        else
            Debug.Log("[StageManager] Portal 캐시 완료");
    }

    // ========================================
    // Initialization
    // ========================================
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning($"[StageManager] Duplicate detected. destroy thisID={GetInstanceID()}, keep instanceID={Instance.GetInstanceID()}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Debug.Log($"[StageManager] Awake thisID={GetInstanceID()}");
    }
    // ========================================
    // Stage Lifecycle API
    // ========================================

    /// <summary>
    /// (로비/런타임 등) 외부에서 "이번에 진행할 스테이지"를 지정할 때 호출.
    /// - 씬 로드 전에 호출하는 걸 권장.
    /// </summary>
    /// 
    private void ParseClearCondition(StageData stage)
    {
        if (string.IsNullOrEmpty(stage.clearConditionType))
        {
            stage.clearType = ClearConditionType.None;
            return;
        }

        if (!System.Enum.TryParse(stage.clearConditionType, true, out ClearConditionType parsed))
        {
            Debug.LogError($"[Stage] clearConditionType 파싱 실패: '{stage.clearConditionType}'");
            parsed = ClearConditionType.None;
        }

        stage.clearType = parsed;
    }


    public void StartStage(StageData stage)
    {
        Debug.Log($"[StageManager] StartStage called on InstanceID={GetInstanceID()}");

        if (stage == null)
        {
            Debug.LogError("[StageManager] StartStage(stage=null)");
            return;
        }

        // 1) 스테이지 데이터 세팅
        currentStage = stage;
        ResetStageProgress();

        // 2) clearCondition 파싱/검증 (JSON 누락 즉시 탐지)
        ParseClearCondition(currentStage);

        if (string.IsNullOrEmpty(currentStage.clearConditionType))
        {
            Debug.LogError("[StageManager] clearConditionType이 비어있음 (StageData 로딩/매핑 문제)");
            return;
        }

        if (currentStage.targetKillCount <= 0)
        {
            Debug.LogError($"[StageManager] targetKillCount가 0 이하임: {currentStage.targetKillCount} (StageData 로딩/매핑 문제)");
            return;
        }

        Debug.Log($"[StageManager] StartStage: {currentStage.stageName} (scene={currentStage.sceneName}) clear={currentStage.clearType} target={currentStage.targetKillCount}");

        // 3) 씬 로드
        if (!string.IsNullOrEmpty(currentStage.sceneName))
            SceneManager.LoadScene(currentStage.sceneName);
        else
            Debug.LogError("[StageManager] stage.sceneName 비어있음");
    }


    /// <summary>
    /// 스테이지 진행 상태 초기화(킬 카운트, 클리어 플래그)
    /// </summary>
    public void ResetStageProgress()
    {
        killCount = 0;
        clearedOnce = false;
    }

    /// <summary>
    /// 적이 죽을 때 Enemy 쪽에서 1회 호출해줘야 함.
    /// </summary>
    public void NotifyEnemyKilled()
    {
        Debug.Log($"[StageManager] NotifyEnemyKilled called on InstanceID={GetInstanceID()} (currentStage={(currentStage == null ? "null" : currentStage.stageName)})");

        if (currentStage == null)
        {
            Debug.LogError("[StageManager] NotifyEnemyKilled: currentStage null");
            return;
        }

        Debug.Log($"[StageManager] Condition={currentStage.clearConditionType}, target={currentStage.targetKillCount}");

        if (currentStage.clearType != ClearConditionType.KillTarget)
        {
            Debug.LogWarning("[StageManager] clearType이 KillTarget이 아님 → 카운트 스킵");
            return;
        }

        killCount++;
        Debug.Log($"[StageManager] KillTarget {killCount}/{currentStage.targetKillCount}");
        Debug.Log($"[StageManager] Condition={currentStage.clearType}, target={currentStage.targetKillCount}");


        if (killCount >= currentStage.targetKillCount)
        {
            Debug.Log("[StageManager] KillTarget 달성 → OnStageCleared 호출");
            OnStageCleared();
        }
    }


    /// <summary>
    /// 클리어 처리(중복 방지 포함)
    /// - RewardUI 흐름으로 내려가게 만드는 핵심 엔트리
    /// </summary>
   public void OnStageCleared()
{
    if (clearedOnce)
    {
        Debug.LogWarning("[StageManager] OnStageCleared() 중복 호출 무시");
        return;
    }
    clearedOnce = true;

    if (currentStage == null)
    {
        Debug.LogError("[StageManager] OnStageCleared() currentStage null");
        return;
    }

    Debug.Log($"[StageManager] Stage Cleared: {currentStage.stageName}");

    // ✅ 클리어 시: 포탈만 활성화
    Portal portal = Object.FindFirstObjectByType<Portal>(FindObjectsInactive.Include);
    if (portal != null)
    {
        portal.Activate();
    }
    else
    {
        Debug.LogWarning("[StageManager] Portal을 찾지 못했습니다.");
    }
}



    // ========================================
    // Navigation
    // ========================================

    public void LoadLobby()
    {
        // 프로젝트 로비 씬 이름에 맞춰 수정
        SceneManager.LoadScene("Lobby");
    }

    // ========================================
    // Debug / Accessor
    // ========================================
    public StageData GetCurrentStage() => currentStage;
    public int GetKillCount() => killCount;
}
