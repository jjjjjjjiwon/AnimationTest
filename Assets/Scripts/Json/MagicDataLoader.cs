using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicDataLoader : MonoBehaviour
{
    [SerializeField] private string jsonPath = "Json/Magic"; // Resources/Json/Magic.json

    private void Start()
    {
        LoadMagicData();
    }

public void LoadMagicData()
{
    TextAsset json = Resources.Load<TextAsset>(jsonPath);
    if (json == null) {
        Debug.LogError($"[에러] JSON 파일을 못 찾음: {jsonPath}");
        return;
    }

    // JSON 텍스트가 잘 읽히는지 확인
    Debug.Log($"[로그] JSON 데이터 읽기 성공: {json.text.Length} 글자");

    MagicListWrapper list = JsonUtility.FromJson<MagicListWrapper>(json.text);
    
    if (list == null || list.magic == null || list.magic.Count == 0) {
        Debug.LogError("[에러] 파싱 실패! JSON 구조와 MagicListWrapper 클래스가 일치하는지 확인하세요.");
        return;
    }

    if (GameData.Instance != null) {
        GameData.Instance.SetMagicDataList(list.magic); 
        Debug.Log($"[성공] GameData에 마법 {list.magic.Count}개 저장 완료!");
    }
}
}
