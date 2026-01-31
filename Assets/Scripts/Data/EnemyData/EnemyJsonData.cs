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
public class EnemyJsonData
{
    public string enemy_ID;                     // 적 Id
    public string enemy_Name;                   // 적 이름

    public float base_Health;                   // 체력
    public float base_Damage;                   // 공격력
    public float base_Speed;                    // 스피드
    public float base_Defense;                  // 방어력
    
    // 유니티 오브젝트 대신 프리펩 경로를 저장 (Resources/Prefabs/Enemy1.prefab 등)
    public string prefab_Path; 

    public string Enemy_Idle_State_ID;   
    public string Enemy_Chase_State_ID;     
    public string Enemy_Dash_State_ID;     
    public string Enemy_Teleport_State_ID;     
    public string Enemy_Attack_State_ID;  
    public string Enemy_Stun_State_ID;     
    public string Enemy_Death_State_ID;     

}
