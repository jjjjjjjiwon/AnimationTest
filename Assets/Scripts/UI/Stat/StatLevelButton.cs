using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatLevelButton : MonoBehaviour
{
    public StatType statType;        // 이 버튼의 역할
    public PlayerInfoUI playerInfoUI;

    public void OnUpClick()
    {        
        playerInfoUI.TryLevelUp(statType);
    }

    public void OnDwonClick()
    {        
        playerInfoUI.TryLevelDown(statType);
    }
}
