using UnityEngine;
using System.Collections.Generic;


/// <summary>
/// Enemy의 설정 데이터
/// ScriptableObject로 관리하여 Inspector에서 설정 가능
/// 여러 Enemy가 같은 데이터를 공유하거나 각자 다른 데이터 사용 가능
/// </summary>
[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    // ========== 기본 설정 ==========

    [Header("기본 정보")]
    public string enemyName;
    public EnemyType enemyType;

    [Header("Movement")]
    public float moveSpeed = 3.5f;

    [Header("체력")]
    public float baseHealth;

    // ========== 공격 설정 ==========

    [Header("Attack")]
    [Tooltip("공격 범위 (Player와의 거리)")]
    public float attackRange = 2f;

    [Tooltip("사용 가능한 공격 Trigger 목록 (랜덤 선택)")]
    public List<string> enabledAttacks = new List<string> { "1ATTACK", "2ATTACK" };

    // ========== 돌진 설정 ==========

    [Header("Dash")]
    [Tooltip("돌진 기능 사용 여부")]
    public bool canDash = true;

    [Tooltip("돌진 속도")]
    public float dashSpeed = 10f;

    [Tooltip("돌진 범위 (이 거리 안에서 돌진 가능)")]
    public float dashRange = 8f;

    [Tooltip("돌진 정지 거리 (Player와 이 거리까지 접근)")]
    public float dashStopDistance = 1f;

    // ========== 기절 설정 ==========

    [Header("Stun")]
    [Tooltip("기절 지속 시간 (초)")]
    public float stunDuration = 2f;

    // ========== 이펙트 설정 (옵션) ==========

    [Header("Effects (Optional)")]
    [Tooltip("사망 시 생성할 이펙트 Prefab")]
    public GameObject deathEffectPrefab;

}


public enum EnemyTypes
{
    Normal,
    Boss
}