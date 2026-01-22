using UnityEngine;

public class BossUpgradeLoader : MonoBehaviour
{
    [SerializeField] private string jsonPath = "Json/BossUpgrade"; // Resources/Json/bossUpgrades.json

    private void Start()
    {
        LoadUpgrades();
    }

    public void LoadUpgrades()
    {
        TextAsset json = Resources.Load<TextAsset>(jsonPath);
        if (json == null)
        {
            Debug.LogError($"[BossUpgradeLoader] JSON not found: Resources/{jsonPath}.json");
            return;
        }

        BossUpgradeJsonList list = JsonUtility.FromJson<BossUpgradeJsonList>(json.text);
        if (list == null || list.upgrades == null)
        {
            Debug.LogError("[BossUpgradeLoader] JSON parse failed");
            return;
        }

        if (GameData.Instance == null)
        {
            Debug.LogError("[BossUpgradeLoader] GameData.Instance null");
            return;
        }

        GameData.Instance.SetBossUpgradeJson(list.upgrades);
        Debug.Log($"[BossUpgradeLoader] loaded upgrades: {list.upgrades.Count}");
    }
}
