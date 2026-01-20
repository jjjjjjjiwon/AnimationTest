using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestInputHandler : MonoBehaviour
{
    // ⭐ Inspector 연결 대신 자동으로 찾기
    private RuntimeManager runtimeManager;
    private EnemyController enemyController;
    private PlayerController playerController;
    private PlayerData playerData;

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

        // C 키: 스탯 출력 테스트
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (runtimeManager.playerStats != null)
            {
                runtimeManager.playerStats.PrintStats();
            }
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
            if (enemyController == null)
                enemyController = FindObjectOfType<EnemyController>();
                
            if (enemyController != null)
            {
                Debug.Log("적에게 데미지!");
                enemyController.TakeDamage(10);
            }
        }

        // ⭐ 보스 강화 테스트 (T 키)
        if (Input.GetKeyDown(KeyCode.T))
        {
            runtimeManager.TestBossUpgrade();
        }

        // Z키: 플레이어 추가 스탯
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (playerController == null)
                playerController = FindObjectOfType<PlayerController>();
                
            if (playerController != null)
            {
                Debug.Log("플레이어 스탯 포인트 +5");
                playerController.AddStatPoints(5);
            }
        }

        // Y키: 플레이어 데미지
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (playerController == null)
                playerController = FindObjectOfType<PlayerController>();
                
            if (playerController != null)
            {
                playerController.TakeDamage(10);
            }
        }

        // X키: 플레이어 스탯
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (playerData == null)
                playerData = FindObjectOfType<PlayerData>();
                
            if (playerData != null && playerData.stats != null)
            {
                playerData.stats.PrintStats();
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