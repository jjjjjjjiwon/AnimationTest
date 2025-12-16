using UnityEngine;

# region
public class TestAnimation : MonoBehaviour
{
    public bool IsMove => isMove;
    private bool isMove = true;


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
        animator.SetFloat("SPEED", moveMonent.AnimatorSpeed);

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("LeftAttack");
        }
        if (Input.GetMouseButtonDown(1))  // 오른클릭
        {
            // 현재 상태 체크
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            // Left 공격 중일 때만 오른공격 가능
            if (stateInfo.IsName("Frist_Attack") || stateInfo.IsName("Three_Attack"))  
            {
                animator.SetTrigger("RightAttack");
            }
        }


        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            animator.SetTrigger("Dash");
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
int a = 0;

    // 이동 관련
    void EnableMovement()
    {
        isMove = true;
        Debug.Log($"aaaaaaa  {a++}");
    }

    void DisableMovement()
    {
        isMove = false;
    }

}
# endregion