using System.Collections.Generic;
using UnityEngine;

// 스테이지 클리어 조건
public enum ClearConditionType 
{ 
    None, 
    KillAll, 
    KillTarget, 
    Destination, 
    Boss 
}

// 적 소환 조건
public enum SpawnConditionType 
{ 
    None,
    TimeElapsed, 
    KillsReached, 
    BeforeWaveDead, 
    PlayerLowHP 
}

[System.Serializable]
public class StageData
{
    [Header("Stage Info")]
    public int stage_ID;
    public string stage_Name;
    public string stage_Description;
    public int difficulty;

    public string icon_Path;
    public string scene_Name;

    [Header("Boss Info")]
    public bool isBossStage;
    public string boss_ID;
    public string boss_Name;

    [Header("Rewards")]
    public int gold_Reward;
    public int levelUp_Point;
    public string[] item_Rewards;
    public string skill_Reward;

    [Header("Clear Condition")]
    public ClearConditionType clear_Type; // ✅ 복구!
    public int target_KillCount;

    [Header("Spawn List")]
    public List<EnemySpawnInfo> enemy_SpawnList; // ✅ 적 소환 리스트


    [System.NonSerialized] public Sprite stage_Icon; 

}


// 적 소한 정보
[System.Serializable]
public class EnemySpawnInfo
{
    public string enemy_ID;
    public bool isBoss;
    public Vector3 spawn_Pos;
    public float spawn_Rotation;
    public SpawnConditionType condition_Type;
    public float condition_Value;
}