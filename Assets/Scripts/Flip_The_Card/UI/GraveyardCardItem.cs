using UnityEngine;
using UnityEngine.UI; // Image 사용을 위해 추가
using TMPro;

public class GraveyardCardItem : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image cardImage;          // MeshRenderer 대신 Image
    [SerializeField] private TextMeshProUGUI nameText;  // TextMeshPro 대신 UGUI
    [SerializeField] private TextMeshProUGUI descText;  // TextMeshPro 대신 UGUI
    
    public void Setup(StageData stageData)
    {
        if (stageData == null) return;
        
        // 이미지 설정 (Sprite 사용)
        if (cardImage != null && stageData.stage_Icon != null)
        {
            cardImage.sprite = stageData.stage_Icon;
        }
        
        if (nameText != null) nameText.text = stageData.stage_Name;
        if (descText != null) descText.text = stageData.stage_Description;
    }
}