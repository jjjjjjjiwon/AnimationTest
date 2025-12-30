using UnityEngine;

/// <summary>
/// 3인칭 카메라 컨트롤러
/// Player의 MoveMonent와 연동 - 카메라 기준 이동 지원
/// 마우스로 자유 회전 가능
/// </summary>
public class CameraController : MonoBehaviour
{
    // ========== Inspector 설정 ==========
    
    [Header("Target")]
    [Tooltip("따라갈 대상 (Player)")]
    [SerializeField] private Transform target;
    
    [Header("Camera Settings")]
    [Tooltip("카메라와 Player 사이의 거리")]
    [SerializeField] private float distance = 10f;
    
    [Tooltip("카메라 높이 (Player 위)")]
    [SerializeField] private float height = 5f;

    [Header("Rotation Settings")]
    [Tooltip("마우스 좌우 회전 속도")]
    [SerializeField] private float rotationSpeed = 5f;
    
    [Tooltip("마우스 상하 회전 속도")]
    [SerializeField] private float verticalSpeed = 3f;
    
    [Tooltip("최소 수직 각도 (아래를 볼 수 있는 각도)")]
    [SerializeField] private float minVerticalAngle = -20f;
    
    [Tooltip("최대 수직 각도 (위를 볼 수 있는 각도)")]
    [SerializeField] private float maxVerticalAngle = 80f;

    [Header("Mouse Settings")]
    [Tooltip("시작 시 마우스 커서 잠금")]
    [SerializeField] private bool lockCursor = true;

    // ========== 회전 값 ==========
    
    /// <summary>현재 수평 회전 각도 (좌우)</summary>
    private float currentRotationY = 0f;
    
    /// <summary>현재 수직 회전 각도 (상하)</summary>
    private float currentRotationX = 20f;

    // ========== Unity 생명주기 ==========

    void Start()
    {
        // 마우스 커서 설정
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // 초기 회전값 설정
        if (target != null)
        {
            currentRotationY = target.eulerAngles.y;
        }
    }

    void Update()
    {
        // ESC 키로 마우스 잠금 해제/다시 잠금
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursorLock();
        }
    }

    void LateUpdate()
    {
        if (target == null)
            return;

        // ========== 1. 마우스 입력 받기 ==========
        HandleMouseInput();

        // ========== 2. 카메라 위치 업데이트 ==========
        UpdateCameraPosition();
    }

    // ========== 마우스 입력 처리 ==========

    /// <summary>
    /// 마우스 움직임을 받아서 회전 값 계산
    /// </summary>
    private void HandleMouseInput()
    {
        // 마우스가 잠겨있을 때만 회전
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        // 마우스 좌우 이동 (X축)
        float mouseX = Input.GetAxis("Mouse X");
        
        // 마우스 상하 이동 (Y축)
        float mouseY = Input.GetAxis("Mouse Y");

        // ========== 수평 회전 (좌우) ==========
        currentRotationY += mouseX * rotationSpeed;

        // ========== 수직 회전 (상하) ==========
        currentRotationX -= mouseY * verticalSpeed;  // -를 해야 자연스러움
        
        // 수직 각도 제한 (너무 위/아래 못 보게)
        currentRotationX = Mathf.Clamp(currentRotationX, minVerticalAngle, maxVerticalAngle);
    }

    // ========== 카메라 위치 업데이트 ==========

    /// <summary>
    /// 회전 값을 기반으로 카메라 위치 계산
    /// Player의 MoveMonent에서 cameraTransform으로 사용됨
    /// </summary>
    private void UpdateCameraPosition()
    {
        // ========== 1. 회전 계산 ==========
        // 수평(Y축), 수직(X축) 회전을 Quaternion으로 변환
        Quaternion rotation = Quaternion.Euler(currentRotationX, currentRotationY, 0f);

        // ========== 2. 카메라 오프셋 계산 ==========
        // 기본 오프셋: 뒤(-Z) + 위(+Y)
        Vector3 offset = new Vector3(0f, height, -distance);
        
        // 회전 적용
        Vector3 rotatedOffset = rotation * offset;

        // ========== 3. 카메라 위치 ==========
        // Player 위치 + 회전된 오프셋
        Vector3 cameraPosition = target.position + rotatedOffset;
        
        transform.position = cameraPosition;

        // ========== 4. 카메라 회전 ==========
        // Player를 바라보도록 (약간 위를 봄 - height 적용)
        transform.LookAt(target.position + Vector3.up * height);
    }

    // ========== 유틸리티 ==========

    /// <summary>
    /// 마우스 커서 잠금/해제 토글
    /// ESC 키로 호출됨
    /// </summary>
    private void ToggleCursorLock()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // ========== Gizmos (Scene 뷰에서 디버깅용) ==========

    void OnDrawGizmosSelected()
    {
        if (target == null)
            return;

        // 카메라 위치 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(target.position, 0.5f);

        // 카메라 방향 표시
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(target.position, transform.position);
    }
}