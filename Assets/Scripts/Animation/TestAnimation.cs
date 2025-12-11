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


        if(Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("LeftAttack");
        }

        if (Input.GetMouseButtonDown(1))  // 오른클릭
        {
            // 현재 상태 체크
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            // Left 공격 중일 때만 오른공격 가능
            if (stateInfo.IsName("Left_Mouse_Attack"))
            {
                animator.SetTrigger("RightAttack");
            }
        }
    }

    // 애니메이션 이벤트에서 호출됨
    void ActivateHitbox()
    {
        Debug.Log(">>> Hitbox ON!");
        // 실제로는 weaponHitbox.EnableHit();
    }
    
    void DeactivateHitbox()
    {
        Debug.Log(">>> Hitbox OFF!");
        // 실제로는 weaponHitbox.DisableHit();
    }


}
