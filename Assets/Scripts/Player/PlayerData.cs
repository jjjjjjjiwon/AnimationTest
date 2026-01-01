using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Player/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Movement")]
    [Tooltip("이동 속도 (달리기)")]
    public float moveSpeed = 8f;  // walkSpeed 제거, runSpeed → moveSpeed
    
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