using System;

[Serializable]
public class EnemyAttackMotionJsonData
{
    public string attack_motion_ID;         // 예: "Z_Atk_01" (콤보에서 참조할 고유 ID)
    public string animation_Name;    // 실제 실행할 애니메이션 트리거/이름
    public float damage;            // 이 동작의 공격력
    public float combo_Start_Range;   // 공격을 '시작'하는 최소 거리 (예: 3.0)
    public float combo_Release_Range; // 공격을 '포기/해제'하는 최대 거리 (예: 7.0)
    public float rotation_Speed;     // 이 동작 중 타겟을 따라가는 회전 속도\
    public float add_Stun_Timer;
}

[Serializable]
public class EnemyAttackMotionListWrapper
{
    public System.Collections.Generic.List<EnemyAttackMotionJsonData> enemyAttackMotion;
}