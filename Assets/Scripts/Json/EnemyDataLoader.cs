using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class EnemyDataList { public List<EnemyJsonData> Enemy; }

public class EnemyDataLoader : MonoBehaviour
{
    [SerializeField] private string JsonPath = "json/Enemy";
    // 리스트가 null인 상태에서 접근하면 에러가 날 수 있으니 미리 초기화해두는 것이 안전합니다.
    private List<EnemyJsonData> enemyList = new List<EnemyJsonData>();

    public void LoadEnemy()
    {

        TextAsset jsonFile = Resources.Load<TextAsset>("Json/Enemy"); // 경로 확인!

        if (jsonFile == null)
        {
            Debug.LogError("❌ [EnemyDataLoader] JSON 파일을 찾을 수 없습니다! 경로를 확인하세요.");
            return;
        }

        Debug.Log("✅ [EnemyDataLoader] 파일 읽기 성공: " + jsonFile.text);

        TextAsset jsonAsset = Resources.Load<TextAsset>(JsonPath);

        if (jsonAsset == null)
        {
            Debug.LogError($"[EnemyDataLoader] JSON 파일을 찾을 수 없습니다: Resources/{JsonPath}");
            return;
        }

        try
        {
            EnemyDataList dataWrapper = JsonUtility.FromJson<EnemyDataList>(jsonAsset.text);

            if (dataWrapper != null && dataWrapper.Enemy != null) // .Enemy로 수정
            {
                enemyList = dataWrapper.Enemy; // .Enemy로 수정
                Debug.Log($"[EnemyDataLoader] {enemyList.Count}개의 적 데이터를 성공적으로 로드했습니다.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"[EnemyDataLoader] JSON 파싱 중 오류 발생: {e.Message}");
        }
    }

    // 외부(Factory)에서 데이터를 가져갈 수 있게 해주는 창구
    public List<EnemyJsonData> GetEnemyList() => enemyList;
}