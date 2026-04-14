using System;
using System.Collections.Generic;
using UnityEngine;

// 어떤 종류의 업그레이드인가?
public enum BossUpgradeType { none, stat, ability, removeState }

// 어떤 스탯을 건드리는가?
public enum BossStatType 
{ 
    none, 
    health,   // base_Health와 매칭
    damage,   // base_Damage와 매칭
    speed,    // base_Speed와 매칭
    defense   // base_Defense와 매칭
}

// 어떤 행동(스테이트)을 타겟으로 하는가?
public enum BossActionType { none, idle, chase, attack, dash, teleport, fly, stun, death }

[Serializable]
public class BossUpgradeJsonList
{
    // 중요: JSON의 키값이 "upgrades"이므로 변수명도 똑같이 upgrades여야 합니다!
    public List<BossUpgradeJsonData> upgrades;
}

[Serializable]
public class BossUpgradeJsonData
{
    public string boss_ID;
    public string upgrade_ID;
    public string upgrade_Name;
    public string upgrade_Description;

    public string upgrade_Type; 
    public BossUpgradeType type => Enum.TryParse(upgrade_Type, true, out BossUpgradeType result) ? result : BossUpgradeType.none;

    public string target_Action;
    public BossActionType target_action_type => Enum.TryParse(target_Action, true, out BossActionType result) ? result : BossActionType.none;
    public string ability_ID;

    public string stat_Type; 
    public BossStatType stat_type_enum => Enum.TryParse(stat_Type, true, out BossStatType result) ? result : BossStatType.none;

    public float value;
}