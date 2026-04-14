using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class EnemyDataList { public List<EnemyJsonData> Enemy; }

public class EnemyDataLoader : MonoBehaviour
{
    [SerializeField] private string JsonPath = "Json/Enemy";
    [SerializeField] private string bossJsonPath = "Json/Boss";

    // 리스트가 null인 상태에서 접근하면 에러가 날 수 있으니 미리 초기화해두는 것이 안전합니다.
    private List<EnemyJsonData> enemyList = new List<EnemyJsonData>();  // enemy
    private List<BossJsonData> bossList = new List<BossJsonData>();     // boss

    public List<EnemyJsonData> Enemies => enemyList;
    public List<BossJsonData> Bosses => bossList;

    public void LoadEnemies()
    {
        // 1. 일반 적 로드
        TextAsset enemyAsset = Resources.Load<TextAsset>(JsonPath);
        if (enemyAsset != null)
        {
            EnemyDataList dataWrapper = JsonUtility.FromJson<EnemyDataList>(enemyAsset.text);
            if (dataWrapper != null && dataWrapper.Enemy != null)
            {
                enemyList = dataWrapper.Enemy;
                Debug.Log($"✅ [EnemyDataLoader] 적 데이터 {enemyList.Count}개 로드 완료");
            }
        }
        else { Debug.LogError($"❌ [EnemyDataLoader] 적 JSON 없음: {JsonPath}"); }

        // 2. 보스 로드
        TextAsset bossAsset = Resources.Load<TextAsset>(bossJsonPath);
        if (bossAsset != null)
        {
            BossJsonList list = JsonUtility.FromJson<BossJsonList>(bossAsset.text);
            if (list != null && list.boss != null)
            {
                bossList = list.boss;
                GameData.Instance?.SetBossJson(bossList); // GameData에도 저장
                Debug.Log($"✅ [EnemyDataLoader] 보스 데이터 {bossList.Count}개 로드 완료");
            }
        }
        else { Debug.LogError($"❌ [EnemyDataLoader] 보스 JSON 없음: {bossJsonPath}"); }
    }

    public void LoadBoss()
    {
        TextAsset json = Resources.Load<TextAsset>(bossJsonPath);
        if (json == null)
        {
            Debug.LogError($"[EnemyDataLoader] Boss JSON not found: {bossJsonPath}");
            return;
        }

        BossJsonList list = JsonUtility.FromJson<BossJsonList>(json.text);
        if (list != null && list.boss != null)
        {
            bossList = list.boss;
            GameData.Instance?.SetBossJson(bossList);
            Debug.Log($"[EnemyDataLoader] Loaded Bosses: {bossList.Count}");
        }
    }

    // 외부(Factory)에서 데이터를 가져갈 수 있게 해주는 창구
    public List<EnemyJsonData> GetEnemyList() => enemyList;
    public List<BossJsonData> GetBossList() => bossList;
}