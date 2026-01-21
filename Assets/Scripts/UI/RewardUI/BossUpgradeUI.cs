using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BossUpgradeUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform cardContainer;
    [SerializeField] private GameObject cardPrefab;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Setup()
    {
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

        List<BossUpgrade> upgrades = RuntimeManager.Instance.GetRandomThreeUpgrades(); // 네가 이미 만든 함수 사용

        if (upgrades == null || upgrades.Count == 0)
        {
            Debug.LogWarning("[BossUpgradeUI] 사용 가능한 강화가 없습니다!");
            gameObject.SetActive(false);
            return;
        }

        foreach (var up in upgrades)
            CreateUpgradeCard(up);

        Debug.Log($"[BossUpgradeUI] Setup 완료 - {upgrades.Count}개");
    }

    private void CreateUpgradeCard(BossUpgrade up)
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
            up.upgradeName,
            up.upgradeDescription,
            null,
            () => OnUpgradePicked(up)
        );
    }

    private void OnUpgradePicked(BossUpgrade picked)
    {
        if (picked == null) return;

        // 1) 저장
        RuntimeManager.Instance.AddBossUpgrade(picked);

        // 2) 닫기
        gameObject.SetActive(false);

        // 3) 보스 씬 진입
        Debug.Log("[BossUpgradeUI] LoadScene BossStage");
        SceneManager.LoadScene("BossStage");
    }
}
