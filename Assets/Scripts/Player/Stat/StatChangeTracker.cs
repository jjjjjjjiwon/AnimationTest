using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스탯 변경사항을 추적하고 관리하는 클래스
/// - 임시 상태 관리 (▲▼ 버튼)
/// - 원본 보호 (확정된 값보다 낮아질 수 없음)
/// - 확정/취소 기능
/// </summary>
public class StatChangeTracker
{
    // ========================================
    // 확정된 값 (기준값 = 원본 보호 대상)
    // ========================================
    private int confirmedLevel;
    private int confirmedAvailablePoints;
    private Dictionary<StatType, int> confirmedStats;

    // ========================================
    // 임시 값 (작업 중인 값)
    // ========================================
    private int tempLevel;
    private int tempAvailablePoints;
    private Dictionary<StatType, int> tempStats;

    // ========================================
    // 변경 추적 (+3, -2 등)
    // ========================================
    private Dictionary<StatType, int> statChanges;

    // ========================================
    // 원본 참조
    // ========================================
    private PlayerStats originalStats;

    // ========================================
    // 생성자
    // ========================================
    public StatChangeTracker(PlayerStats stats)
    {
        originalStats = stats;

        // 1️⃣ 현재 값을 "확정된 값"으로 저장
        confirmedLevel = stats.level;
        confirmedAvailablePoints = stats.availablePoints;

        confirmedStats = new Dictionary<StatType, int>
        {
            { StatType.Health, stats.health_Level },
            { StatType.Defense, stats.defense_Level },
            { StatType.Strength, stats.strength_Level },
            { StatType.Dexterity, stats.dexterity_Level },
            { StatType.Agility, stats.agility_Level },
            { StatType.Intelligence, stats.intelligence_Level },
            { StatType.Luck, stats.luck_Level }
        };

        // 2️⃣ 확정된 값을 "임시 값"에 복사
        tempLevel = confirmedLevel;
        tempAvailablePoints = confirmedAvailablePoints;
        tempStats = new Dictionary<StatType, int>(confirmedStats);

        // 3️⃣ 변경 추적 초기화 (모두 0)
        statChanges = new Dictionary<StatType, int>
        {
            { StatType.Health, 0 },
            { StatType.Defense, 0 },
            { StatType.Strength, 0 },
            { StatType.Dexterity, 0 },
            { StatType.Agility, 0 },
            { StatType.Intelligence, 0 },
            { StatType.Luck, 0 }
        };

        Debug.Log($"[Tracker] 생성 완료 - Level: {confirmedLevel}, 포인트: {confirmedAvailablePoints}");
    }

    // ========================================
    // 스탯 증가 (▲ 버튼)
    // ========================================
    public bool TryIncreaseStat(StatType type)
    {
        // 포인트 체크
        if (tempAvailablePoints <= 0)
        {
            Debug.Log($"[Tracker] 증가 실패: 포인트 부족 ({tempAvailablePoints})");
            return false;
        }

        // 임시값만 변경 (원본은 그대로!)
        tempStats[type]++;
        tempAvailablePoints--;
        tempLevel++; // 총 레벨도 증가
        statChanges[type]++;

        Debug.Log($"[Tracker] {type} 증가 → {tempStats[type]} (변경: +{statChanges[type]})");
        return true;
    }

    // ========================================
    // 스탯 감소 (▼ 버튼)
    // ========================================
    public bool TryDecreaseStat(StatType type)
    {
        // ⭐ 원본 보호: 확정된 값보다 낮아질 수 없음!
        if (tempStats[type] <= confirmedStats[type])
        {
            Debug.Log($"[Tracker] 감소 실패: 확정값({confirmedStats[type]})보다 낮아질 수 없음");
            return false;
        }

        // 임시값만 변경
        tempStats[type]--;
        tempAvailablePoints++;
        tempLevel--; // 총 레벨도 감소
        statChanges[type]--;

        Debug.Log($"[Tracker] {type} 감소 → {tempStats[type]} (변경: {statChanges[type]})");
        return true;
    }

    // ========================================
    // 변경사항 확정 (확정 버튼)
    // ========================================
    public void ConfirmChanges()
    {
        // 1️⃣ 임시값을 원본 PlayerStats에 적용
        originalStats.level = tempLevel;
        originalStats.availablePoints = tempAvailablePoints;
        originalStats.health_Level = tempStats[StatType.Health];
        originalStats.defense_Level = tempStats[StatType.Defense];
        originalStats.strength_Level = tempStats[StatType.Strength];
        originalStats.dexterity_Level = tempStats[StatType.Dexterity];
        originalStats.agility_Level = tempStats[StatType.Agility];
        originalStats.intelligence_Level = tempStats[StatType.Intelligence];
        originalStats.luck_Level = tempStats[StatType.Luck];

        // 2️⃣ 확정값 갱신 (다음 기준값으로)
        confirmedLevel = tempLevel;
        confirmedAvailablePoints = tempAvailablePoints;
        confirmedStats = new Dictionary<StatType, int>(tempStats);

        // 3️⃣ 변경 추적 초기화 (새로 생성)
statChanges[StatType.Health] = 0;
statChanges[StatType.Defense] = 0;
statChanges[StatType.Strength] = 0;
statChanges[StatType.Dexterity] = 0;
statChanges[StatType.Agility] = 0;
statChanges[StatType.Intelligence] = 0;
statChanges[StatType.Luck] = 0;

        Debug.Log("[Tracker] ✅ 변경사항 확정!");
    }

    // ========================================
    // 변경사항 취소 (취소 버튼)
    // ========================================
    public void CancelChanges()
    {
        // 1️⃣ 임시값을 확정값으로 되돌림
        tempLevel = confirmedLevel;
        tempAvailablePoints = confirmedAvailablePoints;
        tempStats = new Dictionary<StatType, int>(confirmedStats);

        // 2️⃣ 변경 추적 초기화 (새로 생성)
statChanges[StatType.Health] = 0;
statChanges[StatType.Defense] = 0;
statChanges[StatType.Strength] = 0;
statChanges[StatType.Dexterity] = 0;
statChanges[StatType.Agility] = 0;
statChanges[StatType.Intelligence] = 0;
statChanges[StatType.Luck] = 0;

        Debug.Log("[Tracker] ❌ 변경사항 취소!");
    }

    // ========================================
    // 조회 메서드
    // ========================================

    /// <summary>변경사항이 있는지 확인</summary>
    public bool HasChanges()
    {
        foreach (var change in statChanges.Values)
        {
            if (change != 0) return true;
        }
        return false;
    }

    /// <summary>특정 스탯의 변경량 (+3, -2 등)</summary>
    public int GetStatChange(StatType type)
    {
        return statChanges[type];
    }

    /// <summary>임시 레벨</summary>
    public int GetTempLevel() => tempLevel;

    /// <summary>임시 포인트</summary>
    public int GetTempAvailablePoints() => tempAvailablePoints;

    /// <summary>임시 스탯</summary>
    public int GetTempStat(StatType type) => tempStats[type];

    /// <summary>확정된 스탯</summary>
    public int GetConfirmedStat(StatType type) => confirmedStats[type];

    /// <summary>임시 스탯으로 계산된 보너스를 가진 PlayerStats 복사본 생성</summary>

}