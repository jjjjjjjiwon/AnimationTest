using System;
using UnityEngine;
using System.Collections.Generic;

public enum SkillType // 보통 C# 관례상 첫 글자는 대문자로 씁니다.
{
    None,
    Attack,
    Dash
}

public enum SkillRarity // 보통 C# 관례상 첫 글자는 대문자로 씁니다.
{
    None,
    Common,
    Rare,
    Epic,
    Legend,
}

[Serializable]
public class PlayerSkillData
{
    [Header("1. Identification & UI")]
    public string player_Skill_ID;          // 고유 ID (예: ATK_SWORD_01)
    public SkillType type;              // 스킬 타입
    public string skill_Name;               // 표시될 이름
    [TextArea]                              // 인스펙터에서 글쓰기 편하게 변경
    public string skill_Description;        // 스킬 설명
    public string skill_Icon_Path;          // 아이콘 리소스 경로
    public SkillRarity Rarity;                 // 희귀도
    public int price;                       // 상점 가격

    [Header("2. Animation & Control")]
    public string animation_Name;           // 실행할 애니메이션 스테이트 이름
    public float coolTime;                  // 재사용 대기 시간
    public float post_Delay;                // 액션 종료 후 후딜레이 (콤보 실패 시 등)
    public Vector3 arrival_Direction;       // 이동/공격 시 바라볼 방향 혹은 오프셋z

    [Header("3. Attack Parameters")]
    public float skill_Damage;                // 기본 데미지
    public float skill_Stun_Duration;         // 추가 스턴 시간
    [Space(5)]
    public float perfect_Start = 0.5f;      // 퍼펙트 타이밍 시작 (0~1)
    public float perfect_End = 0.7f;        // 퍼펙트 타이밍 종료 (0~1)
    public float perfect_Damage_Mult = 1.5f;// 퍼펙트 성공 시 데미지 배율
    public float perfect_Stun_Add = 20f;    // 퍼펙트 성공 시 추가 스턴치

    [Header("4. Dash & Movement")]
    public float skill_Speed;               // 이동 속도
    public float skill_Distance;            // 이동 거리 (공격 시 전진 거리로도 활용)
    public bool skill_IsInvincible;         // 무적 여부
}

[System.Serializable]
public class PlayerSkillDataListWrapper
{
    public List<PlayerSkillData> PlayerSkills; 
}