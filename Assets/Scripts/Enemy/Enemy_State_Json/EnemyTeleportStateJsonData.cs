using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyTeleportStateJsonData
{
    public string enemy_Teleport_State_ID;                 
    public string enemy_Teleport_State_Name;          

    public string start_Animation_Name;
    public string end_Animation_Name;

    public float max_Trigger_Distance;    
    public float min_Teleport_Distance;
    public Vector3 arrival_Direction;
    public float arrival_Distance;

    public float cooldown;
}

[Serializable]
public class EnemyTeleportStateListWrapper
{
    // 중요: JSON 파일의 최상위 Key 이름이 "EnemyChaseState"라면 똑같이 맞춰야 함
    public List<EnemyTeleportStateJsonData> EnemyTeleportState;
}