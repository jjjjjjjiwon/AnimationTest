using UnityEngine;

public interface IEnemy
{
    Transform Transform { get; }
    Transform Player { get; }
    Animator Animator { get; }
    Rigidbody Rigidbody { get; }
    EnemyData Data { get; }
    
    void OnAttackFinished();
}