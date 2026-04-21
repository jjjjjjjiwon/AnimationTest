using System.Collections.Generic;
using UnityEngine;

public class StageDataLoader : MonoBehaviour
{
    // JSON 전체를 감싸는 리스트 그릇
    [System.Serializable] 
    public class StageDataList { public List<StageData> stages; }

    void Start() => LoadStagesFromJSON();

void LoadStagesFromJSON()
{
    TextAsset jsonFile = Resources.Load<TextAsset>("Json/stages");
    if (jsonFile == null) 
    {
        Debug.LogError("[StageDataLoader] JSON 파일을 찾을 수 없습니다!");
        return;
    }

    StageDataList dataList = JsonUtility.FromJson<StageDataList>(jsonFile.text);
    
    // 🔥 [핵심 추가] 각 스테이지의 경로를 읽어 Sprite 에셋을 로드합니다.
    foreach (StageData stage in dataList.stages)
    {
        if (!string.IsNullOrEmpty(stage.icon_Path))
        {
            // Resources/ 하위 경로에서 Sprite를 로드
            // 예: JSON에 "Icons/Stage1"이라고 적혀있다면 Resources/Icons/Stage1.png(또는 jpg)를 가져옵니다.
            stage.stage_Icon = Resources.Load<Sprite>(stage.icon_Path);

            if (stage.stage_Icon == null)
            {
                Debug.LogWarning($"[StageDataLoader] 아이콘 로드 실패: {stage.icon_Path}");
            }
        }
    }

    if (GameData.Instance != null)
    {
        GameData.Instance.allStageData = dataList.stages;
        Debug.Log($"[StageDataLoader] {dataList.stages.Count}개의 스테이지 로드 완료");
    }
}
}