using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Data.Common;

/// <summary>
/// 플레이어 정보창 UI
/// </summary>
public class PlayerInfoUI : MonoBehaviour
{
    // ========================================
    // 참조
    // ========================================

    [Header("참조")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private GameObject uiPanel; // Player Info 전체 패널

    // ========================================
    // Player_Character (캐릭터 정보)
    // ========================================

    [Header("캐릭터 정보")]
    [SerializeField] private TextMeshProUGUI playerName_Text; // Player_Character_Text
    [SerializeField] private Image player_Image; // Player_Character_Image

    // ========================================
    // Player_Status (스탯 표시)
    // ========================================

    [Header("스탯 정보")]
    [SerializeField] private TextMeshProUGUI status_Text; // Player_Status_Text

    // ========================================
    // Player_Weapon (무기 정보)
    // ========================================

    [Header("무기 정보")]
    [SerializeField] private TextMeshProUGUI weapon_Text; // Player_Weapon_Text
    [SerializeField] private Image weapon_Image; // Player_Character_Image

    // ========================================
    // Player_Item (인벤토리) - 추가!
    // ========================================

    [Header("인벤토리")]
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private Transform inventoryGrid; // Player_Item

    private List<Image> slotIcons = new List<Image>(); // 아이콘 이미지들

    // ========================================
    // UI 상태
    // ========================================

    private static bool isUIOpen = false;
    public static bool IsUIOpen => isUIOpen;

    // ========================================
    // Unity 생명주기
    // ========================================

    void Start()
    {
        // UI 초기화
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }
        isUIOpen = false;

        CreateInventorySlots();
    }

