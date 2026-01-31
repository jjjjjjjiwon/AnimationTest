using System;
using System.Collections.Generic;


[Serializable]
public class EnemyIdleStateJsonData
{
    public string Enemy_Idle_State_ID;                 
    public string Enemy_Idle_State_Name;                 
    public string animation_Name;
}

[Serializable]
public class EnemyIdleStateListWrapper
{
    // 중요: JSON의 최상위 키 "EnemyIdleState"와 정확히 일치해야 함
    public List<EnemyIdleStateJsonData> EnemyIdleState;
}