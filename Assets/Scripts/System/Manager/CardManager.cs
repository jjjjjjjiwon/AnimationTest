using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [Header("Card Settings")]
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardParent;
    [SerializeField] private int cardCount = 3;
    [SerializeField] private float cardSpacing = 3f;
    
    private List<Card> spawnedCards = new List<Card>();
    
    void Start()
    {
        // 코루틴으로 대기 후 실행
        StartCoroutine(WaitAndSpawn());
    }
    
    /// <summary>
    /// StageDataLoader가 로드 완료할 때까지 대기
    /// </summary>
    IEnumerator WaitAndSpawn()
    {
        // GameData가 없으면 대기
        while (GameData.Instance == null)
        {
            yield return null;
        }
        
        // allStageData가 로드될 때까지 대기
        while (GameData.Instance.allStageData.Count == 0)
        {
            yield return null;
        }
        
        Debug.Log("[CardManager] JSON 로드 완료 확인 - 카드 생성 시작");
        SpawnCards();
    }
    
    void SpawnCards()
    {
        if (cardPrefab == null)
        {
            Debug.LogError("[CardManager] Card 프리팹이 없습니다!");
            return;
        }

        if (GameData.Instance.allStageData.Count == 0)
        {
            Debug.LogError("[CardManager] GameData에 StageData가 없습니다!");
            return;
        }

        if (GameData.Instance.selectedStages.Count == 0)
        {
            InitializeFirstTime();
        }
        
        CreateCards();
    }
        
    void InitializeFirstTime()
    {
        Random.InitState(GameData.Instance.currentSeed);
        
        List<StageData> selected = GetRandomStages(cardCount);
        GameData.Instance.selectedStages = selected;
        
        Debug.Log($"[CardManager] 처음 진입 - 카드 {selected.Count}개 선택 완료");
    }
    
    void CreateCards()
    {
        List<StageData> stages = GameData.Instance.selectedStages;
        
        int cardsPerRow = 5;
        float rowSpacing = 5f;
        
        for (int i = 0; i < stages.Count; i++)
        {
            StageData stageData = stages[i];
            
            if (stageData == null)
            {
                Debug.Log($"[CardManager] 인덱스 {i}는 null - 빈 공간");
                continue;
            }
            
            GameObject cardObj = Instantiate(cardPrefab, cardParent);
            Card card = cardObj.GetComponent<Card>();
            
            int row = i / cardsPerRow;
            int col = i % cardsPerRow;
            float x = col * cardSpacing;
            float y = row * rowSpacing;
            
            cardObj.transform.localPosition = new Vector3(x, y, 0);
            card.Initialize(stageData, i);
            
            spawnedCards.Add(card);
        }
        
        Debug.Log($"[CardManager] 카드 {spawnedCards.Count}개 생성 완료");
    }
    
    List<StageData> GetRandomStages(int count)
    {
        List<StageData> result = new List<StageData>();
        // 수정: GameData.allStageData 사용
        List<StageData> tempList = new List<StageData>(GameData.Instance.allStageData);
        
        count = Mathf.Min(count, tempList.Count);
        
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, tempList.Count);
            result.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);
        }
        
        return result;
    }
    
    public void OnCardSelected(Card selectedCard)
    {
        Debug.Log($"[CardManager] 카드 선택됨");
        
        foreach (Card card in spawnedCards)
        {
            if (card != selectedCard)
            {
                card.DisableInteraction();
            }
        }
    }
}