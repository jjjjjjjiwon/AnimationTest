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
    [SerializeField] private Transform cardContainer;  // 카드들이 들어갈 부모
    [SerializeField] private GameObject cardPrefab;    // CardUI 프리팹
    [SerializeField] private Button confirmButton;


    private StageData currentStage;
    private bool isBossStage;

    // 선택 추적
    private List<CardUI> rewardCards = new List<CardUI>();

    // ========================================
    // Initialization
    // ========================================

    private void Awake()
    {
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmClick);
        else
            Debug.LogError("[RewardUI] confirmButton 미할당");
    }

    private void OnDestroy()
    {
        if (confirmButton != null)
            confirmButton.onClick.RemoveListener(OnConfirmClick);
    }
    void Start()
    {
        gameObject.SetActive(false);
    }

    // ========================================
    // Public API - UIManager가 호출
    // ========================================

    /// <summary>보상 UI 설정 및 표시 준비</summary>
    public void Setup(StageData stage, bool isBoss)
    {
        if (stage == null)
        {
            Debug.LogError("[RewardUI] stage null");
            return;
        }

        currentStage = stage;
        isBossStage = isBoss;

        foreach (Transform child in cardContainer)
            Destroy(child.gameObject);

        rewardCards.Clear();
        CreateRewardCards();
    }


    // ========================================
    // 카드 생성
    // ========================================

    private void CreateRewardCards()
    {
        // 골드 카드
        if (currentStage.gold_Reward > 0)
        {
            CreateGoldCard();
        }

        // 스탯 포인트 카드
        if (currentStage.levelUp_Point > 0)
        {
            CreateStatPointCard();
        }

        // 아이템 카드들
        if (currentStage.item_Rewards != null)
        {
            foreach (string itemID in currentStage.item_Rewards)
            {
                CreateItemCard(itemID);
            }
        }

        // 스킬 카드
        if (!string.IsNullOrEmpty(currentStage.skill_Reward))
        {
            CreateSkillCard(currentStage.skill_Reward);
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
            int goldAmount = currentStage.gold_Reward;

            card.Setup(
                "골드",
                $"+{goldAmount}G",
                null,
                () => OnGoldCardClick(goldAmount)
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
            int points = currentStage.levelUp_Point;

            card.Setup(
                "스탯 포인트",
                $"+{points}p",
                null,
                () => OnStatPointCardClick(points)
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
                () => OnItemCardClick(itemID)
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
                () => OnSkillCardClick(skillID)
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

    public void OnConfirmClick()
    {
        // ✅ 보스 스테이지일 때만 보스 강화 UI 진입
        if (!isBossStage)
        {
            UIManager.Instance.ShowBossUpgradeUI();
        }

        // 일반 스테이지면 바로 닫기
        gameObject.SetActive(false);
    }


    // ========================================
    // 선택 상태 확인 (Optional - UI 피드백용)
    // ========================================

    /// <summary>모든 카드를 선택했는지 확인</summary>
    private bool AllCardsSelected()
    {
        foreach (CardUI card in rewardCards)
        {
            if (!card.IsSelected)
                return false;
        }
        return true;
    }

    /// <summary>Update에서 확인 버튼 텍스트 업데이트 (Optional)</summary>
    void Update()
    {

    }
}