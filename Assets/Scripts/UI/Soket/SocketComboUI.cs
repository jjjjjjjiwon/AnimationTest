using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SocketComboUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    
    [Header("Icon Slots")]
    [SerializeField] private List<Image> iconSlots = new List<Image>();
    
    [Header("Colors")]
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
    
    private SocketManager socketManager;  // ← 변경!
    private bool isInitialized = false;
    
    void Start()
    {
        Debug.Log("[UI] SocketComboUI Start!");
        
        // PlayerController 자동 검색
        if (playerController == null)
        {
            playerController = GameObject.FindObjectOfType<PlayerController>();
            
            if (playerController != null)
            {
                Debug.Log("[UI] PlayerController 자동 검색 성공!");
            }
            else
            {
                Debug.LogError("[UI] PlayerController를 찾을 수 없습니다!");
            }
        }
    }
    
    void Update()
    {
        // 초기화 안 끝으면 시도
        if (!isInitialized)
        {
            TryInitialize();
            return;
        }
        
        // 초기화 끝으면 업데이트
        UpdateIcons();
    }
    
    /// <summary>
    /// 초기화 시도
    /// - PlayerController.SocketManager가 준비될 때까지 대기
    /// </summary>
    private void TryInitialize()
    {
        // PlayerController 확인
        if (playerController == null)
            return;
        
        // SocketManager 확인
        if (playerController.SocketManager == null)  // ← 변경!
            return;
        
        // 초기화!
        socketManager = playerController.SocketManager;  // ← 변경!
        
        Debug.Log("[UI] SocketManager 할당 완료!");
        
        // Icon Slots 확인
        if (iconSlots.Count == 0)
        {
            Debug.LogError("[UI] Icon Slots가 비어있습니다!");
            return;
        }
        
        Debug.Log($"[UI] Icon Slots: {iconSlots.Count}개");
        
        // 초기 상태
        ClearIcons();
        
        isInitialized = true;
        
        Debug.Log("[UI] 초기화 완료!");
    }
    
    /// <summary>
    /// 아이콘 업데이트
    /// </summary>
    private void UpdateIcons()
    {
        if (socketManager == null)  // ← 변경!
            return;
        
        List<AttackSkillData> history = socketManager.GetComboHistory();  // ← 변경!

        Debug.Log($"[UI] 히스토리 개수: {history.Count}, currentIndex: {socketManager.GetCurrentStep() - 1}");  // ← 변경!
        
        // 히스토리 비어있으면 clear
        if (history.Count == 0)
        {
            Debug.Log("[UI] 히스토리 비어있음 → Clear!"); 
            ClearIcons();
            return;
        }
        
        // 아이콘 표시
        for (int i = 0; i < iconSlots.Count; i++)
        {
            if (i < history.Count)
            {
                AttackSkillData skill = history[i];
                
                if (skill != null && skill.skillIcon != null)
                {
                    // 아이콘 있음
                    iconSlots[i].sprite = skill.skillIcon;
                    iconSlots[i].color = activeColor;
                }
                else
                {
                    // 아이콘 없음 (그래도 활성화)
                    iconSlots[i].sprite = null;
                    iconSlots[i].color = activeColor;
                }
            }
            else
            {
                // 비활성화
                iconSlots[i].sprite = null;
                iconSlots[i].color = inactiveColor;
            }
        }
    }
    
    /// <summary>
    /// 모든 아이콘 비우기
    /// </summary>
    private void ClearIcons()
    {
        Debug.Log($"[UI] === ClearIcons 시작 === Slots: {iconSlots.Count}개");
        
        for (int i = 0; i < iconSlots.Count; i++)
        {
            iconSlots[i].sprite = null;
            iconSlots[i].color = inactiveColor;
            
            Debug.Log($"[UI] Slot{i} Cleared!");
        }
        
        Debug.Log("[UI] === ClearIcons 완료 ===");
    }
}