using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDataLoader : MonoBehaviour
{
    [SerializeField] private string jsonPath = "Json/Enemy"; // Resources/Json/Enemy.json

    private void Start()
    {
        LoadEnemy();
    }

    public void LoadEnemy()
    {
        TextAsset json = Resources.Load<TextAsset>(jsonPath);
        if (json == null)
        {
            Debug.LogError($"[EnemyDataLoader] JSON not found: Resources/{jsonPath}.json");
            return;
        }
        else
        {
            Debug.LogError($"[EnemyDataLoader] Succes");
        }


    }
}
