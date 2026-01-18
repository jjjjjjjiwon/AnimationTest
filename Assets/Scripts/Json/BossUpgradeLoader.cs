using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// JSON에서 보스 강화 데이터를 읽어 GameData로 변환
/// 게임 시작 시 자동 로드
/// </summary>
public class BossUpgradeLoader : MonoBehaviour
{
    void Start()
    {
        LoadBossUpgradesFromJSON();
    }
    
    /// <summary>
    /// JSON 파일을 읽어서 보스 강화 데이터를 GameData에 저장
    /// </summary>
    void LoadBossUpgradesFromJSON()
    {
        // 1. JSON 파일 읽기
        TextAsset jsonFile = Resources.Load<TextAsset>("Data/BossUpgrades");
        
        if (jsonFile == null)
        {
            Debug.LogError("[BossUpgradeLoader] BossUpgrades.json을 찾을 수 없습니다!");
            return;
        }
        
        // 2. JSON 파싱
        BossUpgradeCollection collection = JsonUtility.FromJson<BossUpgradeCollection>(jsonFile.text);
        
        if (collection == null || collection.bosses == null)
        {
            Debug.LogError("[BossUpgradeLoader] JSON 파싱 실패!");
            return;
        }
        
        // 3. Dictionary로 변환
        Dictionary<string, List<BossUpgrade>> upgradesDB = new Dictionary<string, List<BossUpgrade>>();
        
        foreach (var bossUpgrades in collection.bosses)
        {
            upgradesDB[bossUpgrades.bossName] = bossUpgrades.upgrades;
            Debug.Log($"[BossUpgradeLoader] {bossUpgrades.bossName} - {bossUpgrades.upgrades.Count}개 강화 로드");
        }
        
        // 4. GameData에 저장
        if (GameData.Instance != null)
        {
            GameData.Instance.bossUpgradesDB = upgradesDB;
            Debug.Log($"[BossUpgradeLoader] {upgradesDB.Count}개 보스 강화 데이터 로드 완료!");
        }
        else
        {
            Debug.LogError("[BossUpgradeLoader] GameData.Instance가 null입니다!");
        }
    }
}

/// <summary>
/// JSON 최상위 구조
/// </summary>
[System.Serializable]
public class BossUpgradeCollection
{
    public List<BossUpgradeSet> bosses;
}

/// <summary>
/// 보스별 강화 목록
/// </summary>
[System.Serializable]
public class BossUpgradeSet
{
    public string bossName;
    public List<BossUpgrade> upgrades;
}