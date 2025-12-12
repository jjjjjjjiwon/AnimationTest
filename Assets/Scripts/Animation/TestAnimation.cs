using UnityEngine;

# region // Root Motion
// public class TestAnimation : MonoBehaviour
// {
//     private Animator animator;
//     private HitBox hitBox;

//     void Awake()
//     {
//         animator = GetComponent<Animator>();
//         hitBox = GetComponentInChildren<HitBox>();
//     }

//     void Update()
//     {
//         // 공격
//         if (Input.GetMouseButtonDown(0))
//             animator.SetTrigger("LeftAttack");

//         if (Input.GetMouseButtonDown(1))
//         {
//             AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
//             if (stateInfo.IsName("Left_Mouse_Attack"))
//                 animator.SetTrigger("RightAttack");
//         }
//     }

//     // 애니메이션 이벤트에서 호출
//     void ActivateHitbox() => hitBox.EnableHit();
//     void DeactivateHitbox() => hitBox.DisableHit();
// }
#endregion

# region
public class TestAnimation : MonoBehaviour
{
    private Animator animator;
    private HitBox hitBox;
    private MoveMonent moveMonent;


    void Start()
    {
        animator = GetComponent<Animator>();
        hitBox = GetComponentInChildren<HitBox>();
        moveMonent = GetComponent<MoveMonent>();
    }

    void Update()
    {
        animator.SetFloat("speed", moveMonent.AnimatorSpeed);

        if (Input.GetMouseButtonDown(0))
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
        hitBox.EnableHit();
    }
    
    void DeactivateHitbox()
    {
        Debug.Log(">>> Hitbox OFF!");
        hitBox.DisableHit();

    }

}
# endregion