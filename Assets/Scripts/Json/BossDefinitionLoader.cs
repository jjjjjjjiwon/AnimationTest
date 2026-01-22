using UnityEngine;
using System.Collections.Generic;

public class BossDefinitionLoader : MonoBehaviour
{
    [SerializeField] private string jsonPath = "Json/boss"; // Resources/Json/boss.json

    private void Start()
    {
        LoadBosses();
    }

    public void LoadBosses()
    {
        TextAsset json = Resources.Load<TextAsset>(jsonPath);

        if (json == null)
        {
            Debug.LogError($"[BossDefinitionLoader] JSON not found: Resources/{jsonPath}.json");
            return;
        }

        BossJsonList list = JsonUtility.FromJson<BossJsonList>(json.text);

        if (list == null || list.boss == null)
        {
            Debug.LogError("[BossDefinitionLoader] JSON parse failed");
            return;
        }

        if (GameData.Instance == null)
        {
            Debug.LogError("[BossDefinitionLoader] GameData.Instance null");
            return;
        }

        GameData.Instance.SetBossJson(list.boss);
        Debug.Log($"[BossDefinitionLoader] loaded bosses: {list.boss.Count}");
    }
}
