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
        if (jsonFile == null) return;

        // StageData 클래스 하나로 바로 파싱!
        StageDataList dataList = JsonUtility.FromJson<StageDataList>(jsonFile.text);
        
        if (GameData.Instance != null)
            GameData.Instance.allStageData = dataList.stages;
    }
}