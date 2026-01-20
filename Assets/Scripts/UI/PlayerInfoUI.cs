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
    private StatChangeTracker tracker;
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
    [SerializeField] private TextMeshProUGUI status_Total_Level_Text;
    [SerializeField] private TextMeshProUGUI status_stat_Level_Text;
    [SerializeField] private TextMeshProUGUI status_Detail_Stat_Text;

    [SerializeField] private Button confirmButton;  // 확정 버튼

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

        // ⭐ 확정 버튼 이벤트 연결
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirmButtonClick);
        }

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
    // ⭐ RuntimeManager 체크 추가!
    if (RuntimeManager.Instance == null || RuntimeManager.Instance.playerStats == null)
    {
        Debug.LogWarning("[PlayerInfoUI] RuntimeManager가 아직 초기화되지 않았습니다!");
        return;
    }
    
    PlayerController pc = FindObjectOfType<PlayerController>();
    if (pc != null && !pc.CanOpenUI())
    {
        Debug.Log("지금은 UI를 열 수 없습니다!");
        return;
    }

    isUIOpen = true;

    if (uiPanel != null)
    {
        uiPanel.SetActive(true);
    }

    // ⭐ Tracker 생성
    tracker = new StatChangeTracker(RuntimeManager.Instance.playerStats);
    Debug.Log("[정보창] StatChangeTracker 생성");

    // ========== 추가: 씬별 확정 버튼 제어 ==========
    if (confirmButton != null)
    {
        // 로비에서만 확정 버튼 활성화
        bool isLobby = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Lobby");
        confirmButton.interactable = isLobby;

        if (!isLobby)
        {
            Debug.Log("[정보창] 스테이지에서는 스탯 투자 불가 (보기만 가능)");
        }
    }
    // =============================================

    // 데이터 갱신
    RefreshCharacterInfo();

    Debug.Log("[정보창] 열림");
}

    /// <summary>UI 닫기</summary>
    public void CloseUI()
    {
        // ⭐ 미확정 변경사항이 있으면 자동 취소
        if (tracker != null && tracker.HasChanges())
        {
            tracker.CancelChanges();
            Debug.Log("[정보창] 미확정 변경사항 자동 취소");
        }

        isUIOpen = false;

        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }

        tracker = null;

        Debug.Log("[정보창] 닫힘");
    }

    // ========================================
    // 데이터 갱신
    // ========================================

    /// <summary>캐릭터 정보 갱신</summary>
    private void RefreshCharacterInfo()
    {
        if (RuntimeManager.Instance == null || RuntimeManager.Instance.playerData == null)
        {
            Debug.LogWarning("[정보창] PlayerData를 찾을 수 없습니다!");
            return;
        }

        PlayerData data = RuntimeManager.Instance.playerData;
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
        LevelInfo();
        RefreshStatusInfo();
        DetailStatInfo();

        // 무기 정보 갱신
        RefreshWeaponInfo();

        // 인벤토리 갱신
        RefreshInventoryInfo();

        Debug.Log($"[정보창] 캐릭터 정보 갱신: {data.playerName}");
    }


    #region Stat

