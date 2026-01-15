using UnityEngine;

/// <summary>
/// 플레이어 런타임 스탯
/// - PlayerData에서 초기값을 받아옴
/// - 게임 중 변경 가능
/// <summary>
[System.Serializable]
public class PlayerStats
{
    #region 필드

    /// <summary>
    /// 스탯 레벨 (런타임 변경 가능)
    /// </summary>
    public int level;

    public int health_Level;
    public int defense_Level;
    public int strength_Level;
    public int dexterity_Level;
    public int agility_Level;
    public int intelligence_Level;
    public int luck_Level;

    // ========================================
    // 기타 런타임 값
    // ========================================    
    
    public float speed;
    public float current_Health;
    public int availablePoints = 0;

    // ========================================
    // 생성자 (초기값 받아오기)
    // ========================================
    public PlayerStats(PlayerData data)
    {
        level = data.total_Level;
        
        health_Level = data.base_Health_Level;
        defense_Level = data.base_Defense_Level;
        strength_Level = data.base_Strength_Level;
        dexterity_Level = data.base_Dexterity_Level;
        agility_Level = data.base_Agility_Level;
        intelligence_Level = data.base_Intelligence_Level;
        luck_Level = data.base_Luck_Level;

        speed = data.base_Move_Speed;
        current_Health = max_Health;

        Debug.Log($"[PlayerStats] 초기화 완료 - HP: {max_Health}");
    }

    // ========================================
    // 계산된 능력치 (프로퍼티)
    // ========================================

    public float max_Health => (health_Level * 10);
    public float defense => defense_Level * 2;
    public float physicalDamage => 10 + (strength_Level * 5);
    public float dexterityBonus => dexterity_Level * 0.02f;
    public float agilityBonus => agility_Level * 0.02f;
    public float magicDamage => 10 + (intelligence_Level * 8);
    public float luckBonus => luck_Level * 0.05f;

    public float move_Speed => speed + (agility_Level * 0.1f);

    #endregion


    #region 함수

    public void AddPoints(int amount)
    {
        availablePoints += amount;
        Debug.Log($"[스탯] 포인트 +{amount} → 사용 가능: {availablePoints}");
    }

    // ========================================
    // 스탯 투자
    // ========================================

    public bool InvestStat(StatType statType, int amount = 1)
    {
        if (availablePoints < amount)
        {
            Debug.Log("[스탯] 포인트가 부족합니다!");
            return false;
        }

        switch (statType)
        {
            case StatType.Health:
                health_Level += amount;
                Debug.Log($"[스탯] 체력 +{amount} → HP: {max_Health}");
                break;

            case StatType.Defense:
                defense_Level += amount;
                Debug.Log($"[스탯] 방어력 +{amount} → 방어: {defense}");
                break;

            case StatType.Strength:
                strength_Level += amount;
                Debug.Log($"[스탯] 물리 +{amount} → 공격력: {physicalDamage}");
                break;

            case StatType.Dexterity:
                dexterity_Level += amount;
                Debug.Log($"[스탯] 기량 +{amount} → 보너스: {(dexterityBonus * 100):F1}%");
                break;

            case StatType.Agility:
                agility_Level += amount;
                Debug.Log($"[스탯] 민첩 +{amount} → 이속: {move_Speed:F1}");
                break;

            case StatType.Intelligence:
                intelligence_Level += amount;
                Debug.Log($"[스탯] 마법 +{amount} → 마법 공격력: {magicDamage}");
                break;

            case StatType.Luck:
                luck_Level += amount;
                Debug.Log($"[스탯] 력 +{amount} → 드랍률: {(luckBonus * 100):F1}%");
                break;
        }

        availablePoints -= amount;
        return true;
    }

    // ========================================
    // 디버그
    // ========================================

    public void PrintStats()
    {
        Debug.Log($@"
=== 플레이어 스탯 ===
사용 가능 포인트: {availablePoints}

[투자 스탯]
체력: {health_Level} → HP: {max_Health:F0} (현재: {current_Health:F0})
방어: {defense_Level} → 방어력: {defense:F0}
물리: {strength_Level} → 공격력: {physicalDamage:F0}
기량: {dexterity_Level} → 보너스: {(dexterityBonus * 100):F1}%
민첩: {agility_Level} → 이속: {move_Speed:F1}
마법: {intelligence_Level} → 마법 공격력: {magicDamage:F0}
력: {luck_Level} → 드랍률: {(luckBonus * 100):F1}%
");
    }

    #endregion

}

public enum StatType
{
    Health,
    Defense,
    Strength,
    Dexterity,
    Agility,
    Intelligence,
    Luck
}