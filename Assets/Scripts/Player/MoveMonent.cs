using System.Collections;
using System.Collections.Generic;
using UnityEngine;

# region // velocity
public class MoveMonent : MonoBehaviour
{
    [SerializeField] float normalTurnSpeed = 12f;
    [SerializeField] float animationTurnSpeed = 3f;

    public float Speed = 5f;                // 이동 속도
    public Transform cameraTransform;       // 카메라
    public float AnimatorSpeed => animatorSpeed;

    private Vector3 moveDir;        // 이동용
    private Vector3 lookDir;        // 회전용
    private float animatorSpeed;    // 블랜드 트리 파라미터 넘겨줄값

    private Rigidbody rb;
    private TestAnimation testAnimation;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        testAnimation = GetComponent<TestAnimation>();

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }


    void Update()
    {
        HandleInput();
        RotatePlayerToMoveDirection();
    }

    void FixedUpdate()
    {
        if (testAnimation.IsMove)
        {
            MovePlayer();
        }
        else
        {
            // 이동만 막기 (관성 제거)
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }

        UpdateAnimator();
    }

    void HandleInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        Vector3 inputDir = (forward * v + right * h);

        // 회전 방향 갱신 
        lookDir = inputDir;

        // 애니메이션이 실행 중이 아니고, 입력 받을 때
        if (!testAnimation.IsMove || inputDir.sqrMagnitude < 0.01f)
        {
            moveDir = Vector3.zero;
            animatorSpeed = 0f;
            return;
        }

        moveDir = inputDir.normalized;         // 대각선 속도 보정

    }

    void MovePlayer()
    {
        // Y축 유지
        rb.velocity = new Vector3(
            moveDir.x * Speed,
            rb.velocity.y,
            moveDir.z * Speed
    );
    }
    void RotatePlayerToMoveDirection()
    {
        if (lookDir.sqrMagnitude < 0.01f) return;

    float turnSpeed = testAnimation.IsMove 
        ? normalTurnSpeed 
        : animationTurnSpeed;

    Quaternion targetRotation = Quaternion.LookRotation(lookDir);

    rb.rotation = Quaternion.Slerp(
        rb.rotation,
        targetRotation,
        turnSpeed * Time.deltaTime
    );
    }

    void UpdateAnimator()
    {
        Vector3 horizontalVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        animatorSpeed = Mathf.Clamp01(horizontalVelocity.magnitude / Speed);
    }

}
# endregion
