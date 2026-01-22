using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class Boss_Upgreade_UI : MonoBehaviour
{
    [Header("UI Windows")]
    [SerializeField] private GameObject upgradePanel; // 정보를 담고 있는 전체 패널

    [Header("Buttons")]
    [SerializeField] private Button open_Button;    // 패널 여는 버튼
    [SerializeField] private Button close_Button;   // 패널 닫는 버튼 (추가 권장)

    [Header("Grid Settings")]
    [SerializeField] private GameObject boss_Upgrade_Prefab; // 그리드에 들어갈 텍스트 프리팹
    [SerializeField] private Transform container;           // Grid Layout Group이 붙은 부모

    void Start()
    {
        // 1. 초기 상태: 패널 끄기
        if (upgradePanel != null) upgradePanel.SetActive(false);

        // 2. 버튼 리스너 등록
        if (open_Button != null)
            open_Button.onClick.AddListener(OpenUpgradeUI);

        if (close_Button != null)
            close_Button.onClick.AddListener(CloseUpgradeUI);
    }

    

    public void OpenUpgradeUI()
    {
        // 패널을 먼저 켜고
        if (upgradePanel != null) upgradePanel.SetActive(true);

        // 데이터 갱신 (그리드 생성)
        RefreshUpgradeUI();
    }

    public void CloseUpgradeUI()
    {
        // 패널 끄기
        if (upgradePanel != null) upgradePanel.SetActive(false);
    }

    private void RefreshUpgradeUI()
    {
        // 1. 싱글톤에서 데이터 가져오기
        var upgrades = RuntimeManager.Instance?.SelectedBossUpgrades ?? new List<BossUpgradeJsonData>();

        // 2. 기존 그리드 아이템 삭제
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        // 3. 데이터만큼 새 아이템 생성
        foreach (var data in upgrades)
        {

            if (data.Type == BossUpgradeType.none)
            {
                continue;
            }

            GameObject item = Instantiate(boss_Upgrade_Prefab, container);
            TextMeshProUGUI txt = item.GetComponentInChildren<TextMeshProUGUI>();

            if (txt != null)
            {
                txt.text = $"{data.upgradeDescription}";
            }
        }
    }

}
