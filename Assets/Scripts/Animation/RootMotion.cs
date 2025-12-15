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

    private Vector3 accumulatedDelta;

    void OnAnimatorMove()
    {
        accumulatedDelta += animator.deltaPosition;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + accumulatedDelta);
        accumulatedDelta = Vector3.zero;
    }
}
