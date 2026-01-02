using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Player 데이터
/// ScriptableObject로 관리하여 Inspector에서 설정 가능
/// </summary>
[CreateAssetMenu(fileName = "PlayerData", menuName = "Data/PlayerData")]
public class PlayerData : ScriptableObject
{
    // ========================================
    // 기본 능력치
    // ========================================
    
    [Header("기본 능력치")]
    [Tooltip("최대 체력")]
    public float maxHealth = 100f;
    
    [Tooltip("현재 체력 (런타임 변경)")]
    public float currentHealth = 100f;
    
    [Tooltip("이동 속도")]
    public float moveSpeed = 5f;
    
    [Tooltip("회전 속도")]
    public float rotationSpeed = 10f;
    
    // ========================================
    // 전투 능력치
    // ========================================
    
    [Header("전투 능력치")]
    [Tooltip("기본 공격력")]
    public float attackPower = 10f;
    
    [Tooltip("방어력")]
    public float defense = 5f;
    
    // ========================================
    // 스턴 시스템
    // ========================================
    
    [Header("스턴 시스템")]
    [Tooltip("최대 스턴 게이지")]
    public float maxStunGauge = 100f;
    
    [Tooltip("현재 스턴 게이지 (런타임 변경)")]
    public float currentStunGauge = 100f;
    
    [Tooltip("스턴 게이지 회복 속도 (초당)")]
    public float stunRecoveryRate = 10f;
    
    // ========================================
    // 회피 시스템
    // ========================================
    
    [Header("회피 시스템")]
    [Tooltip("회피 쿨타임 (초)")]
    public float dodgeCooldown = 1f;
    
    [Tooltip("회피 거리")]
    public float dodgeDistance = 5f;
    
    [Tooltip("회피 지속 시간")]
    public float dodgeDuration = 0.5f;
    
    // ========================================
    // 콤보 시스템 ← 여기 추가!
    // ========================================
    
    [Header("콤보 시스템")]
    [Tooltip("사용 가능한 콤보 리스트")]
    public List<ComboData> combos = new List<ComboData>();
}