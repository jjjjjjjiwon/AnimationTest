using System;
using System.Collections.Generic;

[Serializable]
public class EnemyDashStateJsonData
{
    public string enemy_Dash_State_ID;                 
    public string enemy_Dash_State_Name;                 
    public string animation_Name;
    public float dash_Speed;
    public float dash_distance;
    public float Cooldown;
}

[Serializable]
public class EnemyDashStateListWrapper
{
    // 중요: JSON 파일의 최상위 Key 이름이 "EnemyChaseState"라면 똑같이 맞춰야 함
    public List<EnemyDashStateJsonData> EnemyDashState;
}