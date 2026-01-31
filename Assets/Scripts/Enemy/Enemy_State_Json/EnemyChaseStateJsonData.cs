using System;
using System.Collections.Generic;

[Serializable]
public class EnemyChaseStateJsonData
{
    public string Enemy_Chase_State_ID;                 
    public string Enemy_Chase_State_Name;                 
    public string animation_Name;
    public float chaseSpeed;
}

[Serializable]
public class EnemyChaseStateListWrapper
{
    // 중요: JSON 파일의 최상위 Key 이름이 "EnemyChaseState"라면 똑같이 맞춰야 함
    public List<EnemyChaseStateJsonData> EnemyChaseState;
}