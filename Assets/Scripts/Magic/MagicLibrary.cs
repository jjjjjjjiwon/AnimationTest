using UnityEngine;
using System.Collections.Generic;

public static class MagicLibrary
{
    // 마법의 오브젝트를 List로 기억
    private static List<MagicBase> activeMagics = new List<MagicBase>();

    private static readonly Dictionary<string, Dictionary<string, float>> _wordStats = new Dictionary<string, Dictionary<string, float>>
    {
        { "Summon",      new Dictionary<string, float> { { "Dist", 5.0f } } },
        { "Expansion",   new Dictionary<string, float> { { "GrowthRate", 0.8f }, { "MaxSize", 4.0f } } },
        { "MoveForward", new Dictionary<string, float> { { "Speed", 10.0f } } },
        { "Spiral",      new Dictionary<string, float> { { "Weight", 40.0f } } }, // 키: Weight
        { "Split",       new Dictionary<string, float> { { "ScaleMult", 0.5f }, { "SpeedMult", 1.2f } } }
    };

    public static void Execute(string magicName, Transform caster)
    {
        // 로그에 SummonElement라고 뜨므로 이름을 맞춰줍니다.
        if (magicName == "SummonElement")
        {
            ClearAll();
            var m = SummonElement(caster);
            if (m != null) activeMagics.Add(m);
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
            if (m != null) Object.Destroy(m.gameObject);
        activeMagics.Clear();
    }

    private static void SplitAll()
    {
        if (activeMagics.Count == 0) return;

        // 현재 리스트를 복사해서 반복문을 돌립니다 (리스트 변조 방지)
        List<MagicBase> parents = new List<MagicBase>(activeMagics);
        List<MagicBase> nextGeneration = new List<MagicBase>();

        foreach (var p in parents)
        {
            if (p == null) continue;

            // 💡 자식들을 생성하고 리스트에 추가
            var children = Split(p);
            nextGeneration.AddRange(children);

            // 💡 부모는 소멸
            Object.Destroy(p.gameObject);
        }

        // 💡 전체 마법 리스트를 자식 세대로 교체
        activeMagics = nextGeneration;
    }

    #region 소환
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
        mb.Init(caster);

        if (_wordStats.TryGetValue("Summon", out var stats))
            mb.followDistance = stats["Dist"];

        return mb;
    }
    #endregion

    #region 거대화
    public static void Gigantism(MagicBase target)
    {
        if (target == null) return;
        target.Launch();

        if (_wordStats.TryGetValue("Expansion", out var stats))
        {
            float rate = stats["GrowthRate"];
            float limit = stats["MaxSize"];

            target.AddLogic(() =>
            {
                if (target == null) return;

                // 속도가 빠를수록 더 빨리 커지게 설정 (거리 기반 느낌)
                // 속도가 0이면 아주 천천히, 속도가 있으면 그에 비례해 커집니다.
                float currentSpeedFactor = Mathf.Max(1.0f, target.moveSpeed);
                float growth = rate * currentSpeedFactor * Time.deltaTime;

                Vector3 nextScale = target.transform.localScale + Vector3.one * growth;

                // 최대 크기 제한
                if (nextScale.x < limit)
                {
                    target.transform.localScale = nextScale;
                }
                else
                {
                    target.transform.localScale = Vector3.one * limit;
                }
            });
        }
    }

    #endregion

    #region 나아가다
    public static void MoveForward(MagicBase target)
    {
        if (target == null) return;
        target.Launch();

        if (_wordStats.TryGetValue("MoveForward", out var stats))
        {
            float speedAmount = stats["Speed"];
            target.moveSpeed += speedAmount; // 데이터 누적

            // 💡 기존처럼 로직을 계속 추가 (누를수록 이 코드가 여러번 실행되어 빨라짐)
            target.AddLogic(() =>
            {
                if (target == null) return;
                // rotationWeight가 0이면 직선, 값이 있으면 나선 중 가속
                if (target.rotationWeight == 0)
                {
                    // 직선 이동 (기존 방식)
                    target.transform.position += target.transform.forward * speedAmount * Time.deltaTime;
                }
            });
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
        if (!_wordStats.TryGetValue("Split", out var stats)) return children;

        int splitCount = 3;
        for (int i = 0; i < splitCount; i++)
        {
            // 1. 계산: 이번 자식이 바라볼 각도 (360도를 등분)
            float angle = i * (360f / splitCount);
            Quaternion rot = Quaternion.Euler(0, angle, 0);

            // 2. 객체 생성 (위치와 회전을 계산된 값으로 설정)
            // rot * Vector3.forward를 통해 각기 다른 방향으로 0.5f만큼 떨어진 곳에 생성
            GameObject childObj = Object.Instantiate(target.gameObject,
                                                   target.transform.position + (rot * Vector3.forward * 0.5f),
                                                   rot); // 💡 여기서 각 자식의 앞방향이 결정됩니다.

            MagicBase child = childObj.GetComponent<MagicBase>();

            // 3. 데이터 및 로직 상속
            child.transform.localScale = target.transform.localScale * stats["ScaleMult"];
            child.moveSpeed = target.moveSpeed * stats["SpeedMult"];
            child.isLaunched = true;
            child.caster = target.caster;

            // 💡 중요: 자식들에게 각자의 정면(forward)으로 나아가라는 명령 주입
            float currentSpeed = child.moveSpeed;
            child.AddLogic(() =>
            {
                if (child == null) return;
                // 이제 각 자식은 Instantiate 때 설정된 '자신의 정면'으로 날아갑니다.
                child.transform.position += child.transform.forward * currentSpeed * Time.deltaTime;
            });

            children.Add(child);
        }
        return children;
    }



    #endregion







}