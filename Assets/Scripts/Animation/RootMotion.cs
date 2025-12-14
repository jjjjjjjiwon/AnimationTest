using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootMotion : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }
    
    // Root Motion 수동 제어
    void OnAnimatorMove()
    {
        // Animator가 계산한 이동량
        Vector3 deltaPosition = animator.deltaPosition;
        
        // Rigidbody로 이동 (돌아가지 않음!)
        rb.MovePosition(rb.position + deltaPosition);
    }
}
