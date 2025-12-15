using UnityEngine;
using UnityEngine.UI;

public class ComboUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Image panelImage;
    [SerializeField] Color defaultColor = Color.white;
    [SerializeField] Color activeColor = Color.yellow;

    [Header("Input")]
    [SerializeField] KeyCode correctKey = KeyCode.Space;

    bool inputWindowOpen = false;
    bool inputSuccess = false;

    // 🔔 애니메이션 이벤트 시작
    public void EventStart()
    {
        panelImage.color = activeColor;
        inputWindowOpen = true;
        inputSuccess = false;
    }

    // 🔔 애니메이션 이벤트 종료
    public void EventEnd()
    {
        panelImage.color = defaultColor;
        inputWindowOpen = false;

        if (inputSuccess)
        {
            Debug.Log("✅ 올바른 입력!");
            // 성공 처리 (콤보 증가, 점수 등)
        }
        else
        {
            Debug.Log("❌ 실패");
            // 실패 처리
        }
    }

    void Update()
    {
        if (!inputWindowOpen)
            return;

        if (Input.GetKeyDown(correctKey))
        {
            inputSuccess = true;
            inputWindowOpen = false; // 한 번만 입력 허용
        }
    }
}
