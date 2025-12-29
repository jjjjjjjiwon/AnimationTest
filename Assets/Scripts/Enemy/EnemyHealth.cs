// using UnityEngine;

// public class EnemyHealth : MonoBehaviour
// {
//     public float maxHealth = 100f;
//     private float currentHealth;

//     private EnemyController enemyController;

//     void Start()
//     {
//         currentHealth = maxHealth;
//         enemyController = GetComponent<EnemyController>();
//     }

//     public void TakeDamage(float damage)
//     {
//         currentHealth -= damage;
//         Debug.Log($"{gameObject.name} took {damage} damage. HP: {currentHealth}/{maxHealth}");

//         // ========== 무조건 기절 ==========
//         if (enemyController != null)
//         {
//             enemyController.TakeStun();
//         }

//         // 사망 체크
//         if (currentHealth <= 0)
//         {
//             Die();
//         }
//     }

//     private void Die()
//     {
//         Debug.Log($"{gameObject.name} died!");

//         // DeathState 진입
//         if (enemyController != null)
//         {
//             enemyController.Die();
//         }
//         else
//         {
//             // EnemyController 없으면 바로 제거
//             Destroy(gameObject);
//         }
//     }
// }