using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BossUpgradeUI : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardPrefab;

    private List<BossJsonData> bossDefs = new();
    private List<BossUpgradeJsonData> bossUpgrades = new();

    public void SetBossJson(List<BossJsonData> defs) => bossDefs = defs ?? new();
    public void SetBossUpgradeJson(List<BossUpgradeJsonData> ups) => bossUpgrades = ups ?? new();

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Setup()
    {
        Debug.Log($"[BossUpgradeUI] currentBossId = '{RuntimeManager.Instance.GetCurrentBossId()}'");

        if (cardContainer == null || cardPrefab == null)
        {
            Debug.LogError("[BossUpgradeUI] cardContainer/cardPrefab 미할당");
            return;
        }

        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);

        if (RuntimeManager.Instance == null)
        {
            Debug.LogError("[BossUpgradeUI] RuntimeManager.Instance null");
            return;
        }

        List<BossUpgradeJsonData> upgrades = RuntimeManager.Instance.GetRandomThreeUpgrades(); // 네가 이미 만든 함수 사용
        if (upgrades == null || upgrades.Count == 0)
        {

            Debug.LogWarning("[BossUpgradeUI] 사용 가능한 강화가 없습니다!");
            Debug.LogWarning($"count {upgrades.Count}");
            gameObject.SetActive(false);
            return;
        }

        foreach (var up in upgrades)
            CreateUpgradeCard(up);

        Debug.Log($"[BossUpgradeUI] Setup 완료 - {upgrades.Count}개");
    }

    private void CreateUpgradeCard(BossUpgradeJsonData up)
    {
        if (up == null) return;

        GameObject cardObj = Instantiate(cardPrefab, cardContainer);
        CardUI card = cardObj.GetComponent<CardUI>();
        if (card == null)
        {
            Debug.LogError("[BossUpgradeUI] cardPrefab에 CardUI 없음");
            return;
        }

        card.Setup(
            up.upgrade_Name,
            up.upgrade_Description,
            null,
            () => OnUpgradePicked(up)
        );
    }

    private void OnUpgradePicked(BossUpgradeJsonData picked)
    {
        if (picked == null) return;

        // 1) 저장
        RuntimeManager.Instance.AddBossUpgrade(picked);

        // 2) 닫기
        gameObject.SetActive(false);

        // 3) 보스 씬 진입
        Debug.Log("[BossUpgradeUI] LoadScene BossStage");
        SceneManager.LoadScene("Lobby");
    }
    
     public BossJsonData GetBossById(string bossId)
    {
        if (string.IsNullOrEmpty(bossId)) return null;
        return bossDefs.Find(b => b.boss_ID == bossId);
    }

    public List<BossUpgradeJsonData> GetUpgradesForBoss(string bossId)
    {
        if (string.IsNullOrEmpty(bossId)) return new List<BossUpgradeJsonData>();
        return bossUpgrades.FindAll(u => string.IsNullOrEmpty(u.boss_ID) || u.boss_ID == bossId);
    }
}
