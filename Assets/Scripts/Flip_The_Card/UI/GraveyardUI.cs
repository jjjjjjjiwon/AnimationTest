using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 묘지 UI 관리
/// 클리어한 카드들을 표시
/// </summary>
public class GraveyardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button graveyardButton;      // 묘지 버튼
    [SerializeField] private Button closeButton;          // 닫기 버튼
    [SerializeField] private TextMeshProUGUI buttonText;  // 버튼 텍스트 (개수 표시)
    [SerializeField] private GameObject graveyardPanel;   // 묘지 팝업 패널
    [SerializeField] private Transform contentParent;     // 카드들이 생성될 부모
    [SerializeField] private GameObject cardItemPrefab;   // 묘지 카드 아이템 프리팹
    
    void Start()
    {
        // 버튼 이벤트 연결
        if (graveyardButton != null)
        {
            graveyardButton.onClick.AddListener(OpenGraveyard);
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseGraveyard);
        }
        
        // 초기 상태: 팝업 닫힘
        if (graveyardPanel != null)
        {
            graveyardPanel.SetActive(false);
        }
        
        // 버튼 텍스트 업데이트
        // ????? 필요할까 ?????
        //UpdateButtonText();
    }
    
    /// <summary>
    /// 묘지 버튼 텍스트 업데이트 (개수 표시)
    /// </summary>
    void UpdateButtonText()
    {
        if (buttonText != null && GameData.Instance != null)
        {
            int count = GameData.Instance.clearedStages.Count;
            //buttonText.text = $"Grave ({count})"; // 수가 필요할까?
            buttonText.text = $"Grave"; 
        }
    }
    
    /// <summary>
    /// 묘지 팝업 열기
    /// </summary>
    void OpenGraveyard()
    {
        if (graveyardPanel == null || GameData.Instance == null) return;
        
        // 팝업 표시
        graveyardPanel.SetActive(true);
        
        // 기존 카드들 삭제
        ClearContent();
        
        // 클리어한 카드들 표시
        DisplayClearedCards();
        
        Debug.Log("[GraveyardUI] 묘지 열림");
    }
    
    /// <summary>
    /// 묘지 팝업 닫기
    /// </summary>
    void CloseGraveyard()
    {
        if (graveyardPanel != null)
        {
            graveyardPanel.SetActive(false);
        }
        
        Debug.Log("[GraveyardUI] 묘지 닫힘");
    }
    
    /// <summary>
    /// 기존 카드 아이템들 삭제
    /// </summary>
    void ClearContent()
    {
        if (contentParent == null) return;
        
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }
    
    /// <summary>
    /// 클리어한 카드들 표시
    /// </summary>
    void DisplayClearedCards()
    {
        if (cardItemPrefab == null || contentParent == null) return;
        
        List<StageData> clearedStages = GameData.Instance.clearedStages;
        
        if (clearedStages.Count == 0)
        {
            Debug.Log("[GraveyardUI] 클리어한 카드 없음");
            return;
        }
        
        // 각 클리어한 카드마다 UI 생성
        foreach (StageData stage in clearedStages)
        {
            GameObject item = Instantiate(cardItemPrefab, contentParent);
            GraveyardCardItem cardItem = item.GetComponent<GraveyardCardItem>();
            
            if (cardItem != null)
            {
                cardItem.Setup(stage);
            }


            RectTransform rt = item.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.localPosition = Vector3.zero;
            rt.localRotation = Quaternion.identity;
            rt.localScale = Vector3.one;
        }

        
        }
        
        Debug.Log($"[GraveyardUI] {clearedStages.Count}개 카드 표시");
    }
}