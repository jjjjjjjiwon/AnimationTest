using UnityEngine;

/// <summary>
/// 공격 스킬 데이터
/// - 플레이어가 소켓에 장착할 공격 스킬
/// - 스킬 자체의 정보만 포함
/// </summary>
[CreateAssetMenu(fileName = "NewAttackSkill", menuName = "Combat/Attack Skill")]
public class AttackSkillData : ScriptableObject
{
    // ========================================
    // 기본 정보
    // ========================================

    [Header("기본 정보")]
    public string skillName;
    public Sprite skillIcon;

    // ========================================
    // 애니메이션
    // ========================================

    [Header("애니메이션")]
    public string animationName;

    [Header("타이밍")]
    public float duration = 0.8f;
    public float exitTime = 0.2f;

    [Header("타격 타이밍")]
    [Range(0f, 1f)]
    [Tooltip("스킬 지속 시간(duration) 중 몇 % 지점에서 타격? (0~1)")]
    public float hitboxTiming = 0.5f;  // ← 추가!

    // ========================================
    // 데미지
    // ========================================

    [Header("데미지")]
    [Tooltip("스킬 기본 데미지")]
    public float baseDamage = 10f;

    // ========================================
    // 효과
    // ========================================

    [Header("효과")]
    [Tooltip("적 스턴 지속 시간 (초)")]
    public float stunDuration = 0.5f;

    [Header("Combo Rhythm Timing (0.0 ~ 1.0)")]
    public float perfectStart = 0.5f; // 퍼펙트 시작 (예: 애니메이션 50% 지점)
    public float perfectEnd = 0.7f;   // 퍼펙트 종료 (예: 애니메이션 70% 지점)

    [Header("Rhythm Rewards")]
    public float perfectDamageMult = 1.5f; // 퍼펙트 시 데미지 배율
    public float perfectStunAdd = 20f;     // 퍼펙트 시 추가 스턴값

    // ========================================
    // 계산 프로퍼티
    // ========================================

    /// <summary>총 시간 (duration + exitTime)</summary>
    public float TotalTime => duration + exitTime;

    /// <summary>타격 시점 (초 단위)</summary>
    public float HitboxTime => duration * hitboxTiming;
}