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
    
    private SocketManager socketManager;
    private bool isInitialized = false;
    
    void Start()
    {
        // PlayerController 자동 검색 (최신 함수 사용 권장)
        if (playerController == null)
        {
            playerController = Object.FindAnyObjectByType<PlayerController>();
        }
    }
    
    void Update()
    {
        if (!isInitialized)
        {
            TryInitialize();
            return;
        }
        
        UpdateIcons();
    }
    
    private void TryInitialize()
    {
        if (playerController == null) return;
        if (playerController.SocketManager == null) return;
        
        socketManager = playerController.SocketManager;
        
        if (iconSlots.Count == 0) return;
        
        ClearIcons();
        isInitialized = true;
    }
    
    private void UpdateIcons()
{
    if (socketManager == null) return;
    
    List<PlayerSkillData> history = socketManager.GetComboHistory();
    
    for (int i = 0; i < iconSlots.Count; i++)
    {
        if (i < history.Count)
        {
            PlayerSkillData skill = history[i];
            
            // ⭐ 경로(string)가 비어있지 않은지 확인
            if (skill != null && !string.IsNullOrEmpty(skill.skill_Icon_Path))
            {
                // Resources 폴더에서 Sprite 로드
                Sprite loadedSprite = Resources.Load<Sprite>(skill.skill_Icon_Path);
                
                if (loadedSprite != null)
                {
                    iconSlots[i].sprite = loadedSprite;
                    iconSlots[i].color = activeColor;
                }
            }
            else
            {
                iconSlots[i].sprite = null;
                iconSlots[i].color = activeColor;
            }
        }
        else
        {
            iconSlots[i].sprite = null;
            iconSlots[i].color = inactiveColor;
        }
    }
}
    
    private void ClearIcons()
    {
        for (int i = 0; i < iconSlots.Count; i++)
        {
            iconSlots[i].sprite = null;
            iconSlots[i].color = inactiveColor;
        }
    }
}