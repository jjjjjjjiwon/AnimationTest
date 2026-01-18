using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 적 기본 데이터 (일반 몹 + 보스)
/// ScriptableObject로 적마다 생성
/// </summary>
[CreateAssetMenu(fileName = "New Enemy", menuName = "Game/Enemy Data")]
public class EnemyDataTest : ScriptableObject
{
    [Header("기본 정보")]
    public string enemyName;              // 적 이름
    public EnemyType enemyType;           // 적 일반인지 보스인지
    
    [Header("기본 스탯")]
    public float baseHealth;              // 체력
    public float baseDamage;              // 공격력
    public float baseSpeed;               // 스피드
    public float baseDefense;             // 방어력
    
    [Header("스테이트 관련")]
    public float attackRange;             // 공격 사거리
    public float attackCooldown;          // 공격 쿨타임
    public float detectionRange;          // 감지 범위
    
    [Header("사용 가능한 스테이트")]
    public List<string> availableStates; // ["Idle", "Chase", "Attack"] // 스테이트
}

/// <summary>
/// 적 타입
/// </summary>
public enum EnemyType
{
    Normal,  // 일반 몹
    Boss     // 보스
}