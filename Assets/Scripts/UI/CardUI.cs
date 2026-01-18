using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// 범용 카드 UI
/// 보상 카드, 강화 카드 등에서 사용
/// </summary>
public class CardUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image backgroundImage;      // 카드 배경
    [SerializeField] private Image iconImage;            // 아이콘
    [SerializeField] private TextMeshProUGUI titleText;  // 제목
    [SerializeField] private TextMeshProUGUI descriptionText;  // 설명
    [SerializeField] private Button button;              // 버튼 (전체 카드)
    
    private Action onClickCallback;
    
    // ========================================
    // 설정
    // ========================================
    
    /// <summary>
    /// 카드 설정
    /// </summary>
    /// <param name="title">제목 (예: "골드", "체력 강화")</param>
    /// <param name="description">설명 (예: "+1000G", "보스 체력 +20%")</param>
    /// <param name="icon">아이콘 (null 가능)</param>
    /// <param name="onClick">클릭 콜백 (null이면 클릭 불가)</param>
    public void Setup(string title, string description, Sprite icon, Action onClick)
    {
        // 텍스트 설정
        if (titleText != null)
            titleText.text = title;
        
        if (descriptionText != null)
            descriptionText.text = description;
        
        // 아이콘 설정
        if (iconImage != null)
        {
            if (icon != null)
            {
                iconImage.sprite = icon;
                iconImage.gameObject.SetActive(true);
            }
            else
            {
                iconImage.gameObject.SetActive(false);  // 아이콘 없으면 숨김
            }
        }
        
        // 클릭 이벤트 설정
        onClickCallback = onClick;
        
        if (button != null)
        {
            button.onClick.RemoveAllListeners();  // 기존 리스너 제거
            
            if (onClick != null)
            {
                // 클릭 가능 (강화 카드)
                button.interactable = true;
                button.onClick.AddListener(OnClick);
            }
            else
            {
                // 클릭 불가 (보상 카드)
                button.interactable = false;
            }
        }
    }
    
    // ========================================
    // 이벤트
    // ========================================
    
    private void OnClick()
    {
        onClickCallback?.Invoke();
    }
}