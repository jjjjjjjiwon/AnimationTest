using System;
using System.Collections.Generic;

[Serializable]
public class EnemyComboJsonData
{
    public string enemy_Combo_ID;             // 콤보의 고유 ID (예: "Z_Combo_Standard")
    public List<string> motion_Steps_ID; // 연결할 모션 ID들의 배열 (순서대로 실행)
}

[Serializable]
public class EnemyComboListWrapper
{
    public List<EnemyComboJsonData> enemyCombo;
}