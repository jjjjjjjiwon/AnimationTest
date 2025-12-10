using UnityEngine;

public class TestAnimation : MonoBehaviour
{
    private Animator animator;


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // W 키 누르면 달리기
        if (Input.GetKey(KeyCode.W))
        {
            animator.SetBool("isRun", true);
        }
        else
        {
            animator.SetBool("isRun", false);
        }

        // bool
        // 왼쪽 마우스 공격
        if(Input.GetMouseButtonDown(0))
        {
            animator.SetBool("isLeftAttack", true);
        }
        else
        {
            animator.SetBool("isLeftAttack", false);
            
        }
        
        // bool
        // 오른쪽 마우스 공격
        if(Input.GetMouseButtonDown(1))
        {
            animator.SetBool("isRightAttack", true);
        }
        else
        {
            animator.SetBool("isRightAttack", false);
            
        }
    }


}
