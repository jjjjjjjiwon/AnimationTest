using UnityEngine;
using System.Collections.Generic;

public class EnemyFactory : MonoBehaviour
{
    // 외부(StageManager 등)에서 접근 가능하도록 싱글톤 설정
    public static EnemyFactory Instance { get; private set; }

    [Header("Json Loaders")]
    [SerializeField] private EnemyDataLoader enemyLoader;
    [SerializeField] private EnemyStateLoader stateLoader;

    // JsonLoader, ID로 즉시 찾기 위한 데이터 창고
    private Dictionary<string, EnemyJsonData> enemyDatabase = new Dictionary<string, EnemyJsonData>();
    private Dictionary<string, EnemyIdleStateJsonData> idleDatabase = new Dictionary<string, EnemyIdleStateJsonData>();
    private Dictionary<string, EnemyChaseStateJsonData> chaseDatabase = new Dictionary<string, EnemyChaseStateJsonData>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 1. 창고를 먼저 채웁니다.
        InitDatabase();
    }

    private void InitDatabase()
    {
        // 로더에게 JSON 파일을 읽으라고 명령
        if (enemyLoader != null) enemyLoader.LoadEnemy();
        if (stateLoader != null)
        {
            stateLoader.LoadIdleState();
            stateLoader.LoadChaseState();
        }

        // [적 본체 데이터 정리]
        foreach (var data in enemyLoader.GetEnemyList())
        {
            if (!enemyDatabase.ContainsKey(data.enemy_ID))
                enemyDatabase.Add(data.enemy_ID, data);
        }

        // [Idle 상태 데이터 정리]
        foreach (var data in stateLoader.GetIdleList())
        {
            if (!idleDatabase.ContainsKey(data.Enemy_Idle_State_ID))
                idleDatabase.Add(data.Enemy_Idle_State_ID, data);
        }

        // [Chase 상태 데이터 정리]
        foreach (var data in stateLoader.GetChaseList())
        {
            if (!chaseDatabase.ContainsKey(data.Enemy_Chase_State_ID))
                chaseDatabase.Add(data.Enemy_Chase_State_ID, data);
        }


        Debug.Log($"[EnemyFactory] Database Init: Enemy({enemyDatabase.Count}), Idle({idleDatabase.Count})");
    }

    // [핵심 로직] StageManager가 호출하는 소환 함수
    public void SpawnEnemy(string enemyId, Vector3 spawnPos, float spawnRotation = 0f)
    {
        // 1. 데이터 패키지 조립 (본체 + 상태 정보)
        EnemyDataPackage package = CreatePackage(enemyId);

        if (package == null)
        {
            Debug.LogError($"[Factory] {enemyId}의 패키지를 생성할 수 없습니다.");
            return;
        }

        // 2. 프리팹 로드 및 소환
        // JSON 데이터에 적힌 prefab_Path를 사용 (예: "Prefabs/Enemies/Zombie")
        GameObject prefab = Resources.Load<GameObject>(package.baseData.prefab_Path);

        if (prefab == null)
        {
            Debug.LogError($"[Factory] 프리팹 로드 실패: {package.baseData.prefab_Path}");
            return;
        }

        GameObject go = Instantiate(prefab, spawnPos, Quaternion.Euler(0, spawnRotation, 0));

        // 3. 적 컨트롤러에게 데이터 뭉치 전달 (Setup 함수가 있다고 가정)
        var controller = go.GetComponent<TestEnemyController>();
        if (controller != null)
        {
            controller.Setup(package);
        }
        else
        {
            Debug.LogWarning($"[Factory] {enemyId} 프리팹에 TestEnemyController가 없습니다.");
        }
    }

    private EnemyDataPackage CreatePackage(string enemy_ID)
    {
        if (!enemyDatabase.ContainsKey(enemy_ID))
        {
            Debug.LogError($"[Factory] {enemy_ID} 본체 데이터가 창고에 없습니다.");
            return null;
        }

        EnemyDataPackage package = new EnemyDataPackage();
        package.baseData = enemyDatabase[enemy_ID];

        // Idle 데이터 조립 (ID 기반 검색)
        string idleId = package.baseData.Enemy_Idle_State_ID;
        if (idleId != "none" && idleDatabase.ContainsKey(idleId))
        {
            package.idleData = idleDatabase[idleId];
        }

        string chaseId = package.baseData.Enemy_Chase_State_ID;
        if (chaseDatabase.ContainsKey(chaseId))
        {
            package.chaseData = chaseDatabase[chaseId];
        }

        return package;
    }
}

// 여러 JSON 데이터를 하나로 묶어주는 배달 상자
public class EnemyDataPackage
{
    public EnemyJsonData baseData;      // 이름, 체력, 프리팹 경로 등
    public EnemyIdleStateJsonData idleData;  // 가만히 있을 때의 애니메이션 등\
    public EnemyChaseStateJsonData chaseData;

}