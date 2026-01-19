using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestInputHandler : MonoBehaviour
{
    public RuntimeManager runtimeManager;
    EnemyController enemyController;
    PlayerController playerController;
    PlayerData playerData;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("======");
            SceneManager.LoadScene("Lobby");   
        }

        // C 키: 스탯 출력 테스트
        if (Input.GetKeyDown(KeyCode.C))
        {
                runtimeManager.playerStats.PrintStats();
        }

        // G 키: 골드 추가 테스트
        if (Input.GetKeyDown(KeyCode.G))
        {
            runtimeManager.gold += 100;
            Debug.Log($"[골드] +100 → 총 {runtimeManager.gold}G");
        }

        // K키: 적 데미지
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("aasdasdsadasdasdsadasd");
            enemyController.TakeDamage(10);
        }

        // ⭐ 보스 강화 테스트 (T 키)
        if (Input.GetKeyDown(KeyCode.T))
        {
            runtimeManager.TestBossUpgrade();
        }

        // Z키: 플렝이어 추가 스탯
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Debug.Log("PPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPP");
            playerController.AddStatPoints(5);
        }

        // Y키: 플레이어 데미지
        if (Input.GetKeyDown(KeyCode.Y))
        {
            playerController.TakeDamage(10);
        }

        // X키: 플레이어 스탯
        if (Input.GetKeyDown(KeyCode.X))
        {
            playerData.stats.PrintStats();
        }



        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (runtimeManager.playerInventory != null)
            {
                runtimeManager.playerInventory.AddItem(1, 1);
            }
        }

        // 2 키: 아이템 ID 2 추가
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (runtimeManager.playerInventory != null)
            {
                runtimeManager.playerInventory.AddItem(2, 1);
            }
        }
    }

}
