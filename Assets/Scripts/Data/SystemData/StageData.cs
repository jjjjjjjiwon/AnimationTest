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
    public int stageID;
    public string stageName;
    public string stageDescription;
    public string iconPath;
    public int difficulty;
    public string sceneName;

    [Header("Boss Info")]
    public bool isBossStage;
    public string bossId;
    public string bossName;

    [Header("Rewards")]
    public int goldReward;
    public int levelUpPoint;
    public string[] itemRewards;
    public string skillReward;

    [Header("Clear Condition")]
    public ClearConditionType clearType; // ✅ 복구!
    public int targetKillCount;

    [Header("Spawn List")]
    public List<EnemySpawnInfo> enemySpawnList; // ✅ 적 소환 리스트

    [System.NonSerialized] public Sprite stageIcon; 
}


// 적 소한 정보
[System.Serializable]
public class EnemySpawnInfo
{
    public string enemy_ID;
    public Vector3 spawnPos;
    public float spawnRotation;
    public SpawnConditionType conditionType;
    public float conditionValue;
}