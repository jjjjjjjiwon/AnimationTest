using System;
using System.Collections.Generic;

[Serializable]
public class EnemyStunStateJsonData
{
    public string enemy_Stun_State_ID;                 
    public string enemy_Stun_State_Name;                 
    public string animation_Name;
    public float stun_Timer;
}

[Serializable]
public class EnemyStunStateListWrapper
{
    // 중요: JSON 파일의 최상위 Key 이름이 "EnemyChaseState"라면 똑같이 맞춰야 함
    public List<EnemyStunStateJsonData> EnemyStunState;
}