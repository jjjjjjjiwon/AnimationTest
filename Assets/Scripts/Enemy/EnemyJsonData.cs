using System;
using System.Collections.Generic;

public enum Enemystate
{
    none,
    idle,
    chase,
    dash,
    teleport,
    attack,
    stun,
    Death,
}

[Serializable]
public class EnemyJsonData : IEnemyData
{
    public float vision_Angle;  // 아직 안함

    // [1. 데이터용 변수] - JSON의 키값과 토씨 하나 안 틀리게!
    public string enemy_ID;      // JSON: "enemy_ID"
    public string enemy_Name;    // JSON: "enemy_Name"
    public string prefab_Path;   // JSON: "prefab_Path"

    public float base_Health;
    public float base_Damage;
    public float base_Speed;
    public float base_Defense;
    public float detect_Range;

    public string enemy_Idle_State_ID;
    public string enemy_Chase_State_ID;
    public string enemy_Dash_State_ID;
    public string enemy_Teleport_State_ID;
    public List<string> enemy_Combo_ID;
    public string enemy_Stun_State_ID;
    public string enemy_Death_State_ID;

    // [2. 인터페이스 연결] - 팩토리가 사용할 통로

    string IEnemyData.enemy_ID { get => enemy_ID; set => enemy_ID = value; }
    string IEnemyData.enemy_Name { get => enemy_Name; set => enemy_Name = value; }
    string IEnemyData.prefab_Path { get => prefab_Path; set => prefab_Path = value; }
    float IEnemyData.base_Health { get => base_Health; set => base_Health = value; }
    float IEnemyData.base_Damage { get => base_Damage; set => base_Damage = value; }
    float IEnemyData.base_Speed { get => base_Speed; set => base_Speed = value; }
    float IEnemyData.base_Defense { get => base_Defense; set => base_Defense = value; }
    string IEnemyData.enemy_Idle_State_ID { get => enemy_Idle_State_ID; set => enemy_Idle_State_ID = value; }
    string IEnemyData.enemy_Chase_State_ID { get => enemy_Chase_State_ID; set => enemy_Chase_State_ID = value; }
    string IEnemyData.enemy_Dash_State_ID { get => enemy_Dash_State_ID; set => enemy_Dash_State_ID = value; }
    string IEnemyData.enemy_Teleport_State_ID { get => enemy_Teleport_State_ID; set => enemy_Teleport_State_ID = value; }
    List<string> IEnemyData.enemy_Combo_ID { get => enemy_Combo_ID; set => enemy_Combo_ID = value; }
    string IEnemyData.enemy_Stun_State_ID { get => enemy_Stun_State_ID; set => enemy_Stun_State_ID = value; }
    string IEnemyData.enemy_Death_State_ID { get => enemy_Death_State_ID; set => enemy_Death_State_ID = value; }
}