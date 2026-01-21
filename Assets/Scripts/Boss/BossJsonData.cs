using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class BossJsonList
{
    public List<BossJsonData> bosses;
}

[System.Serializable]
public class BossJsonData
{
    public string bossId;
    public string bossName;
    public string prefabPath;

    public float maxHp;
    public float damage;
    public float moveSpeed;
    public float armor;

    public List<string> abilityKeys;
    public List<string> patternKeys;
}
