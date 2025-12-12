using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#region // Root Motion
// public class MoveMonent : MonoBehaviour
// {
//     public float RotationSpeed = 10f;  // 회전 속도
//     public Transform CameraTransform;

//     private Vector3 moveInput;
//     private Rigidbody rb;
//     private Animator animator;

//     public Vector3 MoveInput => moveInput;
//     public float AnimatorSpeed { get; private set; }

//     void Awake()
//     {
//         rb = GetComponent<Rigidbody>();
//         animator = GetComponent<Animator>();

//         rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

//         if (CameraTransform == null)
//             CameraTransform = Camera.main.transform;
//     }

//     void Update()
//     {
//         HandleInput();
//         RotatePlayer();
//         UpdateAnimator();
//     }

//     void HandleInput()
//     {
//         float h = Input.GetAxisRaw("Horizontal");
//         float v = Input.GetAxisRaw("Vertical");

//         Vector3 forward = CameraTransform.forward;
//         Vector3 right = CameraTransform.right;

//         forward.y = 0f;
//         right.y = 0f;
//         forward.Normalize();
//         right.Normalize();

//         Vector3 rawInput = forward * v + right * h;

//         if (rawInput.sqrMagnitude < 0.01f)
//         {
//             moveInput = Vector3.zero;
//             AnimatorSpeed = 0f;
//             return;
//         }

//         moveInput = rawInput.normalized;
//         AnimatorSpeed = Mathf.Clamp01(rawInput.magnitude);
//     }

//     void RotatePlayer()
//     {
//         if (moveInput.sqrMagnitude > 0.01f)
//         {
//             // moveInput방향 회전
//             Quaternion targetRot = Quaternion.LookRotation(moveInput);
//             transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, RotationSpeed * Time.deltaTime);
//         }
//     }

//     void UpdateAnimator()
//     {
//         animator.SetFloat("speed", AnimatorSpeed);
//         animator.SetBool("isRun", AnimatorSpeed > 0f);
//     }

//     // Root Motion 사용 시 Rigidbody velocity는 건드리지 않고 충돌만 처리
//     void OnAnimatorMove()
//     {
//         // Rigidbody 위치는 Animator에서 Root Motion으로 처리
//         rb.MovePosition(animator.rootPosition);
//         rb.MoveRotation(animator.rootRotation);
//     }
// }
# endregion

# region // velocity
public class MoveMonent : MonoBehaviour
{
    public float Speed = 5f;                // 이동 속도
    public Transform cameraTransform;       // 카메라
    public Vector3 MoveInput => moveInput;  
    public float AnimatorSpeed => animatorSpeed;

    private Vector3 moveInput;
    private float animatorSpeed;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;


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
        MovePlayer();
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

        Vector3 rawInput = forward * v + right * h;

        // 입력 없으면 정지
        if (rawInput.sqrMagnitude < 0.01f)
        {
            moveInput = Vector3.zero;
            animatorSpeed = 0f;
            return;
        }

        moveInput = rawInput.normalized;               // 대각선 속도 보정
        animatorSpeed = Mathf.Clamp01(rawInput.magnitude); // 0~1 애니메이션 speed
    }

    void MovePlayer()
    {
        // velocity 기반 이동
        rb.velocity = moveInput * Speed;
    }

    void RotatePlayerToMoveDirection()
    {
        if (moveInput.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, 0.2f);
        }
    }
}
# endregion

// public class MoveMonent : MonoBehaviour
// {
//         public float Speed = 10f;  // 이동 속도
//         public Transform cameraTransform;   // 카메라
        
//         public Vector3 MoveInput => moveInput;
//         public float AnimatorSpeed => animatorSpeed;
        
//         private Vector3 moveInput;
//         private Rigidbody rb;
//         private float animatorSpeed;

//         void Awake()
//         {
//             rb = GetComponent<Rigidbody>();

//             // 카메라 확인
//             if (cameraTransform == null)
//             cameraTransform = Camera.main.transform;
//         }
        

//         void Update()
//         {
//             HandleInput();
//             RotatePlayerToMoveDirection();
//         }

//     void FixedUpdate()
//     {
//         MovePlayer();
//     }

//     void HandleInput()
//     {
//         float h = Input.GetAxis("Horizontal");
//         float v = Input.GetAxis("Vertical");

//         if (h == 0 && v == 0)
//         {
//             moveInput = Vector3.zero;
//             animatorSpeed = 0f;
//             return;
//         }

//         // 카메라 기준 방향으로 이동 벡터 계산
//         Vector3 forward = cameraTransform.forward;
//         Vector3 right = cameraTransform.right;

//         // Y축 제거 → 평면 이동
//         forward.y = 0f;
//         right.y = 0f;

//         forward.Normalize();
//         right.Normalize();

//         // 정규화 전 입력 → 애니메이션 speed 용
//         Vector3 rawInput = forward * v + right * h;

//         // speed 값 (0~1)
//         animatorSpeed = Mathf.Clamp01(rawInput.magnitude);
//         // 이동 입력 (대각선 속도 증가 방지)
//         moveInput = rawInput.normalized;
//     }

//     void MovePlayer()
//     {
//         // 입력이 없으면 즉시 멈춤
//         if (moveInput == Vector3.zero)
//         {
//             rb.velocity = Vector3.zero;
//             return;
//         }
//         rb.MovePosition(rb.position + moveInput * Speed * Time.fixedDeltaTime);
//     }
//     void RotatePlayerToMoveDirection()
//     {
//         if (moveInput.sqrMagnitude > 0.01f)
//         {
//             // 이동 방향으로 회전
//             Quaternion targetRotation = Quaternion.LookRotation(moveInput);
//             rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, 0.2f);
//         }
//     }
// }
