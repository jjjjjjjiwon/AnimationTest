using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
[Header("Basic Info")]
    public string enemyName;
    public RuntimeAnimatorController animatorController;

    [Header("Stats")]
    public float maxHealth = 100f;
    public float moveSpeed = 3f;

    [Header("Attack Settings")]
    public float attackRange = 2f;
    public List<string> enabledAttacks = new List<string> { "1ATTACK" };

    [Header("Stun Settings")]
    public float stunDuration = 2f;

    [Header("Death Settings")]
    public float deathDelay = 2f;

    // ========== Optional States ==========
    [Header("Optional States")]
    public bool canDash = false;



    // ========== Dash (canDash가 true일 때만 보임) ==========
    [Header("Dash Settings")]
    [ShowIf("canDash")]
    public float dashSpeed = 10f;
    
    [ShowIf("canDash")]
    public float dashRange = 5f;
    
    [ShowIf("canDash")]
    public float dashStopDistance = 1f;

}