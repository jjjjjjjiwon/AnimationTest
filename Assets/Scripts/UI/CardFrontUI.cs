using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CardFrontUI : MonoBehaviour
{
    [SerializeField] private TextMeshPro StageName;  // 제목
    private StageData stageData;  
    public void Setup(string Name)
    {
        // 텍스트 설정
        if (StageName != null)
            StageName.text = stageData.stage_Name;
    }
}
