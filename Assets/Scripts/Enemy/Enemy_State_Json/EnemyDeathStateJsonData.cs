using System;
using System.Collections.Generic;

[Serializable]
public class EnemyDeathStateJsonData
{
    public string enemy_Death_State_ID;                 
    public string enemy_Death_State_Name;                 
    public string animation_Name;
}

[Serializable]
public class EnemyDeathStateListWrapper
{
    // 중요: JSON 파일의 최상위 Key 이름이 "EnemyChaseState"라면 똑같이 맞춰야 함
    public List<EnemyDeathStateJsonData> EnemyDeathState;
}