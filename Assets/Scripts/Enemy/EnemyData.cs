using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Stats")]
    public float maxHealth = 100f;      // 체력
    public float moveSpeed = 5f;        // 이동 속도

    [Header("Combat")]
    public float attackRange = 2f;      // 공격 사거리
    public float attackDamage = 10f;    // 데미지

    [Header("Special")]
    public float dashRange = 5f;    // 대시 공격 사거리
    public bool canDash = false;    // 대시 공격 여부
    public float dashSpeed = 15f;   // 대시 속도
    public float dashStopDistance = 1f; // 대시 종료 거리

    // ========== 여기부터 추가! ==========
    [Header("Animation")]
    public RuntimeAnimatorController animatorController;  // Animator Controller

    [Header("Attacks")]
    public List<string> enabledAttacks = new List<string>();  // 사용할 공격 Trigger
}