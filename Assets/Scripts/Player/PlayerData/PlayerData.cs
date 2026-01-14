using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SocketSlotData
{
    public InputTypes assignedInput;
    public AttackSkillData equippedSkill;
}

/// <summary>
/// Player 데이터
/// ScriptableObject로 관리하여 Inspector에서 설정 가능
/// </summary>
[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/PlayerData")]
public class PlayerData : ScriptableObject
{
    // ========================================
    // 기본 정보
    // ========================================

    [Header("기본 정보")]

    public string playerName = "Knight";
    public int total_Level = 10;
    public Sprite character_Sprite; // 플레이어 기본 이미지
    public WeaponData weaponData;

    [Header("스탯 레벨")]
    public int base_Health_Level = 10;
    public int base_Defense_Level = 5;
    public int base_Strength_Level = 10;
    public int base_Dexterity_Level = 6;
    public int base_Agility_Level = 5;
    public int base_Intelligence_Level = 1;
    public int base_Luck_Level = 3;

    public float base_Move_Speed = 5f;

    [Tooltip("회전 속도")]
    public float rotationSpeed = 10f;

    // ========================================
    // 스턴 시스템
    // ========================================

    [Header("스턴 시스템")]
    [Tooltip("최대 스턴 게이지")]
    public float maxStunGauge = 100f;

    [Tooltip("스턴 게이지 회복 속도 (초당)")]
    public float stunRecoveryRate = 10f;

    // ========================================
    // 회피 시스템
    // ========================================

    [Header("회피 시스템")]
    [Tooltip("회피 쿨타임 (초)")]
    public float dodgeCooldown = 1f;

    [Tooltip("회피 지속 시간")]
    public float dodgeDuration = 0.5f;

    // ========================================
    // 콤보 시스템 ← 여기 추가!
    // ========================================

    [Header("콤보 시스템")]
    [Tooltip("사용 가능한 콤보 리스트")]
    public List<ComboData> combos = new List<ComboData>();

    // 소켓
    [Header("소켓 시스템")]
    [Tooltip("저장된 소켓 정보")]
    public List<SocketSlotData> socketSlots = new List<SocketSlotData>();

        [System.NonSerialized]
    private PlayerStats _runtimeStats;
    
    /// <summary>런타임 스탯 접근자</summary>
    public PlayerStats stats
    {
        get
        {
            if (_runtimeStats == null)
            {
                _runtimeStats = new PlayerStats(this);
            }
            return _runtimeStats;
        }
    }

}