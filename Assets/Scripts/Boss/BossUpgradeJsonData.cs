using System;
using System.Collections.Generic;
using UnityEngine;

public enum BossUpgradeType
{
    none,
    Stat,       // 스탯 변경
    ability,   // 상태/패턴 추가
    removeState // 상태/패턴 제거 (옵션)
}

public enum BossStatType
{
    Hp,
    Damage,
    MoveSpeed,
    Armor
}

[Serializable]
public class BossUpgradeJsonList
{
    public List<BossUpgradeJsonData> upgrades;
}

[Serializable]
public class BossUpgradeJsonData
{
    public string bossId;       // ""이면 전체 보스 공용도 가능

    public string upgradeID;
    public string upgradeName;
    public string upgradeDescription;

// JSON의 문자열을 그대로 받아오는 변수
    [SerializeField] private string upgradeType; 

    // 코드에서 실제로 사용할 안전한 변수 (Enum 변환)
    public BossUpgradeType Type
    {
        get
        {
            if (string.IsNullOrEmpty(upgradeType)) return BossUpgradeType.none;
            
            // 문자열을 Enum으로 변환 (대소문자 무시)
            if (Enum.TryParse(upgradeType, true, out BossUpgradeType result))
                return result;
            
            return BossUpgradeType.none;
        }
    }

    // type == Stat
    public BossStatType statType;
    public float value;

    // type == AddState/RemoveState
    public string abilityID;
}
