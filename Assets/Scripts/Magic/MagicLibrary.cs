using UnityEngine;
using System.Collections.Generic;

public static class MagicLibrary
{
    // 마법의 오브젝트를 List로 기억
    private static List<MagicBase> activeMagics = new List<MagicBase>();

    private static readonly Dictionary<string, Dictionary<string, float>> _wordStats = new Dictionary<string, Dictionary<string, float>>
    {
        { "Summon",      new Dictionary<string, float> { { "Dist", 5.0f } } },
        { "Gigantism",   new Dictionary<string, float> { { "GrowthRate", 0.05f }, { "MaxSize", 30.0f } } },
        { "MoveForward", new Dictionary<string, float> { { "Speed", 10.0f } } },
        { "Spiral",      new Dictionary<string, float> { { "Weight", 40.0f } } }, // 키: Weight
        { "Split",       new Dictionary<string, float> { { "ScaleMult", 0.5f }, { "SpeedMult", 1.2f } } }
    };

    public static void Execute(string magicName, Transform caster)
    {
        if (magicName == "SummonElement")
        {
            RemoveOnlyElements();

            var m = SummonElement(caster);
            if (m != null) activeMagics.Add(m);
        }
        else if (magicName == "SetPlayerAsTarget")
        {
            ClearAll();

            if (caster.TryGetComponent(out MagicBase mb))
            {
                SetPlayerAsTarget(mb);
                activeMagics.Add(mb);
                Debug.Log("플레이어가 마법의 타겟으로 설정되었습니다.");
            }
        }
        else if (magicName == "Gigantism")
        {
            foreach (var m in activeMagics) Gigantism(m);
        }
        else if (magicName == "MoveForward")
        {
            foreach (var m in activeMagics) MoveForward(m);
        }
        else if (magicName == "Spiral")
        {
            foreach (var m in activeMagics) Spiral(m);
        }
        else if (magicName == "Split")
        {
            SplitAll();
        }
    }

    private static void ClearAll()
    {
        foreach (var m in activeMagics)
        {
            if (m == null) continue;

            if (m.isTargetingPlayer)
            {
                m.ResetToNormalState();
                continue;
            }

            Object.Destroy(m.gameObject);
        }
        activeMagics.Clear();
    }

    private static void RemoveOnlyElements()
    {
        for (int i = activeMagics.Count - 1; i >= 0; i--)
        {
            var m = activeMagics[i];
            if (m != null && !m.isTargetingPlayer)
            {
                Object.Destroy(m.gameObject);
                activeMagics.RemoveAt(i);
            }
        }
    }

    public static void CleanUpOrUpdateMagics()
    {
        List<MagicBase> nextMagics = new List<MagicBase>();

        foreach (var m in activeMagics)
        {
            if (m == null) continue;

            if (m.magicLifeTime <= 0)
            {
                if (m.isTargetingPlayer)
                {
                    m.ResetToNormalState();
                    nextMagics.Add(m);
                }
                else
                {
                    Object.Destroy(m.gameObject);
                }
                continue;
            }

            if (m.isTargetingPlayer)
            {
                nextMagics.Add(m);
            }
        }
        activeMagics = nextMagics;
    }

    public static void SplitAll()
    {
        List<MagicBase> nextMagics = new List<MagicBase>();

        foreach (var m in activeMagics)
        {
            if (m == null) continue;

            if (m.isTargetingPlayer)
            {
                // --- [핵심: 플레이어 보호] ---
                // 플레이어는 분열을 해도 원본이 사라지면 안 되므로 리스트에 유지합니다.
                nextMagics.Add(m);

                // 그리고 분신들을 생성해서 추가합니다.
                List<MagicBase> clones = Split(m); // 여기서 MagicLibrary.Split 호출
                if (clones != null) nextMagics.AddRange(clones);
            }
            else
            {
                // 일반 원소(화구 등)는 분열 후 원본을 파괴합니다.
                List<MagicBase> clones = Split(m);
                if (clones != null) nextMagics.AddRange(clones);
                Object.Destroy(m.gameObject);
            }
        }
        activeMagics = nextMagics;
    }

    private static void CopyAnimatorParameters(Animator source, Animator target)
    {
        foreach (AnimatorControllerParameter param in source.parameters)
        {
            if (param.type == AnimatorControllerParameterType.Float)
                target.SetFloat(param.name, source.GetFloat(param.name));
            else if (param.type == AnimatorControllerParameterType.Int)
                target.SetInteger(param.name, source.GetInteger(param.name));
            else if (param.type == AnimatorControllerParameterType.Bool)
                target.SetBool(param.name, source.GetBool(param.name));
            else if (param.type == AnimatorControllerParameterType.Trigger && source.GetBool(param.name))
                target.SetTrigger(param.name);
        }
    }

