using UnityEngine;
using System.Collections.Generic;

public static class MagicLibrary
{
    // 하나가 아닌 리스트로 모든 활성 마법을 기억합니다.
    private static List<MagicBase> _activeMagics = new List<MagicBase>();

    public static void Execute(string magicName, Transform caster)
    {
        if (magicName == "SummonElement")
        {
            ClearAll(); // 칠판 지우기
            var newMagic = SummonElement("SummonElement", caster);
            if (newMagic != null) _activeMagics.Add(newMagic);
        }
        else if (magicName == "MoveForward")
        {
            // 리스트에 있는 모든 마법에게 명령 (낙오자 없음)
            foreach (var m in _activeMagics) MoveForward(m);
        }
        else if (magicName == "Split")
        {
            if (_activeMagics.Count == 0) return;

            // 1. 현재 세대 복사본 생성 (반복 중 리스트 변경 에러 방지)
            List<MagicBase> parents = new List<MagicBase>(_activeMagics);
            // 2. 다음 세대를 담을 임시 리스트
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
            _activeMagics = nextGeneration;
        }
    }

    private static readonly Dictionary<string, Dictionary<string, float>> _wordStats = new Dictionary<string, Dictionary<string, float>>
    {
        { "Summon",      new Dictionary<string, float> { { "Dist", 10.0f }} },
        { "MoveForward", new Dictionary<string, float> { { "Speed", 10.0f } } },
        { "Split",       new Dictionary<string, float> { { "ScaleMult", 0.5f }, { "SpeedMult", 1.2f } } }
    };

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

    public static void MoveForward(MagicBase target)
    {
        if (target == null) return;

        if (_wordStats.TryGetValue("MoveForward", out var stats))
        {
            target.Launch();
            target.moveSpeed += stats["Speed"];

            // 💡 Y축 값을 0으로 만들어 수평으로만 날아가게 설정 (선택 사항)
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

    // 💡 Split의 역할을 '생성'과 '명단 반환'으로 분리했습니다.
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

    private static void ClearAll()
    {
        foreach (var m in _activeMagics)
            if (m != null) Object.Destroy(m.gameObject);
        _activeMagics.Clear();
    }
}