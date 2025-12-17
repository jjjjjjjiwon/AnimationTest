using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ComboUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Image panelImage;
    [SerializeField] Color defaultColor = Color.white;
    [SerializeField] Color activeColor = Color.yellow;
    [SerializeField] Color hitColor = Color.magenta;

    [Header("Hit Effect")]
    [SerializeField] float hitFlashTime = 0.15f;
    [SerializeField] float punchScale = 0.2f;


    [Header("Input")]
    [SerializeField] KeyCode correctKey = KeyCode.Space;

    bool inputWindowOpen = false;
    bool inputSuccess = false;

    void Update()
    {
        if (!inputWindowOpen)
            return;

        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(0))
        {
            inputSuccess = true;
            inputWindowOpen = false; // 한 번만 입력 허용
        }
    }

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
    inputWindowOpen = false;

    if (inputSuccess)
    {
       //panelImage.DOColor(hitColor, 0.01f).SetEase(Ease.OutCubic);
        Debug.Log("⚔ JUST ATTACK!");
        // 여기서 데미지 증가
    }
    else
    {
        panelImage.color = defaultColor;
    }
}

public void OnNextAnimationStart()
{
    // 이전 공격에서 남은 연출 제거
    panelImage.color = defaultColor;

}




}