    #region 원소 타겟
    public static MagicBase SummonElement(Transform caster)
    {
        // 💡 경로 확인: Resources/Prefab/Magic/SummonElement 인지 Element 인지 꼭 확인하세요!
        GameObject prefab = Resources.Load<GameObject>("Prefab/Magic/SummonElement");

        if (prefab == null)
        {
            Debug.LogError("마법 프리팹을 찾을 수 없습니다! 경로를 확인하세요.");
            return null;
        }

        Vector3 spawnPos = caster.position + (Vector3.up * 1.5f);
        GameObject obj = Object.Instantiate(prefab, spawnPos, caster.rotation);

        MagicBase mb = obj.GetComponent<MagicBase>();

        // 💡 중요: Init을 해야 OnLogic에 '따라다니기' 대본이 들어갑니다.
        mb.isTargetingPlayer = false; // 소환된 원소이므로 false
        mb.Init(caster);

        if (_wordStats.TryGetValue("Summon", out var stats))
            mb.followDistance = stats["Dist"];

        Debug.Log($"{mb.isTargetingPlayer} 타겟");


        return mb;
    }
    #endregion

    #region 플레이어 타겟
    public static void SetPlayerAsTarget(MagicBase playerMb)
    {
        if (playerMb == null) return;

        // 1. 이 MagicBase는 이제 플레이어를 제어 대상으로 함
        playerMb.isTargetingPlayer = true;
        Debug.Log($"{playerMb.isTargetingPlayer} 타겟");

    }
    #endregion

    #region 거대화
    public static void Gigantism(MagicBase target)
    {
        if (target == null) return;

        if (_wordStats.TryGetValue("Gigantism", out var stats))
        {
            float rate = stats["GrowthRate"];
            float limit = stats["MaxSize"];

            // 중요: Launch 직전의 위치를 확실히 잡습니다.
            Vector3 spawnPos = target.transform.position;
            float initialScale = target.transform.localScale.x;

            target.AddLogic(() =>
            {
                if (target == null) return;

                // 1. 현재 이동한 거리 계산
                float dist = Vector3.Distance(spawnPos, target.transform.position);

                // 2. 목표 크기 계산 (기본 크기 + 거리 비율)
                float targetScale = initialScale + (dist * rate);
                targetScale = Mathf.Min(targetScale, limit);

                // 3. [핵심] 갑자기 커지지 않게 Lerp로 부드럽게 보간
                // 현재 크기에서 목표 크기로 매 프레임 조금씩 접근
                float smoothScale = Mathf.Lerp(target.transform.localScale.x, targetScale, Time.deltaTime * 5f);

                target.transform.localScale = Vector3.one * smoothScale;
            });

            target.Launch();
        }
    }

    #endregion

    #region 나아가다
    public static void MoveForward(MagicBase target)
    {
        if (target == null) return;

        // 1. 발사 전이라면 발사 처리 (여기서 _moveDirection이 결정됨)
        if (!target.isLaunched) target.Launch();

        if (_wordStats.TryGetValue("MoveForward", out var stats))
        {
            // 2. 속도만 올려주면, 나머지는 Base의 ApplyPhysicalMovement가 
            // 고정된 방향으로 전진시키는 것을 책임집니다.
            target.moveSpeed += stats["Speed"];
        }
    }
    #endregion

