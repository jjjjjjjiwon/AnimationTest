using System.Collections.Generic;
using UnityEngine;

public class UnlockManager : MonoBehaviour
{
    public List<ComboData> GetUnlockedCombos() => unlockedCombos;

    [SerializeField] private List<ComboData> allCombos; // 모든 ComboData
    private List<ComboData> unlockedCombos = new List<ComboData>();

    private void Awake()
    {
        // 초기 해금 체크 (예: 첫 콤보만 해금)
        foreach (var combo in allCombos)
        {
            if (IsUnlocked(combo))
            {
                unlockedCombos.Add(combo);
            }
        }
    }

    // 해금 조건 체크 함수
    private bool IsUnlocked(ComboData combo)
    {
        // 예시: 모든 콤보 기본 해금
        return true;
    }

    // 외부에서 해금 시 호출
    public void UnlockCombo(ComboData combo)
    {
        if (!unlockedCombos.Contains(combo))
        {
            unlockedCombos.Add(combo);
            // PlayerCombat 갱신 등 필요 시 이벤트 호출 가능
        }
    }
}
