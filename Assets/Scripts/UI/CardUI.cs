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
    [SerializeField] private GameObject selectedOverlay; // 선택됨 표시 (Optional)
    
    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(0.8f, 1f, 0.8f); // 연한 녹색
    
    private Action onClickCallback;
    private bool isSelected = false;
    
    // ========================================
    // 설정
    // ========================================
    
    /// <summary>
    /// 카드 설정
    /// </summary>
    /// <param name="title">제목</param>
    /// <param name="description">설명</param>
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
                iconImage.gameObject.SetActive(false);
            }
        }
        
        // 클릭 이벤트 설정
        onClickCallback = onClick;
        
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            
            if (onClick != null)
            {
                // 클릭 가능
                button.interactable = true;
                button.onClick.AddListener(OnClick);
            }
            else
            {
                // 클릭 불가 (원래 보상 카드는 이거였음)
                button.interactable = false;
            }
        }
        
        // 초기 상태 설정
        SetSelected(false);
    }
    
    // ========================================
    // 선택 상태
    // ========================================
    
    /// <summary>
    /// 선택 상태 설정
    /// </summary>
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        
        // 배경색 변경
        if (backgroundImage != null)
        {
            backgroundImage.color = selected ? selectedColor : normalColor;
        }
        
        // 선택 오버레이 표시
        if (selectedOverlay != null)
        {
            selectedOverlay.SetActive(selected);
        }
        
        // 선택되면 클릭 불가능하게 (중복 획득 방지)
        if (button != null && selected)
        {
            button.interactable = false;
        }
    }
    
    /// <summary>
    /// 현재 선택 상태
    /// </summary>
    public bool IsSelected => isSelected;
    
    // ========================================
    // 이벤트
    // ========================================
    
    private void OnClick()
    {
        if (!isSelected)
        {
            onClickCallback?.Invoke();
            SetSelected(true);  // 클릭 시 선택 상태로 변경
        }
    }
}