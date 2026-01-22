using System;
using System.Collections.Generic;

[Serializable]
public class BossJsonList
{
    public List<BossJsonData> boss;
}

[Serializable]
public class BossJsonData
{
    public string bossId;
    public string bossName;
    public string prefabPath;

    public int baseHp;
    public int baseDamage;
    public float baseMoveSpeed;
    public int baseArmor;

    public List<string> abilityKeys;
    public List<string> patternKeys;
}
