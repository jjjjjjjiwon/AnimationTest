using UnityEngine;
using System.Collections.Generic;

public static class MagicLibrary
{
    // 마법의 오브젝트를 List로 기억
    private static List<MagicBase> activeMagics = new List<MagicBase>();

    public static void Execute(string magicName, Transform caster)
    {
        if (magicName == "SummonElement")
        {
            ClearAll();
            var newMagic = SummonElement("SummonElement", caster);
            if (newMagic != null) activeMagics.Add(newMagic);
        }
        else if (magicName == "MoveForward")
        {
            foreach (var m in activeMagics) MoveForward(m);
        }
        else if (magicName == "Split")
        {
            if (activeMagics.Count == 0) return;

            List<MagicBase> parents = new List<MagicBase>(activeMagics);
            List<MagicBase> nextGeneration = new List<MagicBase>();

            foreach (var p in parents)
            {
                if (p == null) continue;
                // 3. 분열된 자식들을 생성하고 리스트로 받아옴
                List<MagicBase> children = Split(p);
                nextGeneration.AddRange(children);

                // 4. 부모는 임무를 마쳤으므로 삭제
                Object.Destroy(p.gameObject);
            }

            // 5. 전체 명단을 다음 세대로 교체!
            activeMagics = nextGeneration;
        }
    }

    private static readonly Dictionary<string, Dictionary<string, float>> _wordStats = new Dictionary<string, Dictionary<string, float>>
    {
        { "Summon",      new Dictionary<string, float> { { "Dist",  5.0f }} },
        { "MoveForward", new Dictionary<string, float> { { "Speed", 10.0f } } },
        { "Split",       new Dictionary<string, float> { { "ScaleMult", 0.5f }, { "SpeedMult", 1.2f } } }
    };

        private static void ClearAll()
    {
        foreach (var m in activeMagics)
            if (m != null) Object.Destroy(m.gameObject);
        activeMagics.Clear();
    }

#region 소환
    public static MagicBase SummonElement(string prefabName, Transform caster)
    {
        GameObject prefab = Resources.Load<GameObject>($"Prefab/Magic/{prefabName}");
        if (prefab == null) return null;

        // 💡 소환 위치 자체를 플레이어의 머리 높이 정도로 오프셋을 줍니다.
        Vector3 spawnPos = caster.position + (Vector3.up * 1.5f);
        GameObject obj = Object.Instantiate(prefab, spawnPos, caster.rotation);

        MagicBase mb = obj.GetComponent<MagicBase>();

        if (_wordStats.TryGetValue("Summon", out var stats))
            mb.followDistance = stats["Dist"];

        mb.Init(caster);
        return mb;
    }
#endregion

#region 나아가다
    public static void MoveForward(MagicBase target)
    {
        if (target == null) return;

        if (_wordStats.TryGetValue("MoveForward", out var stats))
        {
            target.Launch();
            target.moveSpeed += stats["Speed"];

            Vector3 shootDir = target.transform.forward;
            shootDir.y = 0;
            shootDir.Normalize();

            target.AddLogic(() =>
            {
                if (target == null) return;
                target.transform.position += shootDir * target.moveSpeed * Time.deltaTime;
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
        float scaleMult = stats["ScaleMult"];
        float speedMult = stats["SpeedMult"];

        for (int i = 0; i < splitCount; i++)
        {
            // 💡 1. 소환 위치에 약간의 오프셋(간격)을 줍니다. 
            // 겹쳐 있지 않게 좌우/앞뒤로 살짝 벌려줍니다.
            Vector3 spreadOffset = Quaternion.Euler(0, i * (360f / splitCount), 0) * Vector3.forward * 0.5f;
            Vector3 spawnPos = target.transform.position + spreadOffset;

            GameObject childObj = Object.Instantiate(target.gameObject, spawnPos, target.transform.rotation);
            MagicBase child = childObj.GetComponent<MagicBase>();

            // 💡 2. 스케일 설정: 부모의 현재 스케일에 배율을 곱함
            child.transform.localScale = target.transform.localScale * scaleMult;
            child.moveSpeed = target.moveSpeed * speedMult;

            // 3. 상태 상속 로직
            if (target.isLaunched)
            {
                child.Launch();
                // 각자 다른 방향으로 퍼져나가게 설정
                Vector3 shootDir = (spawnPos - target.transform.position).normalized;
                if (shootDir == Vector3.zero) shootDir = target.transform.forward;

                child.AddLogic(() =>
                {
                    if (child == null) return;
                    child.transform.position += shootDir * child.moveSpeed * Time.deltaTime;
                });
            }
            else
            {
                child.Init(target.caster);
            }

            children.Add(child);
        }

        Debug.Log($"{splitCount}개로 분열 완료!");
        return children;
    }
#endregion

#region 역행
    private static void Rewind(MagicBase target)
    {
        
    }
    #endregion


#region 나선
    public static void Spiral(MagicBase target, bool isClockwise = true)
    {
        if (target == null) return;

        target.Launch();
        // 데이터 시트에서 속도와 회전 반경 등을 가져온다고 가정
        float speed = 5.0f;
        float spiralRadius = 2.0f; // 원의 크기
        float spiralSpeed = 10.0f; // 회전 속도

        Vector3 forwardDir = target.transform.forward;
        Vector3 rightDir = target.transform.right;
        Vector3 upDir = target.transform.up;

        float elapsedTime = 0f;
        int direction = isClockwise ? 1 : -1;

        target.AddLogic(() =>
        {
            if (target == null) return;
            elapsedTime += Time.deltaTime;

            // 1. 기준점 이동 (중심축이 앞으로 나아감)
            Vector3 centerPos = forwardDir * speed * Time.deltaTime;

            // 2. 나선 회전 계산 (Sin, Cos)
            // 시간에 따라 Right와 Up 방향으로 위치를 변형
            float x = Mathf.Cos(elapsedTime * spiralSpeed) * spiralRadius;
            float y = Mathf.Sin(elapsedTime * spiralSpeed) * spiralRadius * direction;

            // 새로운 위치 = 현재 위치 + 전진량 + (회전 보정치)
            // 다만 단순히 더하면 축이 꼬이므로, '중심축'을 기준으로 오프셋을 계산하는 게 깔끔합니다.
            Vector3 offset = (rightDir * x) + (upDir * y);

            // 실제 이동: 축 방향 전진 + 회전 오프셋 적용
            // (이 방식은 물체가 나선을 그리며 '진행 방향'을 바라보게 하려면 추가 회전이 필요합니다)
            target.transform.position += centerPos + (offset * 0.1f); // 0.1f는 부드러운 전이를 위한 계수

            // 3. 방향 정렬 (나선 궤적의 접선 방향을 바라보게 함)
            target.transform.forward = forwardDir; // 일단은 중심축 유지
        });
    }
#endregion


}