using System.Collections.Generic;

public class EnemyDataPackage
{
    // [1. 본체 데이터]
    // 보스면 bossData가, 일반 적이면 baseData가 채워집니다.
    public EnemyJsonData baseData;          
    public BossJsonData bossData;           

    // [2. 이동/상태 데이터]
    // 팩토리가 최종 확정된 ID로 찾은 '실물 데이터'들입니다.
    public EnemyIdleStateJsonData idleData; 
    public EnemyChaseStateJsonData chaseData;
    public EnemyDashStateJsonData dashData;      // 업그레이드 반영된 실물
    public EnemyTeleportStateJsonData teleportData; // 업그레이드 반영된 실물
    public EnemyStunStateJsonData stunData;
    public EnemyDeathStateJsonData deathData;

    // [3. 공격 데이터]
    public List<EnemyComboJsonData> comboList = new List<EnemyComboJsonData>();
    public Dictionary<string, EnemyAttackMotionJsonData> motionDic = new Dictionary<string, EnemyAttackMotionJsonData>();

    // [4. 최종 계산된 스탯 (선택 사항)]
    // 컨트롤러에서 계산하기 싫다면 팩토리가 계산해서 여기에 넣어주면 됩니다.
    public float finalMaxHP;
    public float finalMoveSpeed;
    public float finalDamage;
    public float finalDefense;
}