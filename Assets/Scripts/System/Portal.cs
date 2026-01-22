using UnityEngine;

/// <summary>
/// 스테이지 클리어 포탈
/// 클리어 조건 달성 시 활성화
/// 역할:
/// - 플레이어 상호작용 감지
/// - 보상 UI 요청만 담당
/// </summary>
public class Portal : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject portalEffect;
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [Header("UI")]
    [SerializeField] private GameObject interactUI;

    private bool isActive = false;
    private bool playerInRange = false;

    // ========================================
    // 초기화
    // ========================================

    void Start()
    {
        Deactivate();
    }

    // ========================================
    // 활성 / 비활성
    // ========================================

    public void Activate()
    {
        isActive = true;

        if (portalEffect != null)
            portalEffect.SetActive(true);

        Debug.Log("[Portal] 활성화");
    }

    public void Deactivate()
    {
        isActive = false;
        playerInRange = false;

        if (portalEffect != null)
            portalEffect.SetActive(false);

        if (interactUI != null)
            interactUI.SetActive(false);
    }

    // ========================================
    // 플레이어 감지
    // ========================================

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive)
            return;

        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;

        if (interactUI != null)
            interactUI.SetActive(true);

        Debug.Log("[Portal] 플레이어 범위 진입");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = false;

        if (interactUI != null)
            interactUI.SetActive(false);
    }

    // ========================================
    // 상호작용
    // ========================================

    void Update()
    {
        if (!isActive || !playerInRange)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            UsePortal();
        }
    }

    private void UsePortal()
    {
        Debug.Log("[Portal] 사용");

        if (StageManager.Instance == null)
        {
            Debug.LogError("[Portal] StageManager.Instance null");
            return;
        }

        StageData stage = StageManager.Instance.GetCurrentStage();
        if (stage == null)
        {
            Debug.LogError("[Portal] currentStage null");
            return;
        }

        if (RuntimeManager.Instance == null)
        {
            Debug.LogError("[Portal] RuntimeManager.Instance null");
            return;
        }

        // ✅ 포탈의 유일한 책임
        UIManager.Instance.ShowRewardUI(stage, stage.isBossStage);

        // 중복 방지
        Deactivate();
    }
}
