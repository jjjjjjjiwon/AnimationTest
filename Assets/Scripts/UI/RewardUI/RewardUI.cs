using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 보상 화면 UI
/// 스테이지 클리어 후 표시
/// 카드를 클릭하면 보상 획득
/// </summary>
public class RewardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform cardContainer;  // 카드들이 들어갈 부모
    [SerializeField] private GameObject cardPrefab;    // CardUI 프리팹
    [SerializeField] private Button confirmButton;
    [SerializeField] private TextMeshProUGUI confirmButtonText; // "확인" 또는 "다음"
    
    private StageData currentStage;
    private bool isBossStage;
    
    // 선택 추적
    private List<CardUI> rewardCards = new List<CardUI>();
    
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
        rewardCards.Clear();
        
        // 보상 카드 생성
        CreateRewardCards();
        
        // 확인 버튼 텍스트 설정
        if (confirmButtonText != null)
        {
            confirmButtonText.text = "확인";
        }
        
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
            CreateGoldCard();
        }
        
        // 스탯 포인트 카드
        if (currentStage.levelUpPoint > 0)
        {
            CreateStatPointCard();
        }
        
        // 아이템 카드들
        if (currentStage.itemRewards != null)
        {
            foreach (string itemID in currentStage.itemRewards)
            {
                CreateItemCard(itemID);
            }
        }
        
        // 스킬 카드
        if (!string.IsNullOrEmpty(currentStage.skillReward))
        {
            CreateSkillCard(currentStage.skillReward);
        }
    }
    
    // ========================================
    // 개별 카드 생성 (각각 클릭 이벤트 포함)
    // ========================================
    
    private void CreateGoldCard()
    {
        GameObject cardObj = Instantiate(cardPrefab, cardContainer);
        CardUI card = cardObj.GetComponent<CardUI>();
        
        if (card != null)
        {
            int goldAmount = currentStage.goldReward;
            
            card.Setup(
                "골드", 
                $"+{goldAmount}G", 
                null,
                () => OnGoldCardClick(goldAmount)  // ⭐ 클릭 콜백
            );
            
            rewardCards.Add(card);
        }
    }
    
    private void CreateStatPointCard()
    {
        GameObject cardObj = Instantiate(cardPrefab, cardContainer);
        CardUI card = cardObj.GetComponent<CardUI>();
        
        if (card != null)
        {
            int points = currentStage.levelUpPoint;
            
            card.Setup(
                "스탯 포인트", 
                $"+{points}p", 
                null,
                () => OnStatPointCardClick(points)  // ⭐ 클릭 콜백
            );
            
            rewardCards.Add(card);
        }
    }
    
    private void CreateItemCard(string itemID)
    {
        GameObject cardObj = Instantiate(cardPrefab, cardContainer);
        CardUI card = cardObj.GetComponent<CardUI>();
        
        if (card != null)
        {
            // TODO: 아이템 이름/아이콘 가져오기
            card.Setup(
                "아이템", 
                itemID, 
                null,
                () => OnItemCardClick(itemID)  // ⭐ 클릭 콜백
            );
            
            rewardCards.Add(card);
        }
    }
    
    private void CreateSkillCard(string skillID)
    {
        GameObject cardObj = Instantiate(cardPrefab, cardContainer);
        CardUI card = cardObj.GetComponent<CardUI>();
        
        if (card != null)
        {
            // TODO: 스킬 이름/아이콘 가져오기
            card.Setup(
                "스킬", 
                skillID, 
                null,
                () => OnSkillCardClick(skillID)  // ⭐ 클릭 콜백
            );
            
            rewardCards.Add(card);
        }
    }
    
    // ========================================
    // 카드 클릭 이벤트 (보상 지급)
    // ========================================
    
    private void OnGoldCardClick(int amount)
    {
        RuntimeManager.Instance.gold += amount;
        Debug.Log($"[RewardUI] 골드 획득: +{amount}G → 총 {RuntimeManager.Instance.gold}G");
    }
    
    private void OnStatPointCardClick(int points)
    {
        RuntimeManager.Instance.playerStats.availablePoints += points;
        Debug.Log($"[RewardUI] 스탯 포인트 획득: +{points}p → 총 {RuntimeManager.Instance.playerStats.availablePoints}p");
    }
    
    private void OnItemCardClick(string itemID)
    {
        RuntimeManager.Instance.playerInventory.AddItem(int.Parse(itemID), 1);
        Debug.Log($"[RewardUI] 아이템 획득: {itemID}");
    }
    
    private void OnSkillCardClick(string skillID)
    {
        // TODO: 스킬 시스템 구현 후 추가
        Debug.Log($"[RewardUI] 스킬 획득: {skillID}");
    }
    
    // ========================================
    // 버튼 이벤트
    // ========================================
    
    private void Start()
    {
        confirmButton.onClick.AddListener(OnConfirmClick);
        panel.SetActive(false);
    }
    
    /// <summary>
    /// 확인 버튼 클릭 - 다음 단계로
    /// (보상은 이미 카드 클릭 시 지급됨)
    /// </summary>
    private void OnConfirmClick()
    {
        // UI 닫기
        panel.SetActive(false);
        
        // 다음 단계
        if (isBossStage)
        {
            // 보스 클리어 → 다음 층 → 로비
            RuntimeManager.Instance.MoveToNextFloor();
            
            if (StageManager.Instance != null)
            {
                StageManager.Instance.LoadLobby();
            }
        }
        else
        {
            // 일반 스테이지 → 보스 강화 선택
            BossUpgradeUI.Show();
        }
    }
    
    // ========================================
    // 선택 상태 확인 (Optional - UI 피드백용)
    // ========================================
    
    /// <summary>
    /// 모든 카드를 선택했는지 확인
    /// </summary>
    private bool AllCardsSelected()
    {
        foreach (CardUI card in rewardCards)
        {
            if (!card.IsSelected)
                return false;
        }
        return true;
    }
    
    /// <summary>
    /// Update에서 확인 버튼 텍스트 업데이트 (Optional)
    /// </summary>
    void Update()
    {
        if (confirmButtonText != null && panel.activeSelf)
        {
            // 모든 카드를 선택했으면 "확인" → "다음"
            if (AllCardsSelected())
            {
                confirmButtonText.text = "다음";
            }
            else
            {
                confirmButtonText.text = "확인";
            }
        }
    }
}