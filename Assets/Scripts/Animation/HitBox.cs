using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public Weapon weaponData;

    public bool hit => hitActive;
    private bool hitActive = false;
    
    private List<Collider> hitTargets = new List<Collider>();

    void OnTriggerEnter(Collider other)
    {
        if (!hitActive) return;

        if (hitTargets.Contains(other)) return;

        // Health 컴포넌트 찾기
        Health health = other.GetComponent<Health>();
        if (health != null)
        {
            hitTargets.Add(other);
            health.TakeDamage(weaponData.damage);
            Debug.Log($"Hit {other.name} with {weaponData.weaponName} for {weaponData.damage} damage");
        }
    }

    public void EnableHit()
    {
        hitActive = true;
        hitTargets.Clear();
    }

    public void DisableHit()
    {
        hitActive = false;
    }
}
