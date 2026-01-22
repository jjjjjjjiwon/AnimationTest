using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    private StageData currentStage;
    private int killCount;
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

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        portal = Object.FindFirstObjectByType<Portal>(FindObjectsInactive.Include);

        if (portal == null)
            Debug.LogWarning("[StageManager] Portal not found");
        else
            Debug.Log("[StageManager] Portal cached");
    }

    // ================= Stage Start =================

    public void StartStage(StageData stage)
    {
        if (stage == null)
        {
            Debug.LogError("[StageManager] StartStage: stage null");
            return;
        }

        currentStage = stage;
        ResetStageProgress();
        ParseClearCondition(stage);

        Debug.Log($"[StageManager] StartStage: {stage.stageName} ({stage.sceneName}) clear={stage.clearType}");

        // ✅ 씬 로드는 딱 한 번
        SceneManager.LoadScene(stage.sceneName);
    }

    private void ParseClearCondition(StageData stage)
    {
        if (string.IsNullOrEmpty(stage.clearConditionType))
        {
            stage.clearType = ClearConditionType.None;
            return;
        }

        if (!System.Enum.TryParse(stage.clearConditionType, true, out ClearConditionType parsed))
        {
            Debug.LogError($"[StageManager] clearConditionType parse failed: {stage.clearConditionType}");
            parsed = ClearConditionType.None;
        }

        stage.clearType = parsed;
    }

    private void ResetStageProgress()
    {
        killCount = 0;
        clearedOnce = false;
    }

    // ================= Clear Events =================

    public void NotifyEnemyKilled()
    {
        if (currentStage == null) return;
        if (currentStage.clearType != ClearConditionType.KillTarget) return;

        killCount++;

        if (killCount >= currentStage.targetKillCount)
            OnStageCleared();
    }

    public void NotifyBossKilled()
    {
        if (currentStage == null) return;
        if (currentStage.clearType != ClearConditionType.Boss) return;

        OnStageCleared();
    }

    // ================= Clear Result =================

    private void OnStageCleared()
    {
        if (clearedOnce) return;
        clearedOnce = true;

        Debug.Log($"[StageManager] Stage Cleared: {currentStage.stageName}");

        if (portal == null)
            portal = Object.FindFirstObjectByType<Portal>(FindObjectsInactive.Include);

        if (portal != null)
            portal.Activate();
    }

    // ================= Accessor =================

    public StageData GetCurrentStage() => currentStage;
    public int GetKillCount() => killCount;
}
