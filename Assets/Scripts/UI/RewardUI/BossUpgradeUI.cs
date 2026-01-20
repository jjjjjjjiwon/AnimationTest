using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 보스 강화 선택 UI
/// 일반 스테이지 클리어 후 표시
/// </summary>
public class BossUpgradeUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform cardContainer;  // 카드 3개가 들어갈 부모
    [SerializeField] private GameObject cardPrefab;    // CardUI 프리팹
    
    // ========================================
    // Initialization
    // ========================================
    
    void Start()
    {
        gameObject.SetActive(false);
    }
    
    // ========================================
    // Public API - UIManager가 호출
    // ========================================
    
    /// <summary>보스 강화 UI 설정 및 표시 준비</summary>
    public void Setup()
    {
        // 기존 카드 제거
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }
        
        // 랜덤 3개 강화 가져오기
        List<BossUpgrade> upgrades = RuntimeManager.Instance.GetRandomThreeUpgrades();
        
        if (upgrades.Count == 0)
        {
            Debug.LogWarning("[BossUpgradeUI] 사용 가능한 강화가 없습니다!");
            gameObject.SetActive(false);
            return;
        }
        
        // 카드 생성
        foreach (var upgrade in upgrades)
        {
            CreateUpgradeCard(upgrade);
        }
        
        Debug.Log($"[BossUpgradeUI] Setup 완료 - {upgrades.Count}개 강화");
    }
    
    // ========================================
    // 카드 생성
    // ========================================
    
    private void CreateUpgradeCard(BossUpgrade upgrade)
    {
        GameObject cardObj = Instantiate(cardPrefab, cardContainer);
        CardUI card = cardObj.GetComponent<CardUI>();
        
        if (card != null)
        {
            // 카드 설정 (클릭 시 OnCardSelected 호출)
            card.Setup(
                upgrade.upgradeName, 
                upgrade.upgradeDescription, 
                null,  // TODO: 아이콘
                () => OnCardSelected(upgrade)
            );
        }
    }
    
    // ========================================
    // 이벤트
    // ========================================
    
    private void OnCardSelected(BossUpgrade upgrade)
    {
        // 강화 선택 저장
        RuntimeManager.Instance.SelectUpgrade(upgrade.upgradeID);
        
        // UI 닫기
        gameObject.SetActive(false);
        
        // 로비로 복귀
        if (StageManager.Instance != null)
        {
            StageManager.Instance.LoadLobby();
        }
        
        Debug.Log($"[BossUpgradeUI] 강화 선택: {upgrade.upgradeName}");
    }
}