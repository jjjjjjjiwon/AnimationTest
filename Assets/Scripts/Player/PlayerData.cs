using UnityEngine;

/// <summary>
/// Player 설정 데이터
/// ScriptableObject로 Inspector에서 편집 가능
/// </summary>
[CreateAssetMenu(fileName = "PlayerData", menuName = "Player/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Movement")]
    [Tooltip("걷기 속도")]
    public float walkSpeed = 5f;
    
    [Tooltip("달리기 속도")]
    public float runSpeed = 8f;
    
    [Tooltip("회전 속도")]
    public float rotationSpeed = 10f;

    [Header("Dodge")]
    [Tooltip("회피 쿨타임")]
    public float dodgeCooldown = 1f;

    [Header("Resources")]
    [Tooltip("최대 HP")]
    public float maxHP = 100f;
    
    [Tooltip("최대 스턴 게이지")]
    public float maxStunGauge = 100f;

    [Header("Combat")]
    [Tooltip("Perfect 타이밍 성공 시 데미지 배율")]
    public float perfectDamageMultiplier = 1.5f;
    
    [Tooltip("Perfect 타이밍 성공 시 스턴 시간 증가 (초)")]
    public float perfectStunIncrease = 0.3f;
}