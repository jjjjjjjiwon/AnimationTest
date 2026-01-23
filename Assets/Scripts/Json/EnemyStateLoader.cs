using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateLoader : MonoBehaviour
{
    [SerializeField] private string IdleJsonPath = "Json/EnemyState/EnemyIdleState"; 

    private void Start()
    {
        LoadEnemy();
    }

    public void LoadEnemy()
    {
        TextAsset Idlejson = Resources.Load<TextAsset>(IdleJsonPath);
        if (Idlejson == null)
        {
            Debug.LogError($"[IdleJsonPath] JSON not found: Resources/{IdleJsonPath}.json");
            return;
        }
        else
        {
            Debug.LogError($"[IdleJsonPath] Success");
        }

    }
}
