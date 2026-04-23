using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestInputHandler : MonoBehaviour
{
    // ⭐ Inspector 연결 대신 자동으로 찾기
    private RuntimeManager runtimeManager;
    private PlayerController playerController;
    private PlayerData playerData;

    private TestEnemyController testEnemyController;

    void Start()
    {
        // ⭐ 자동으로 찾기
        runtimeManager = RuntimeManager.Instance;

        if (runtimeManager == null)
        {
            Debug.LogWarning("[TestInputHandler] RuntimeManager를 찾을 수 없습니다!");
        }
    }

    void Update()
    {
        // ⭐ RuntimeManager 체크
        if (runtimeManager == null)
            return;

        if (InputManager.StatOutput())
        {
            if (runtimeManager.playerStats != null)
            {
                runtimeManager.playerStats.PrintStats();
            }
        }


        if (InputManager.GetDebugAddGold())
        {
            runtimeManager.gold += 100;
            Debug.Log($"[골드] +100 → 총 {runtimeManager.gold}G");
        }

        if (InputManager.EnemyStun())
        {
            // 1. 씬에 있는 모든 TestEnemyController를 배열로 가져옵니다.
            TestEnemyController[] allEnemies = GameObject.FindObjectsByType<TestEnemyController>(FindObjectsSortMode.None);

            if (allEnemies.Length > 0)
            {
                float testDamage = 10f;
                float testAddStun = 0.5f;

                // 2. 루프를 돌며 모든 적의 TakeDamage를 호출합니다.
                foreach (var enemy in allEnemies)
                {
                    enemy.TakeDamage(testDamage, testAddStun);
                }

                Debug.Log($"[테스트] 총 {allEnemies.Length}명의 적에게 데미지를 전달했습니다.");
            }
            else
            {
                Debug.LogWarning("씬에 적이 하나도 없습니다!");
            }
        }

        if (InputManager.PlayerDamage())
        {
            if (playerController == null)
                playerController = FindObjectOfType<PlayerController>();

            if (playerController != null)
            {
                playerController.TakeDamage(10);
            }
        }



        if (InputManager.PlayerAddStat())
        {
            if (playerController == null)
                playerController = FindObjectOfType<PlayerController>();

            if (playerController != null)
            {
                Debug.Log("플레이어 스탯 포인트 +5");
                playerController.AddStatPoints(5);
            }
        }

        // F1 키: 아이템 ID 1 추가
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (runtimeManager.playerInventory != null)
            {
                runtimeManager.playerInventory.AddItem(1, 1);
                Debug.Log("[테스트] 아이템 1 추가");
            }
        }

        // F2 키: 아이템 ID 2 추가
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (runtimeManager.playerInventory != null)
            {
                runtimeManager.playerInventory.AddItem(2, 1);
                Debug.Log("[테스트] 아이템 2 추가");
            }
        }
    }
}