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
    [Tooltip("스킬 이름 (UI 표시용)")]
    public string skillName;
    
    [Tooltip("스킬 아이콘 (UI용)")]
    public Sprite icon;
    
    // ========================================
    // 애니메이션
    // ========================================
    
    [Header("애니메이션")]
    [Tooltip("재생할 애니메이션 이름")]
    public string animationName;
    
    [Tooltip("공격 지속 시간 (초)")]
    public float duration = 0.8f;
    
    [Tooltip("공격 종료 후 대기 시간 (초)")]
    public float exitTime = 0.2f;
    
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
    
    // ========================================
    // 계산 프로퍼티
    // ========================================
    
    /// <summary>총 시간 (duration + exitTime)</summary>
    public float TotalTime => duration + exitTime;
}