using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MagicSlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI keyText;
    [SerializeField] private Image selectionHighlight; // 선택된 칸 표시 (필요 시)

    private int myIndex;
    private SocketManagerUI manager;

    // 초기화: SocketManagerUI의 Start나 별도 세팅 함수에서 호출
    public void Initialize(int index, SocketManagerUI managerUI, string keyName)
    {
        myIndex = index;
        manager = managerUI;
        if (keyText != null) keyText.text = keyName;

        // 💡 생성되자마자 버튼 기능을 매니저와 연결
        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(OnClickSlot);
        }

        UpdateVisual();
    }

    // 데이터가 바뀔 때마다 아이콘 갱신
    public void UpdateVisual()
    {
        if (RuntimeManager.Instance == null) return;

        if (RuntimeManager.Instance.EquipedMagics.TryGetValue(myIndex, out MagicData data))
        {
            Sprite loadedIcon = Resources.Load<Sprite>(data.Icon_Path);
            if (loadedIcon != null)
            {
                iconImage.sprite = loadedIcon;
                iconImage.enabled = true; // 마법이 있을 때만 아이콘 켬
                iconImage.color = Color.white;
            }
        }
        else
        {
            // 💡 중요: 아이콘만 끄고, 슬롯 배경은 보여야 합니다.
            // 만약 iconImage가 슬롯 배경과 같다면 이 줄을 지우거나 알파값만 조절하세요.
            iconImage.enabled = false;
        }
    }

    // 유니티 버튼의 OnClick 이벤트에 이 함수를 연결하세요!
    public void OnClickSlot()
    {
        if (manager != null)
        {
            // -1은 무기 소켓이 아님을 뜻함, myIndex는 0~8
            manager.SelectSocketSlot(-1, myIndex);
            Debug.Log($"[마법 슬롯 선택] {myIndex}번 슬롯이 활성화되었습니다.");
        }
    }
}