using UnityEngine;

[System.Serializable]
public class BossUpgrade
{
    [Header("기본 정보")]
    public string upgradeID;              // "health_20"
    public string upgradeName;            // "체력 강화"

    [TextArea(2, 3)]
    public string upgradeDescription;     // "보스 체력 +20%"

    [Header("강화 타입")]
    public string upgradeType;            // "stat", "ability", "none"

    [Header("스탯 강화")]
    public string statType;               // "health", "damage", "speed"
    public float value;                   // 0.2 = +20%

    [Header("능력 강화")]
    public string abilityID;              // "teleport"
}
