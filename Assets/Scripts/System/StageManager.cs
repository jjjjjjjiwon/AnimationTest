using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// 스테이지 씬 로드 및 클리어 관리
/// </summary>
public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    private StageData currentStage;

    [Header("Clear Condition")]
    [SerializeField] private ClearConditionType clearCondition;
    [SerializeField] private int targetKillCount;  // 처치 목표 수
    [SerializeField] private Portal portal;        // 포탈 참조

    private int currentKillCount = 0;

    public enum ClearConditionType
    {
        KillAll,      // 모든 적 처치
        KillTarget,   // 특정 수만큼 처치
        Boss          // 보스 처치
    }

    // ========================================
    // 싱글톤
    // ========================================

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

   void Start()
{
    StartCoroutine(InitializeStage());
}

IEnumerator InitializeStage()
{
    // 1프레임 대기 (다른 Start()들이 끝날 때까지)
    yield return null;
    
    Debug.Log("[StageManager] Start() 실행!");
    
    if (currentStage == null && GameData.Instance != null)
    {
        Debug.Log("[StageManager] currentStage null - 1층 로드 시도");
        currentStage = GameData.Instance.GetStageByFloor(1);
        
        if (currentStage != null)
        {
            Debug.Log($"[StageManager] {currentStage.stageName} 로드 완료");
        }
        else
        {
            Debug.LogError("[StageManager] 1층을 찾을 수 없습니다!");
        }
    }
}


    // ========================================
    // 스테이지 로드
    // ========================================

    /// <summary>스테이지 씬 로드</summary>
    public void LoadStage(StageData stageData)
    {

        // 보스 정보 설정 (추가!)
if (RuntimeManager.Instance != null)
{
    RuntimeManager.Instance.PrepareFloor(stageData.stageID);
    Debug.Log($"[StageManager] 층 {stageData.stageID} 준비 완료 - 보스: {RuntimeManager.Instance.currentBossName}");
}
        if (stageData == null)
        {
            Debug.LogError("[StageManager] StageData가 null입니다!");
            return;
        }

        if (string.IsNullOrEmpty(stageData.sceneName))
        {
            Debug.LogError($"[StageManager] {stageData.stageName}의 sceneName이 비어있습니다!");
            return;
        }

        // 현재 스테이지 저장
        currentStage = stageData;

        Debug.Log($"[StageManager] 씬 로드: {stageData.sceneName}");

        // 씬 로드
        SceneManager.LoadScene(stageData.sceneName);
    }

    // ========================================
    // 적 처치 카운트
    // ========================================

    /// <summary>적 처치 시 호출</summary>
    public void OnEnemyKilled()
    {
        currentKillCount++;

        Debug.Log($"[StageManager] 적 처치: {currentKillCount}");

        // 클리어 조건 체크
        CheckClearCondition();
    }

    private void CheckClearCondition()
    {
        Debug.Log($"[CheckClearCondition] 조건: {clearCondition}, 카운트: {currentKillCount}/{targetKillCount}");

        bool cleared = false;

        switch (clearCondition)
        {
            case ClearConditionType.KillAll:
                int remainingEnemies = FindObjectsOfType<EnemyController>().Length;
                Debug.Log($"[KillAll] 남은 적: {remainingEnemies}");
                if (remainingEnemies == 0)
                    cleared = true;
                break;

            case ClearConditionType.KillTarget:
                Debug.Log($"[KillTarget] {currentKillCount} >= {targetKillCount}?");
                if (currentKillCount >= targetKillCount)
                    cleared = true;
                break;

            case ClearConditionType.Boss:
                Debug.Log("[Boss] 보스 모드");
                break;
        }

        Debug.Log($"[CheckClearCondition] Cleared: {cleared}");

        if (cleared)
        {
            ActivatePortal();
        }
    }

    /// <summary>포탈 활성화</summary>
    public void ActivatePortal()
    {
        if (portal != null)
        {
            portal.Activate();
            Debug.Log("[StageManager] 포탈 활성화!");
        }
        else
        {
            Debug.LogWarning("[StageManager] Portal 참조가 없습니다!");
        }
    }

    // ========================================
    // 스테이지 클리어
    // ========================================

    /// <summary>스테이지 클리어 처리 (Portal에서 호출)</summary>
    public void OnStageCleared()
    {
        if (currentStage == null)
        {
            Debug.LogError("[StageManager] currentStage가 null입니다!");
            return;
        }

        Debug.Log($"[StageManager] 스테이지 클리어: {currentStage.stageName}");

        // 보상 지급 + UI 표시
        RuntimeManager.Instance.GiveReward(currentStage);
    }

    // ========================================
    // 로비 복귀
    // ========================================

    /// <summary>로비로 돌아가기</summary>
    public void LoadLobby()
    {
        Debug.Log("[StageManager] 로비로 복귀");

        // TODO: 로비 씬 이름 확인 필요
        SceneManager.LoadScene("Lobby");
    }
}