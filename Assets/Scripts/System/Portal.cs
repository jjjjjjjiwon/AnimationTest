using UnityEngine;

/// <summary>
/// 스테이지 클리어 포탈
/// 클리어 조건 달성 시 활성화
/// </summary>
public class Portal : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject portalEffect;  // 포탈 이펙트
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [Header("UI")]
    [SerializeField] private GameObject interactUI;  // "F키를 눌러 포탈 사용"

    private bool isActive = false;
    private bool playerInRange = false;

    // ========================================
    // 초기화
    // ========================================

    void Start()
    {
        // 포탈 비활성화
        Deactivate();
    }

    // ========================================
    // 활성화/비활성화
    // ========================================

    /// <summary>포탈 활성화 (클리어 조건 달성 시 호출)</summary>
    public void Activate()
    {
        isActive = true;

        if (portalEffect != null)
            portalEffect.SetActive(true);

        Debug.Log("[Portal] 활성화!");
    }

    /// <summary>포탈 비활성화</summary>
    public void Deactivate()
    {
        isActive = false;

        if (portalEffect != null)
            portalEffect.SetActive(false);

        if (interactUI != null)
            interactUI.SetActive(false);
    }

    // ========================================
    // 플레이어 감지
    // ========================================

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerInRange = true;

        if (interactUI != null)
            interactUI.SetActive(true);

        Debug.Log($"[Portal] 플레이어 범위 진입 / 활성 상태 = {isActive}");
    }


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }

    // ========================================
    // 상호작용
    // ========================================

    void Update()
    {
        if (!isActive)
            return;

        if (!playerInRange)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            UsePortal();
        }
    }
    private void UsePortal()
{
    Debug.Log("[Portal] 사용!");

    // StageManager / StageData 확인
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

    // ✅ 포탈은 '보상 UI 띄우기'만 담당
    if (RuntimeManager.Instance != null)
    {
        RuntimeManager.Instance.GiveReward(stage);
    }
    else
    {
        Debug.LogError("[Portal] RuntimeManager.Instance null");
    }

    // ✅ 포탈은 보상 띄운 뒤 다시 눌려도 중복 호출 안 되게 꺼버리는게 안전
    Deactivate();
}



}