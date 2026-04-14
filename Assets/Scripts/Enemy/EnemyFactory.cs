using UnityEngine;
using System.Collections.Generic;

public class EnemyFactory : MonoBehaviour
{
    public static EnemyFactory Instance { get; private set; }

    [Header("Json Loaders")]
    [SerializeField] private EnemyDataLoader enemyLoader;
    [SerializeField] private EnemyStateLoader stateLoader;

    // 데이터베이스 창고들
    private Dictionary<string, EnemyJsonData> enemyDatabase = new Dictionary<string, EnemyJsonData>();
    private Dictionary<string, BossJsonData> bossDatabase = new Dictionary<string, BossJsonData>();
    private Dictionary<string, EnemyIdleStateJsonData> idleDatabase = new Dictionary<string, EnemyIdleStateJsonData>();
    private Dictionary<string, EnemyChaseStateJsonData> chaseDatabase = new Dictionary<string, EnemyChaseStateJsonData>();
    private Dictionary<string, EnemyDashStateJsonData> dashDatabase = new Dictionary<string, EnemyDashStateJsonData>();
    private Dictionary<string, EnemyTeleportStateJsonData> teleportDatabase = new Dictionary<string, EnemyTeleportStateJsonData>();
    private Dictionary<string, EnemyAttackMotionJsonData> attackMotionDatabase = new Dictionary<string, EnemyAttackMotionJsonData>();
    private Dictionary<string, EnemyComboJsonData> attackComboDatabase = new Dictionary<string, EnemyComboJsonData>();
    private Dictionary<string, EnemyStunStateJsonData> stunDatabase = new Dictionary<string, EnemyStunStateJsonData>();
    private Dictionary<string, EnemyDeathStateJsonData> deathDatabase = new Dictionary<string, EnemyDeathStateJsonData>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        InitDatabase();
    }

    private void InitDatabase()
    {
        Debug.Log("[EnemyFactory] 데이터베이스 초기화 시작...");

        // 1. 보스 데이터 로드 확인
        if (enemyLoader != null)
        {
            enemyLoader.LoadEnemies();

            // 보스 리스트를 가져와서 bossDatabase에 채워넣기
            var bossList = enemyLoader.GetBossList();
            if (bossList != null)
            {
                foreach (var b in bossList)
                {
                    // boss_ID가 flame_titan인지 확인해보세요!
                    if (!bossDatabase.ContainsKey(b.boss_ID))
                    {
                        bossDatabase.Add(b.boss_ID, b);
                    }
                }
                Debug.Log($"[EnemyFactory] 보스 데이터 {bossDatabase.Count}개 딕셔너리 등록 완료");
            }
        }

        // 1. 적 & 보스 로드
        if (enemyLoader != null)
        {
            enemyLoader.LoadEnemies();
            // Getter 함수(GetEnemyList)를 사용하여 데이터 채우기
            foreach (var e in enemyLoader.GetEnemyList()) enemyDatabase.TryAdd(e.enemy_ID, e);
            foreach (var b in enemyLoader.GetBossList()) bossDatabase.TryAdd(b.boss_ID, b);
        }

        // 2. 상태 데이터 로드
        if (stateLoader != null)
        {
            // 하나씩 개별 로드 함수 호출 (원래 있던 방식)
            stateLoader.LoadIdleState();
            stateLoader.LoadChaseState();
            stateLoader.LoadDashState();
            stateLoader.LoadTeleportState();
            stateLoader.LoadAttackMotions();
            stateLoader.LoadCombos();
            stateLoader.LoadStunState();
            stateLoader.LoadDeathState();

            // [핵심] 원래 있던 함수명인 GetXXXList()를 사용하여 딕셔너리에 채우기
            foreach (var s in stateLoader.GetIdleList()) idleDatabase.TryAdd(s.Enemy_Idle_State_ID, s);
            foreach (var s in stateLoader.GetChaseList()) chaseDatabase.TryAdd(s.enemy_Chase_State_ID, s);
            foreach (var s in stateLoader.GetDashList()) dashDatabase.TryAdd(s.enemy_Dash_State_ID, s);
            foreach (var s in stateLoader.GetTeleportList()) teleportDatabase.TryAdd(s.enemy_Teleport_State_ID, s);
            foreach (var s in stateLoader.GetStunList()) stunDatabase.TryAdd(s.enemy_Stun_State_ID, s);
            foreach (var s in stateLoader.GetDeathList()) deathDatabase.TryAdd(s.enemy_Death_State_ID, s);

            // 공격 관련 (GetMotionList, GetComboList 사용)
            foreach (var s in stateLoader.GetMotionList()) attackMotionDatabase.TryAdd(s.attack_motion_ID, s);
            foreach (var s in stateLoader.GetComboList()) attackComboDatabase.TryAdd(s.enemy_Combo_ID, s);

            Debug.Log($"[EnemyFactory] 로드 완료! (Idle:{idleDatabase.Count}, Chase:{chaseDatabase.Count})");
        }
    }

    public void Spawn(string id, Vector3 pos, float rot = 0f)
    {
        Debug.Log($"[EnemyFactory] Spawn 호출됨 - ID: {id}");

        EnemyDataPackage package = CreatePackage(id);
        if (package == null)
        {
            Debug.LogError($"[EnemyFactory] 패키지 생성 실패 - ID: {id}");
            return;
        }

        Debug.Log($"[EnemyFactory] 패키지 생성 성공, 실제 Instantiate 시도");

        if (package == null) return;

        IEnemyData commonData = (package.bossData != null) ? (IEnemyData)package.bossData : (IEnemyData)package.baseData;
        Debug.Log($"[EnemyFactory] 프리팹 로드 시도 경로: {commonData.prefab_Path}");
        GameObject prefab = Resources.Load<GameObject>(commonData.prefab_Path);
        if (prefab == null)
        {
            Debug.LogError($"[EnemyFactory] 프리팹 로드 실패! 경로를 확인하세요: {commonData.prefab_Path}");
            return;
        }
        Debug.Log($"[EnemyFactory] 프리팹 로드 성공! 진짜 소환합니다.");
        GameObject go = Instantiate(prefab, pos, Quaternion.Euler(0, rot, 0));
        var controller = go.GetComponent<TestEnemyController>();
        if (controller != null) controller.Setup(package);


    }


    public EnemyDataPackage CreatePackage(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;

        EnemyDataPackage package = new EnemyDataPackage();
        IEnemyData rawData = null;

        // 1. 기초 데이터 찾기
        if (bossDatabase.TryGetValue(id, out var b)) { package.bossData = b; rawData = b; }
        else if (enemyDatabase.TryGetValue(id, out var e)) { package.baseData = e; rawData = e; }

        if (rawData == null)
        {
            Debug.LogError($"[EnemyFactory] {id}를 어느 DB에서도 찾을 수 없습니다.");
            return null;
        }

        // 2. [ID 기반 조립] (null 방어 추가)
        string tid = GetUpgradeId(id, BossActionType.teleport, rawData.enemy_Teleport_State_ID);
        string did = GetUpgradeId(id, BossActionType.dash, rawData.enemy_Dash_State_ID);

        // 3. 필터링된 ID로 실물 데이터 채우기 (ID가 null이 아닐 때만 시도!)
        // 💡 TryGetValue에 넘기기 전 string.IsNullOrEmpty를 꼭 체크해야 합니다.

        if (!string.IsNullOrEmpty(rawData.enemy_Idle_State_ID))
            if (idleDatabase.TryGetValue(rawData.enemy_Idle_State_ID, out var idle)) package.idleData = idle;

        if (!string.IsNullOrEmpty(rawData.enemy_Chase_State_ID))
            if (chaseDatabase.TryGetValue(rawData.enemy_Chase_State_ID, out var chase)) package.chaseData = chase;

        if (!string.IsNullOrEmpty(tid))
            if (teleportDatabase.TryGetValue(tid, out var tele)) package.teleportData = tele;

        if (!string.IsNullOrEmpty(did))
            if (dashDatabase.TryGetValue(did, out var dash)) package.dashData = dash;

        if (!string.IsNullOrEmpty(rawData.enemy_Stun_State_ID))
            if (stunDatabase.TryGetValue(rawData.enemy_Stun_State_ID, out var stun)) package.stunData = stun;

        if (!string.IsNullOrEmpty(rawData.enemy_Death_State_ID))
            if (deathDatabase.TryGetValue(rawData.enemy_Death_State_ID, out var death)) package.deathData = death;

        // 4. 공격 콤보 조립 (기존 로직 유지하되 안전하게)
        if (rawData.enemy_Combo_ID != null)
        {
            foreach (var cID in rawData.enemy_Combo_ID)
            {
                if (string.IsNullOrEmpty(cID)) continue;
                if (attackComboDatabase.TryGetValue(cID, out var combo))
                {
                    package.comboList.Add(combo);
                    if (combo.motion_Steps_ID != null)
                    {
                        foreach (var mID in combo.motion_Steps_ID)
                        {
                            if (string.IsNullOrEmpty(mID)) continue;
                            if (attackMotionDatabase.TryGetValue(mID, out var motion))
                                package.motionDic.TryAdd(mID, motion);
                        }
                    }
                }
            }
        }

        // 5. 스탯 계산
        CalculateFinalStats(package, rawData);

        return package;
    }

    private void CalculateFinalStats(EnemyDataPackage package, IEnemyData rawData)
    {
        // 인터페이스의 소문자 변수명을 사용합니다.
        float hp = rawData.base_Health;
        float atk = rawData.base_Damage;
        float speed = rawData.base_Speed;
        float def = rawData.base_Defense;

        if (RuntimeManager.Instance != null)
        {
            foreach (var up in RuntimeManager.Instance.SelectedBossUpgrades)
            {
                // up.boss_id (소문자 필드)와 rawData.enemy_id (인터페이스) 비교
                if (string.IsNullOrEmpty(up.boss_ID) || up.boss_ID == rawData.enemy_ID)
                {
                    if (up.type == BossUpgradeType.stat)
                    {
                        // [핵심 수정] up.stat_Type(문자열)이 아니라 
                        // Enum으로 변환된 up.stat_type_enum을 사용해야 합니다!
                        switch (up.stat_type_enum)
                        {
                            case BossStatType.health: hp *= (1 + up.value); break;
                            case BossStatType.damage: atk *= (1 + up.value); break;
                            case BossStatType.speed: speed *= (1 + up.value); break;
                            case BossStatType.defense: def *= (1 + up.value); break;
                        }
                    }
                }
            }
        }

        // 최종 결과물 저장
        package.finalMaxHP = hp;
        package.finalDamage = atk;
        package.finalMoveSpeed = speed;
        package.finalDefense = def;
    }

    private string GetUpgradeId(string ownerId, BossActionType type, string defaultId)
    {
        if (RuntimeManager.Instance == null) return defaultId;

        var upgrade = RuntimeManager.Instance.SelectedBossUpgrades.Find(u =>
            (string.IsNullOrEmpty(u.boss_ID) || u.boss_ID == ownerId) && u.target_action_type == type);

        // [수정] upgrade_ID가 아니라 실제 데이터 주소인 ability_ID를 리턴해야 합니다!
        return upgrade != null ? upgrade.ability_ID : defaultId;
    }
}