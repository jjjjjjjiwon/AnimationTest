using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 스테이지 씬 로드 관리
/// Card의 두 번째 클릭 시 호출됨
/// </summary>
public class StageManager : MonoBehaviour
{
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
        
        Debug.Log($"[StageManager] 씬 로드: {stageData.sceneName}");
        
        // 씬 로드
        SceneManager.LoadScene(stageData.sceneName);
    }

    // %%%임시%%%
    // 로비로 돌아가기
}