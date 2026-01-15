using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// JSON에서 아이템 데이터를 읽어 GameData에 저장
/// 게임 시작 시 자동 로드
/// </summary>
public class ItemDataLoader : MonoBehaviour
{
    void Start()
    {
        LoadItemsFromJSON();
    }
    
    /// <summary>
    /// JSON 파일을 읽어서 ItemData들을 생성
    /// </summary>
    void LoadItemsFromJSON()
    {
        // 1. JSON 파일 읽기
        TextAsset jsonFile = Resources.Load<TextAsset>("items");
        
        if (jsonFile == null)
        {
            Debug.LogError("[ItemLoader] items.json 파일을 찾을 수 없습니다!");
            return;
        }
        
        // 2. JSON 파싱
        ItemDataList dataList = JsonUtility.FromJson<ItemDataList>(jsonFile.text);
        
        if (dataList == null || dataList.items == null)
        {
            Debug.LogError("[ItemLoader] JSON 파싱 실패!");
            return;
        }
        
        // 3. Dictionary 생성
        Dictionary<int, ItemData> itemDict = new Dictionary<int, ItemData>();
        
        foreach (ItemJsonData jsonData in dataList.items)
        {
            // ScriptableObject 생성
            ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
            
            // 기본 데이터 복사
            itemData.itemID = jsonData.itemID;
            itemData.itemName = jsonData.itemName;
            itemData.itemType = ParseItemType(jsonData.itemType);
            itemData.healAmount = jsonData.healAmount;
            itemData.description = jsonData.description;
            
            // Sprite 로드 (아이콘)
            if (!string.IsNullOrEmpty(jsonData.iconPath))
            {
                itemData.icon = Resources.Load<Sprite>(jsonData.iconPath);
                
                if (itemData.icon == null)
                {
                    Debug.LogWarning($"[ItemLoader] '{jsonData.iconPath}' 이미지를 찾을 수 없습니다!");
                }
            }
            
            // Dictionary에 추가
            itemDict[itemData.itemID] = itemData;
            
            Debug.Log($"[ItemLoader] {itemData.itemName} 로드 - ID:{itemData.itemID}");
        }
        
        // 4. GameData에 저장
        if (GameData.Instance != null)
        {
            GameData.Instance.itemDatabase = itemDict;
            Debug.Log($"[ItemLoader] {itemDict.Count}개 아이템 로드 완료!");
        }
        else
        {
            Debug.LogError("[ItemLoader] GameData.Instance가 null입니다!");
        }
    }
    
    /// <summary>
    /// 문자열을 ItemType으로 변환
    /// </summary>
    ItemType ParseItemType(string typeString)
    {
        switch (typeString)
        {
            case "Consumable": return ItemType.Consumable;
            case "Equipment": return ItemType.Equipment;
            case "Material": return ItemType.Material;
            case "KeyItem": return ItemType.KeyItem;
            default: return ItemType.Consumable;
        }
    }
}

/// <summary>
/// JSON 최상위 구조 (items 배열 포함)
/// </summary>
[System.Serializable]
public class ItemDataList
{
    public List<ItemJsonData> items;
}

/// <summary>
/// JSON의 개별 아이템 데이터 구조
/// </summary>
[System.Serializable]
public class ItemJsonData
{
    public int itemID;
    public string itemName;
    public string iconPath;
    public string itemType;
    public float healAmount;
    public string description;
}