using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 보상 화면 UI
/// 스테이지 클리어 후 표시
/// </summary>
public class RewardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform cardContainer;  // 카드들이 들어갈 부모
    [SerializeField] private GameObject cardPrefab;    // CardUI 프리팹
    [SerializeField] private Button confirmButton;
    
    private StageData currentStage;
    private bool isBossStage;
    
    // ========================================
    // 표시
    // ========================================
    
    /// <summary>보상 UI 표시</summary>
    public static void Show(StageData stage, bool isBoss)
    {
        // UI 인스턴스 찾기 (씬에 미리 배치되어 있어야 함)
        RewardUI instance = FindObjectOfType<RewardUI>();
        
        if (instance == null)
        {
            Debug.LogError("[RewardUI] RewardUI 인스턴스를 찾을 수 없습니다!");
            return;
        }
        
        instance.ShowInternal(stage, isBoss);
    }
    
    private void ShowInternal(StageData stage, bool isBoss)
    {
        currentStage = stage;
        isBossStage = isBoss;
        
        // 기존 카드 제거
        foreach (Transform child in cardContainer)
        {
            Destroy(child.gameObject);
        }
        
        // 보상 카드 생성
        CreateRewardCards();
        
        // 패널 표시
        panel.SetActive(true);
    }
    
    // ========================================
    // 카드 생성
    // ========================================
    
    private void CreateRewardCards()
    {
        // 골드 카드
        if (currentStage.goldReward > 0)
        {
            CreateCard("골드", $"+{currentStage.goldReward}G", null);
        }
        
        // 스탯 포인트 카드
        if (currentStage.levelUpPoint > 0)
        {
            CreateCard("스탯 포인트", $"+{currentStage.levelUpPoint}p", null);
        }
        
        // 아이템 카드들
        if (currentStage.itemRewards != null)
        {
            foreach (string itemID in currentStage.itemRewards)
            {
                CreateCard("아이템", itemID, null);  // TODO: 아이템 이름/아이콘
            }
        }
        
        // 스킬 카드
        if (!string.IsNullOrEmpty(currentStage.skillReward))
        {
            CreateCard("스킬", currentStage.skillReward, null);  // TODO: 스킬 이름/아이콘
        }
    }
    
    private void CreateCard(string title, string description, Sprite icon)
    {
        GameObject cardObj = Instantiate(cardPrefab, cardContainer);
        CardUI card = cardObj.GetComponent<CardUI>();
        
        if (card != null)
        {
            card.Setup(title, description, icon, null);  // 보상 카드는 클릭 불가
        }
    }
    
    // ========================================
    // 버튼 이벤트
    // ========================================
    
    private void Start()
    {
        confirmButton.onClick.AddListener(OnConfirmClick);
        panel.SetActive(false);
    }
    
    private void OnConfirmClick()
{
    // 아이템/스킬 지급
    GiveItems();
    
    // UI 닫기
    panel.SetActive(false);
    
    // 다음 단계
    if (isBossStage)
    {
        // 보스 클리어 → 다음 층
        RuntimeManager.Instance.MoveToNextFloor();
        
        // ⭐ 로비 씬 로드
        if (StageManager.Instance != null)
        {
            StageManager.Instance.LoadLobby();
        }
    }
    else
    {
        // 일반 스테이지 → 강화 선택
        BossUpgradeUI.Show();
    }
}
    
    private void GiveItems()
    {
        // 아이템 지급
        if (currentStage.itemRewards != null)
        {
            foreach (string itemID in currentStage.itemRewards)
            {
                RuntimeManager.Instance.playerInventory.AddItem(int.Parse(itemID), 1);
            }
        }
        
        // 스킬 지급
        if (!string.IsNullOrEmpty(currentStage.skillReward))
        {
            // TODO: 스킬 시스템 구현 후 추가
            Debug.Log($"[RewardUI] 스킬 획득: {currentStage.skillReward}");
        }
    }
}