    void Update()
    {

        // I 키로 정보창 열기/닫기
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleUI();
        }
    }

    // ========================================
    // UI 열기/닫기
    // ========================================

    /// <summary>UI 토글</summary>
    public void ToggleUI()
    {
        if (isUIOpen)
        {
            CloseUI();
        }
        else
        {
            OpenUI();
        }
    }

    /// <summary>UI 열기</summary>
    public void OpenUI()
    {
        // Idle이나 Move 상태에서만 열기
        if (playerController != null && !playerController.CanOpenUI())
        {
            Debug.Log("[정보창] 지금은 열 수 없습니다!");
            return;
        }

        isUIOpen = true;

        if (uiPanel != null)
        {
            uiPanel.SetActive(true);
        }

        // 데이터 갱신
        RefreshCharacterInfo();

        Debug.Log("[정보창] 열림");
    }

    /// <summary>UI 닫기</summary>
    public void CloseUI()
    {
        isUIOpen = false;

        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }

        Debug.Log("[정보창] 닫힘");
    }

    // ========================================
    // 데이터 갱신
    // ========================================

    /// <summary>캐릭터 정보 갱신</summary>
    private void RefreshCharacterInfo()
    {
        if (playerController == null)
            return;

        PlayerData data = playerController.PlayerData;

        // 이름 표시
        if (playerName_Text != null)
        {
            playerName_Text.text = data.playerName;
        }

        // 캐릭터 이미지 표시
        if (player_Image != null && data.character_Sprite != null)
        {
            player_Image.sprite = data.character_Sprite;
        }

        // 스탯 정보 갱신
        RefreshStatusInfo();

        // 무기 정보 갱신
        RefreshWeaponInfo();

        // 인벤토리 갱신
        RefreshInventoryInfo();

        Debug.Log($"[정보창] 캐릭터 정보 갱신: {data.playerName}");
    }


    /// <summary>스탯 정보 갱신</summary>
    private void RefreshStatusInfo()
    {
        if (status_Text == null)
            return;

        // ⭐ RuntimeManager에서 가져오기
        PlayerData data = playerController.PlayerData;
        PlayerStats stats = RuntimeManager.Instance.playerStats;

        // 레벨 + 스탯 정보
        string statusInfo = $@"Level {stats.level} +{stats.availablePoints}

▼▲ {stats.health_Level} 체력 → HP: {stats.max_Health:F0} (현재: {stats.current_Health:F0})
▼▲ {stats.defense_Level} 방어 → 방어력: {stats.defense:F0}
▼▲ {stats.strength_Level} 물리 → 공격력: {stats.physicalDamage:F0}
▼▲ {stats.dexterity_Level} 기량 → 보너스: {(stats.dexterityBonus * 100):F1}%
▼▲ {stats.agility_Level} 민첩 → 이동속도: {stats.move_Speed:F1}
▼▲ {stats.intelligence_Level} 마법 → 마법 공격력: {stats.magicDamage:F0}
▼▲ {stats.luck_Level} 럭 → 드랍률: {(stats.luckBonus * 100):F1}%";

        status_Text.text = statusInfo;

        Debug.Log("[정보창] 스탯 정보 갱신");
    }


    /// <summary>무기 정보 갱신</summary>
    private void RefreshWeaponInfo()
    {
        if (playerController == null)
            return;

        PlayerData data = playerController.PlayerData;

        // 무기 정보
        if (data.weaponData != null)
        {
            // 무기 이미지 표시
            if (weapon_Image != null)
            {
                weapon_Image.sprite = data.weaponData.icon;
                weapon_Image.enabled = true;
            }

            // 무기 텍스트 표시
            if (weapon_Text != null)
            {
                string weaponInfo = $@"{data.weaponData.weaponName} Power: {data.weaponData.damage:F0}";

                weapon_Text.text = weaponInfo;
            }
        }
        else
        {
            // 무기 없음
            if (weapon_Image != null)
            {
                weapon_Image.enabled = false;
            }

            if (weapon_Text != null)
            {
                weapon_Text.text = "No Weapon";
            }
        }

        Debug.Log("[정보창] 무기 정보 갱신");
    }

    // ========================================
    // 인벤토리 슬롯 생성 (추가!)
    // ========================================

    /// <summary>인벤토리 슬롯 생성</summary>
    void CreateInventorySlots()
{
    if (itemSlotPrefab == null || inventoryGrid == null)
    {
        Debug.LogWarning("[인벤토리] 프리팹 또는 그리드가 설정되지 않았습니다!");
        return;
    }
    
    // 9개 슬롯 생성
    for (int i = 0; i < 9; i++)
    {
        GameObject slot = Instantiate(itemSlotPrefab, inventoryGrid);
        slot.name = $"ItemSlot_{i}";
        
        // ⭐ 모든 Image 컴포넌트 찾기
        Image[] images = slot.GetComponentsInChildren<Image>();
        
        Debug.Log($"[인벤토리] 슬롯 {i}: Image 개수 = {images.Length}"); // 디버그
        
        if (images.Length >= 2)
        {
            // images[0] = 부모 Image (배경)
            // images[1] = 자식 Image (아이콘)
            Image icon = images[1];
            slotIcons.Add(icon);
            icon.enabled = false; // 처음엔 비어있음
            
            Debug.Log($"[인벤토리] 슬롯 {i}: 아이콘 추가 완료");
        }
        else if (images.Length == 1)
        {
            // Image가 1개만 있으면 그것을 아이콘으로 사용
            Debug.LogWarning($"[인벤토리] 슬롯 {i}: Image가 1개만 있습니다. 이것을 아이콘으로 사용합니다.");
            Image icon = images[0];
            slotIcons.Add(icon);
            icon.enabled = false;
        }
        else
        {
            Debug.LogError($"[인벤토리] 슬롯 {i}: Image 컴포넌트를 찾을 수 없습니다!");
        }
    }
    
    Debug.Log($"[인벤토리] {slotIcons.Count}개 슬롯 생성 완료");
}


    /// <summary>인벤토리 정보 갱신</summary>
    private void RefreshInventoryInfo()
{
    if (RuntimeManager.Instance == null || RuntimeManager.Instance.playerInventory == null)
    {
        Debug.LogWarning("[정보창] RuntimeManager 또는 인벤토리가 null입니다!");
        return;
    }
    
    PlayerInventory inventory = RuntimeManager.Instance.playerInventory;
    
    // 9개 슬롯 갱신
    for (int i = 0; i < slotIcons.Count; i++)
    {
        InventorySlot slot = inventory.GetSlot(i);
        
        if (slot != null && !slot.IsEmpty)
        {
            ItemData item = GameData.Instance.GetItem(slot.itemID);
            
            if (item != null && item.icon != null)
            {
                slotIcons[i].sprite = item.icon;
                slotIcons[i].enabled = true;
                slotIcons[i].color = Color.white;
            }
            else
            {
                // 아이콘 없음 → 색으로 표시
                slotIcons[i].enabled = true;
                slotIcons[i].sprite = null;
                slotIcons[i].color = Color.yellow;
            }
        }
        else
        {
            // 빈 슬롯
            slotIcons[i].enabled = false;
        }
    }
    
}

}
