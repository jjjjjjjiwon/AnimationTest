using UnityEngine;

[CreateAssetMenu(fileName = "BossDefinition", menuName = "Game/Boss Definition")]
public class BossDefinition : ScriptableObject
{
    [Header("IDs")]
    public string bossId;      // JSON/Loader용 키
    public string bossName;    // 표시용

    [Header("Prefab")]
    public string prefabPath;  // Resources 경로
    public GameObject prefab;  // 로드된 프리팹

    [Header("Base Stats")]
    public BossStats baseStats = new BossStats();

    [Header("Keys")]
    public string[] abilityKeys;
    public string[] patternKeys;
}
