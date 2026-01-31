using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Enemy_State
{
    idle,
    chase,
    dash,
    teleport,
    attack,
    stun,
    Death,
}

public class Enemydata : ScriptableObject
{
    public string enemy_Id;                     // 적 Id
    public string enemy_Name;                   // 적 이름

    public float base_Health;                   // 체력
    public float base_Damage;                   // 공격력
    public float base_Speed;                    // 스피드
    public float base_Defense;                  // 방어력

    public object prefab;                       // 프리펩



    public string idle_State_Animaion_Id;   

    public string chase_State_Animaion_Id;     
    public string dash_State_Animaion_Id;     
    public string teleport_State_Animaion_Id;     

    public string attack_State_Animaion_Id; 

    public string stu_State_Animaion_Id;      
    public string Death_State_Animaion_Id;    

}
