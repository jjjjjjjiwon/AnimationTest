using UnityEngine;
using UnityEngine.UI;

public class ComboGaugeUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Scrollbar gaugeScrollbar;
    [SerializeField] private RectTransform perfectZoneRect;
    [SerializeField] private Image perfectZoneImage;

    private Animator playerAnimator;
    private SocketManager socketManager;
    private RectTransform scrollRect;
    private float barWidth;

    private void Awake()
    {
        if (gaugeScrollbar != null)
        {
            scrollRect = gaugeScrollbar.GetComponent<RectTransform>();
            barWidth = scrollRect.rect.width;
        }
    }

    public void Init(Animator animator, SocketManager manager)
    {
        playerAnimator = animator;
        socketManager = manager;
        gameObject.SetActive(false); 
    }

    public void RefreshPerfectZone()
    {
        if (socketManager == null) return;

        // AttackSkillData -> PlayerSkillData로 수정
        PlayerSkillData skill = socketManager.GetCurrentSkill();
        if (skill == null) return;

        // 변수명을 데이터 구조(perfect_Start, perfect_End)에 맞춤
        float zoneWidth = (skill.perfect_End - skill.perfect_Start) * barWidth;
        
        perfectZoneRect.sizeDelta = new Vector2(zoneWidth, perfectZoneRect.sizeDelta.y);
        
        // 시작 위치 설정 (Pivot이 좌측 하단(0,0) 기준이어야 정확합니다)
        perfectZoneRect.anchoredPosition = new Vector2(skill.perfect_Start * barWidth, 0);

        gaugeScrollbar.value = 0;
    }

    void Update()
    {
        if (playerAnimator == null)
        {
            PlayerController pc = Object.FindAnyObjectByType<PlayerController>();
            if (pc != null) playerAnimator = pc.Animator;
            return; 
        }

        // 현재 재생 중인 애니메이션 정보 가져오기
        var stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(0);
        
        // 콤보 게이지 바늘 업데이트
        if (gaugeScrollbar != null)
        {
            gaugeScrollbar.value = stateInfo.normalizedTime % 1.0f;
        }
    }
}