private void LevelInfo()
{
    int level, points;

    if (tracker != null)
    {
        level = tracker.GetTempLevel();
        points = tracker.GetTempAvailablePoints();
    }
    else
    {
        // ⭐ RuntimeManager null 체크 추가!
        if (RuntimeManager.Instance == null || RuntimeManager.Instance.playerStats == null)
        {
            Debug.LogWarning("[PlayerInfoUI] RuntimeManager 또는 playerStats가 없습니다!");
            status_Total_Level_Text.text = "Level ? + ?";
            return;
        }
        
        PlayerStats stats = RuntimeManager.Instance.playerStats;
        level = stats.level;
        points = stats.availablePoints;
    }

    status_Total_Level_Text.text = $"Level {level} + {points}";
}
    private void RefreshStatusInfo()
    {
        if (status_stat_Level_Text == null)
            return;

        // ⭐ Tracker가 있으면 임시값 + 색상 표시
        if (tracker != null)
        {
            int health = tracker.GetTempStat(StatType.Health);
            int defense = tracker.GetTempStat(StatType.Defense);
            int strength = tracker.GetTempStat(StatType.Strength);
            int dexterity = tracker.GetTempStat(StatType.Dexterity);
            int agility = tracker.GetTempStat(StatType.Agility);
            int intelligence = tracker.GetTempStat(StatType.Intelligence);
            int luck = tracker.GetTempStat(StatType.Luck);

            int healthChange = tracker.GetStatChange(StatType.Health);
            int defenseChange = tracker.GetStatChange(StatType.Defense);
            int strengthChange = tracker.GetStatChange(StatType.Strength);
            int dexterityChange = tracker.GetStatChange(StatType.Dexterity);
            int agilityChange = tracker.GetStatChange(StatType.Agility);
            int intelligenceChange = tracker.GetStatChange(StatType.Intelligence);
            int luckChange = tracker.GetStatChange(StatType.Luck);

            status_stat_Level_Text.text = $@"체력: {FormatStatValue(health, healthChange)}
방어: {FormatStatValue(defense, defenseChange)}
물리: {FormatStatValue(strength, strengthChange)}
기량: {FormatStatValue(dexterity, dexterityChange)}
민첩: {FormatStatValue(agility, agilityChange)}
마법: {FormatStatValue(intelligence, intelligenceChange)}
운: {FormatStatValue(luck, luckChange)}";
        }
        else
        {
            // 원본값 표시
            PlayerStats stats = RuntimeManager.Instance.playerStats;
            status_stat_Level_Text.text = $@"체력: {stats.health_Level}
방어: {stats.defense_Level}
물리: {stats.strength_Level}
기량: {stats.dexterity_Level}
민첩: {stats.agility_Level}
마법: {stats.intelligence_Level}
운: {stats.luck_Level}";
        }

        Debug.Log("[정보창] 스탯 정보 갱신");
    }



    private void DetailStatInfo()
    {
        if (status_Detail_Stat_Text == null)
            return;

        PlayerStats stats = RuntimeManager.Instance.playerStats;

        if (tracker != null)
        {
            int health = tracker.GetTempStat(StatType.Health);
            int defense = tracker.GetTempStat(StatType.Defense);
            int strength = tracker.GetTempStat(StatType.Strength);
            int dexterity = tracker.GetTempStat(StatType.Dexterity);
            int agility = tracker.GetTempStat(StatType.Agility);
            int intelligence = tracker.GetTempStat(StatType.Intelligence);
            int luck = tracker.GetTempStat(StatType.Luck);

            // 변경량 가져오기
            int healthChange = tracker.GetStatChange(StatType.Health);
            int defenseChange = tracker.GetStatChange(StatType.Defense);
            int strengthChange = tracker.GetStatChange(StatType.Strength);
            int dexterityChange = tracker.GetStatChange(StatType.Dexterity);
            int agilityChange = tracker.GetStatChange(StatType.Agility);
            int intelligenceChange = tracker.GetStatChange(StatType.Intelligence);
            int luckChange = tracker.GetStatChange(StatType.Luck);

            // 보너스 계산
            float maxHealth = health * 10;
            float defenseValue = defense * 2;
            float physicalDamage = 10 + (strength * 5);
            float dexterityBonus = dexterity * 0.02f;
            float agilityBonus = agility * 0.02f;
            float magicDamage = 10 + (intelligence * 8);
            float luckBonus = luck * 0.05f;

            // ⭐ FormatStatValue 사용해서 색상 적용
            status_Detail_Stat_Text.text = $@"체력: {FormatStatValue(maxHealth, healthChange)}
방어력: {FormatStatValue(defenseValue, defenseChange)}
물리력: {FormatStatValue(physicalDamage, strengthChange)}
기량: {FormatStatValue(dexterityBonus, dexterityChange)}
민첩: {FormatStatValue(agilityBonus, agilityChange)}
마법력: {FormatStatValue(magicDamage, intelligenceChange)}
운: {FormatStatValue(luckBonus, luckChange)}";
        }
        else
        {
            status_Detail_Stat_Text.text = $@"체력: {stats.max_Health:F0}
방어력: {stats.defense:F0}
물리력: {stats.physicalDamage:F0}
기량: {stats.dexterityBonus:F2}
민첩: {stats.agilityBonus:F2}
마법력: {stats.magicDamage:F0}
운: {stats.luckBonus:F2}";
        }
    }

    /// <summary>스탯 값 포맷</summary>
    private string FormatStatValue(float value, int change)
    {
        if (change > 0)
        {
            if (value < 1)
                return $"<color=green>{value:F2}</color>";
            else
                return $"<color=green>{value:F0}</color>";
        }
        else
        {
            if (value < 1)
                return $"{value:F2}";
            else
                return $"{value:F0}";
        }
    }

    /// <summary>스탯을 변경량과 함께 포맷팅</summary>
    private string FormatStatWithChange(int value, int change)
    {
        if (change > 0)
        {
            return $"{value} <color=green>(+{change})</color>";
        }
        else if (change < 0)
        {
            return $"{value} <color=red>({change})</color>";
        }
        else
        {
            return value.ToString();
        }
    }

    /// <summary>스탯 증가 (▲ 버튼)</summary>
    public void TryLevelUp(StatType type)
    {
        if (tracker == null)
        {
            Debug.LogWarning("[정보창] Tracker가 없습니다!");
            return;
        }

        // ⭐ Tracker를 통해 임시 증가
        if (tracker.TryIncreaseStat(type))
        {
            // UI 갱신
            LevelInfo();
            RefreshStatusInfo();
            DetailStatInfo();
        }
    }

    /// <summary>스탯 감소 (▼ 버튼)</summary>
    public void TryLevelDown(StatType type)  // ⭐ Dwon → Down
    {
        if (tracker == null)
        {
            Debug.LogWarning("[정보창] Tracker가 없습니다!");
            return;
        }

        // ⭐ Tracker를 통해 임시 감소
        if (tracker.TryDecreaseStat(type))
        {
            // UI 갱신
            LevelInfo();
            RefreshStatusInfo();
            DetailStatInfo();
        }
    }

    /// <summary>확정 버튼 클릭</summary>
    private void OnConfirmButtonClick()
    {
        if (tracker == null)
        {
            Debug.LogWarning("[정보창] Tracker가 없습니다!");
            return;
        }

        if (!tracker.HasChanges())
        {
            Debug.Log("[정보창] 변경사항이 없습니다.");
            return;
        }

        // ✅ 변경사항 확정
        tracker.ConfirmChanges();

        // UI 갱신
        LevelInfo();
        RefreshStatusInfo();
        DetailStatInfo();

        Debug.Log("[정보창] ✅ 스탯 변경 확정!");
    }


    #endregion


    #region Weapon

    /// <summary>무기 정보 갱신</summary>
    private void RefreshWeaponInfo()
    {
        if (RuntimeManager.Instance == null || RuntimeManager.Instance.playerData == null)
        {
            Debug.LogWarning("[정보창] PlayerData를 찾을 수 없습니다!");
            return;
        }

        PlayerData data = RuntimeManager.Instance.playerData;

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

    #endregion


    #region Inventory

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

    #endregion

}
