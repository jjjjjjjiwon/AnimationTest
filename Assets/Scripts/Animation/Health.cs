using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;    

    public float CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0;

    private float currentHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(float damage)
    {
        if (IsDead) return;
        
        currentHealth -= damage;
        Debug.Log($"데미지 : {damage}, 체력 : {currentHealth}/{maxHealth}");
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }
    
    public void Heal(float heal)
    {
        if (IsDead) return;
        
        currentHealth += heal;
        
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        
        Debug.Log($"{gameObject.name} healed {heal}! HP: {currentHealth}/{maxHealth}");
    }
    
    void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        // Entity에서 처리하도록 이벤트 발생 (나중에)
        gameObject.SetActive(false);
    }
}