using System.Collections.Generic;

public interface IEnemyData
{
    string enemy_ID { get; set; }
    string enemy_Name { get; set; }
    string prefab_Path { get; set; }
    float base_Health { get; set; }
    float base_Damage { get; set; }
    float base_Speed { get; set; }
    float base_Defense { get; set; }
    string enemy_Idle_State_ID { get; set; }
    string enemy_Chase_State_ID { get; set; }
    string enemy_Dash_State_ID { get; set; }
    string enemy_Teleport_State_ID { get; set; }
    List<string> enemy_Combo_ID { get; set; }
    string enemy_Stun_State_ID { get; set; }
    string enemy_Death_State_ID { get; set; }
}