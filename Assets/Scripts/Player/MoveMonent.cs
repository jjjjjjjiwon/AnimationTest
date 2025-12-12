using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMonent : MonoBehaviour
{
        public float Speed = 10f;  // 이동 속도
        public Transform cameraTransform;   // 카메라
        public Vector3 MoveInput => moveInput;
        
        private Vector3 moveInput;
        private Rigidbody rb;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();

            // 카메라 확인
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
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        // 카메라 기준 방향으로 이동 벡터 계산
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Y축 제거 → 평면 이동
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        moveInput = (forward * v + right * h).normalized;
    }

    void MovePlayer()
    {
        rb.MovePosition(rb.position + moveInput * Speed * Time.fixedDeltaTime);
    }
    void RotatePlayerToMoveDirection()
    {
        if (moveInput.sqrMagnitude > 0.01f)
        {
            // 이동 방향으로 회전
            Quaternion targetRotation = Quaternion.LookRotation(moveInput);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, 0.2f);
        }
    }
}
