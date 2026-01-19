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
        Debug.Log($"[Portal] Trigger 감지: {other.gameObject.name}, Tag: {other.tag}");

        if (!isActive)
        {
            Debug.Log("[Portal] 포탈이 비활성 상태");
            return;
        }

        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("[Portal] 플레이어 범위 진입!");

            if (interactUI != null)
                interactUI.SetActive(true);
        }
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

        Debug.Log($"[Portal] Update - 활성: {isActive}, 범위: {playerInRange}");
        // F키 입력
        if (Input.GetKeyDown(interactKey))
        {
            UsePortal();
        }
    }

    private void UsePortal()
    {
        Debug.Log("[Portal] 사용!");

        // 스테이지 클리어 처리
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnStageCleared();
        }
    }
}