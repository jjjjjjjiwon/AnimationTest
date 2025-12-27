using UnityEngine;

public interface IEnemy
{
    Transform Transform { get; }
    Transform Player { get; }
    Rigidbody Rigidbody { get; }
    Animator Animator { get; }
    EnemyData Data { get; }
}