using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossDefinitionLoader : MonoBehaviour
{
    private void Start()
    {
        LoadBossesFromJSON();
    }

    private void LoadBossesFromJSON()
    {
        // Resources/Json/boss.json (확장자 없이)
        TextAsset jsonFile = Resources.Load<TextAsset>("Json/boss");

        if (jsonFile == null)
        {
            Debug.LogError("[BossDefinitionLoader] boss.json 파일을 찾을 수 없습니다! (Resources/Json/boss.json)");
            return;
        }

        BossJsonList list = JsonUtility.FromJson<BossJsonList>(jsonFile.text);
        if (list == null || list.bosses == null)
        {
            Debug.LogError("[BossDefinitionLoader] JSON 파싱 실패!");
            return;
        }

        List<BossDefinition> defs = new List<BossDefinition>();

        foreach (var j in list.bosses)
        {
            BossDefinition def = ScriptableObject.CreateInstance<BossDefinition>();

def.bossId = j.bossId;
def.bossName = j.bossName;
def.prefabPath = j.prefabPath;

// baseStats
def.baseStats = new BossStats
{
    maxHp = j.maxHp,
    damage = j.damage,
    moveSpeed = j.moveSpeed,
    armor = j.armor
};

// keys
def.abilityKeys = j.abilityKeys != null ? j.abilityKeys.ToArray() : new string[0];
def.patternKeys = j.patternKeys != null ? j.patternKeys.ToArray() : new string[0];

// ✅ prefab 로드는 반드시 여기서
if (!string.IsNullOrEmpty(def.prefabPath))
{
    def.prefab = Resources.Load<GameObject>(def.prefabPath);

    if (def.prefab == null)
        Debug.LogWarning($"[BossDefinitionLoader] prefab 로드 실패: path='{def.prefabPath}' boss='{def.bossName}'");
}
        }

        if (GameData.Instance != null)
        {
            GameData.Instance.SetBossDefinitions(defs);
            Debug.Log($"[BossDefinitionLoader] {defs.Count}개 보스 정의 로드 완료!");
        }
        else
        {
            Debug.LogError("[BossDefinitionLoader] GameData.Instance null");
        }
    }
}
