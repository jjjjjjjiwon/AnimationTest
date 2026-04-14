using System;
using System.Collections.Generic;

[Serializable]
public class BossJsonList
{
    public List<BossJsonData> boss;
}

public enum Bossstate 
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
public class BossJsonData : IEnemyData
{
    // 1. JSON 파일의 키값과 정확히 일치하는 [공개 필드]
    public string boss_ID;
    public string boss_Name;
    public string prefab_Path; // 이제 데이터가 여기로 들어옵니다.

    public float base_Health;
    public float base_Damage;
    public float base_Speed;
    public float base_Defense;

    public string enemy_Idle_State_ID;
    public string enemy_Chase_State_ID;
    public string enemy_Dash_State_ID;
    public string enemy_Teleport_State_ID;
    public List<string> enemy_Combo_ID;
    public string enemy_Stun_State_ID;
    public string enemy_Death_State_ID;

    // 2. IEnemyData 인터페이스 구현 (프로퍼티 에러 해결)
    // 인터페이스가 요구하는 프로퍼티들을 아래 필드들과 연결해줍니다.
    string IEnemyData.enemy_ID { get => boss_ID; set => boss_ID = value; }
    string IEnemyData.enemy_Name { get => boss_Name; set => boss_Name = value; }
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
