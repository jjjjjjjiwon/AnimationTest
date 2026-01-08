using UnityEngine;

/// <summary>
/// Player 대기 상태
/// 입력 대기 중
/// </summary>
public class PlayerIdleState : PlayerState
{
    private Animator animator;
    private readonly int isMovingHash = Animator.StringToHash("IsMoving");
    public override bool InterruptsCombo => false;

    public PlayerIdleState(PlayerController player) : base(player)
    {
        animator = player.Animator;
    }

    public override void Enter()
    {
        base.Enter();
        animator.SetBool(isMovingHash, false);  // Transition용
        animator.Play(AnimationConstants.IDLE);  // 강제 재생
        player.Rigidbody.velocity = Vector3.zero;
    }

    public override void Execute()
    {
        // WASD 입력 확인
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // 이동 입력 있으면 MoveState로
        if (horizontal != 0 || vertical != 0)
        {
            player.StateMachine.ChangeState(player.MoveState);
        }
    }

    public override void Exit()
    {
        Debug.Log("PlayerIdleState 종료");
    }
}