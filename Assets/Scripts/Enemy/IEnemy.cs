using UnityEngine;

// 적이 가지고 있어야 할 것
// EnemyController가 이를 구현하고, State들이 사용
public interface IEnemy
{
    Transform Transform { get; }
    Transform Player { get; }
    Animator Animator { get; }
    Rigidbody Rigidbody { get; }
    EnemyData Data { get; }
    
    void OnAttackFinished();
    void OnStunFinished();
}