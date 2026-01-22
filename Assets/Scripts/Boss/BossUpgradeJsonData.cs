using System;
using System.Collections.Generic;

public enum BossUpgradeType
{
    Stat,       // 스탯 변경
    AddState,   // 상태/패턴 추가
    RemoveState // 상태/패턴 제거 (옵션)
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
    public string upgradeID;
    public string upgradeName;
    public string upgradeDescription;

    public string targetBossId;          // ""이면 전체 보스 공용도 가능

    public BossUpgradeType type;

    // type == Stat
    public BossStatType stat;
    public float value;

    // type == AddState/RemoveState
    public string stateKey;
}