    #region 나선
    public static void Spiral(MagicBase target)
    {
        if (target == null) return;
        target.Launch();

        if (_wordStats.TryGetValue("Spiral", out var stats))
        {
            target.rotationWeight = stats["Weight"];

            // 1. 발사 시점의 정보 고정
            Vector3 startPos = target.transform.position;
            Vector3 forwardX = target.transform.forward; // 진행 방향 (X축 역할)

            // 2. 진행 방향에 수직인 바닥 쪽 방향 (Z축 역할)
            Vector3 rightZ = Vector3.Cross(Vector3.up, forwardX).normalized;

            float elapsedTime = 0f;

            target.AddLogic(() =>
            {
                if (target == null) return;
                elapsedTime += Time.deltaTime;

                // [사용자님의 그림 2번: Y축 변화 없이 X, Z로만 그리는 나선]

                // A. X축 전진 (중심선 이동)
                float xDist = target.moveSpeed * elapsedTime;
                Vector3 xPoint = startPos + (forwardX * xDist);

                // B. 나선 반경 (속도에 비례해 넓어짐)
                float radius = elapsedTime * (target.moveSpeed * 0.15f);

                // C. 가변 각도 로직 (속도가 빠를수록 30도 -> 80도처럼 완만하게)
                float frequency = 12.0f / (1.0f + target.moveSpeed * 0.1f);
                float angle = elapsedTime * target.rotationWeight * frequency;
                float rad = angle * Mathf.Deg2Rad;

                // D. 위치 계산: 전진(X) + 좌우(Z)
                // Y값은 startPos.y를 그대로 유지하거나, 아주 미세한 보정만 들어갑니다.
                Vector3 zOffset = rightZ * Mathf.Sin(rad) * radius;

                // X축 이동 성분에도 Cos을 주어 자연스러운 나선 곡선을 만듭니다.
                Vector3 xOffset = forwardX * Mathf.Cos(rad) * radius;

                target.transform.position = xPoint + xOffset + zOffset;

                // 고개는 진행 축을 바라보게 정렬
                target.transform.forward = forwardX;
            });
        }
    }
    #endregion

    #region 분열
    public static List<MagicBase> Split(MagicBase target)
{
    List<MagicBase> children = new List<MagicBase>();

    // 1. [공통] 기준 방향 결정
    Quaternion baseRotation;
    if (target.isTargetingPlayer && target.caster != null)
        baseRotation = target.caster.rotation; // 플레이어 시선 기준
    else
        baseRotation = Quaternion.LookRotation(target.InitialFireDirection != Vector3.zero 
                       ? target.InitialFireDirection : target.transform.forward);

    int splitCount = 3;

    // --- CASE A: 플레이어가 마법인 경우 (분신 생성) ---
    if (target.isTargetingPlayer)
    {
        PlayerController originalCtrl = target.GetComponent<PlayerController>();
        
        for (int i = 0; i < splitCount; i++)
        {
            float angle = i * (360f / splitCount);
            Quaternion rot = baseRotation * Quaternion.Euler(0, angle, 0);
            // 분신은 겹치지 않게 약간 떨어진 위치에 소환
            Vector3 spawnPos = target.transform.position + (rot * Vector3.forward * 1.5f);

            GameObject dummyObj = Object.Instantiate(target.gameObject, spawnPos, rot);

            // 중요: 플레이어 조작 스크립트는 삭제하고 더미 컨트롤러 주입
            if (dummyObj.TryGetComponent(out PlayerController oldCtrl)) 
                Object.Destroy(oldCtrl);
            
            PlayerDummyController dummyCtrl = dummyObj.AddComponent<PlayerDummyController>();
            dummyCtrl.Setup(originalCtrl);

            MagicBase dummyBase = dummyObj.GetComponent<MagicBase>();
            dummyBase.isTargetingPlayer = false; // 분신은 조작 대상이 아님
            dummyBase.isLaunched = true;        // 상태만 발사로 변경
            
            children.Add(dummyBase);
        }
    }
    // --- CASE B: 일반 원소 마법인 경우 (투사체 분열) ---
// --- CASE B: 일반 원소 마법인 경우 ---
else
{
    if (!_wordStats.TryGetValue("Split", out var stats)) return children;

    float speed = target.moveSpeed; 

    for (int i = 0; i < splitCount; i++)
    {
        float angle = i * (360f / splitCount);
        Quaternion spawnRot = baseRotation * Quaternion.Euler(0, angle, 0);

        GameObject childObj = Object.Instantiate(target.gameObject, target.transform.position, spawnRot);
        MagicBase child = childObj.GetComponent<MagicBase>();

        // 1. [초기화] 복제된 부모의 대본을 비웁니다. 
        child.OnLogic = null; 

        // 2. [데이터] 속도 설정
        child.moveSpeed = speed * stats["SpeedMult"];
        child.caster = target.caster;

        // 3. [핵심: AddLogic] 자식에게 "직선 전진"이라는 새로운 대본을 써줍니다.
        // 이것이 사용자님이 원하시는 "하나씩 추가하는" 방식입니다.
        Vector3 moveDir = child.transform.forward; 
        float moveSpeed = child.moveSpeed;

        child.AddLogic(() => {
            if (child == null) return;
            // 베이스가 아니라 이 로직이 직접 이동을 담당합니다.
            child.transform.position += moveDir * moveSpeed * Time.deltaTime;
        });

        // 4. [보조] 베이스 엔진도 일단 켜둡니다. (혹시 모를 다른 기능을 위해)
        child.isLaunched = true;

        children.Add(child);
    }
}

    return children;
}

    #endregion







}