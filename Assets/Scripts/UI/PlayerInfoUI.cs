using UnityEngine;
using TMPro;
using UnityEngine.UI;
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

    // Player_Weapon (무기 정보)
    // ========================================

    [Header("무기 정보")]
    [SerializeField] private TextMeshProUGUI weapon_Text; // Player_Weapon_Text
    [SerializeField] private Image weapon_Image; // Player_Character_Image


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

        if (player_Image != null && data.character_Sprite != null)
        {
            player_Image.sprite = data.character_Sprite;
        }


        // 스탯 정보 갱신 추가!
        RefreshStatusInfo();

        // 무기 정보 갱신 (추가!)
        RefreshWeaponInfo();

        Debug.Log($"[정보창] 캐릭터 정보 갱신: {data.playerName}");
    }


    /// <summary>스탯 정보 갱신</summary>
    private void RefreshStatusInfo()
    {
        if (playerController == null || status_Text == null)
            return;

        PlayerData data = playerController.PlayerData;
        PlayerStats stats = data.stats;

        // 레벨 + 스탯 정보
        string statusInfo = $@"Level {data.total_Level}
Available point: {stats.availablePoints}

Health: {stats.health_Level} → HP: {stats.max_Health:F0} (Current: {stats.current_Health:F0})
Defense: {stats.defense_Level} → DF: {stats.defense:F0}
Strength: {stats.strength_Level} → ST: {stats.physicalDamage:F0}
Dexterity: {stats.dexterity_Level} → DT: {(stats.dexterityBonus * 100):F1}%
Agility: {stats.agility_Level} → AL: {stats.move_Speed:F1}
Intelligence: {stats.intelligence_Level} → Magic: {stats.magicDamage:F0}
Luck: {stats.luck_Level} → Luck: {(stats.luckBonus * 100):F1}%";

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

}
