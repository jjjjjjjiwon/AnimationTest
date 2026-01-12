using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 묘지에 표시되는 개별 카드 아이템
/// </summary>
public class GraveyardCardItem : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image cardImage;          // 카드 이미지
    [SerializeField] private TextMeshProUGUI nameText; // 이름
    [SerializeField] private TextMeshProUGUI descText; // 설명
    
    /// <summary>
    /// 카드 정보 세팅
    /// </summary>
    /// <param name="stageData">표시할 스테이지 데이터</param>
    public void Setup(StageData stageData)
    {
        if (stageData == null)
        {
            Debug.LogWarning("[GraveyardCardItem] StageData가 null입니다!");
            return;
        }
        
        // 이미지
        if (cardImage != null && stageData.stageIcon != null)
        {
            cardImage.sprite = stageData.stageIcon;
        }
        
        // 이름
        if (nameText != null)
        {
            nameText.text = stageData.stageName;
        }
        
        // 설명
        if (descText != null)
        {
            descText.text = stageData.stageDescription;
        }
    }
}
