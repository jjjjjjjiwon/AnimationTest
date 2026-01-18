using UnityEngine;
using UnityEngine.SceneManagement;

public enum ClearConditionType
{
    KillAll,      // 모든 적 처치
    KillTarget,   // 특정 수만큼 처치
    Boss          // 보스 처치
}

/// <summary>
/// 스테이지 씬 로드 및 클리어 관리
/// </summary>
public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }


    [Header("Clear Condition")]
[SerializeField] private ClearConditionType clearCondition;
[SerializeField] private int targetKillCount;  // 처치 목표 수
[SerializeField] private Portal portal;        // 포탈 참조

private int currentKillCount = 0;
    
    private StageData currentStage;  // ⭐ 추가
    
    // ========================================
    // 싱글톤 ⭐
    // ========================================
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // 씬 전환 시에도 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // ========================================
    // 스테이지 로드
    // ========================================
    
    /// <summary>스테이지 씬 로드</summary>
    public void LoadStage(StageData stageData)
    {
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
        
        // ⭐ 현재 스테이지 저장
        currentStage = stageData;
        
        Debug.Log($"[StageManager] 씬 로드: {stageData.sceneName}");
        
        // 씬 로드
        SceneManager.LoadScene(stageData.sceneName);
    }
    
    // ========================================
    // 스테이지 클리어 ⭐
    // ========================================
    
    /// <summary>스테이지 클리어 처리 (Boss/Enemy가 호출)</summary>
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
    bool cleared = false;
    
    switch (clearCondition)
    {
        case ClearConditionType.KillAll:
            // 모든 적 처치 (씬에 적 없음)
            int remainingEnemies = FindObjectsOfType<EnemyController>().Length;
            if (remainingEnemies == 0)
                cleared = true;
            break;
            
        case ClearConditionType.KillTarget:
            // 목표 수만큼 처치
            if (currentKillCount >= targetKillCount)
                cleared = true;
            break;
            
        case ClearConditionType.Boss:
            // 보스는 Die()에서 직접 포탈 활성화
            break;
    }
    
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
}
}