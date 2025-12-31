using UnityEngine;

/// <summary>
/// Player 이동 상태
/// WASD로 카메라 기준 이동
/// </summary>
public class PlayerMoveState : PlayerState
{
    private Animator animator;
    private Rigidbody rb;
    private Transform cameraTransform;
    private PlayerData data;
    
    public PlayerMoveState(PlayerController player) : base(player)
    {
        animator = player.Animator;
        rb = player.Rigidbody;
        cameraTransform = player.CameraTransform;
        data = player.Data;
    }
    
    public override void Enter()
    {
        Debug.Log("PlayerMoveState 진입");
        
        // Walk 애니메이션 재생
        animator.Play(AnimationConstants.WALK);
    }
    
    public override void Execute()
    {
        // 입력 받기
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        // 입력 없으면 IdleState로
        if (horizontal == 0 && vertical == 0)
        {
            player.StateMachine.ChangeState(player.IdleState);
            return;
        }
        
        // 카메라 기준 이동 방향 계산
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;
        
        // Y축 제거 (평면 이동)
        cameraForward.y = 0;
        cameraRight.y = 0;
        cameraForward.Normalize();
        cameraRight.Normalize();
        
        // 이동 방향
        Vector3 moveDirection = (cameraForward * vertical + cameraRight * horizontal).normalized;
        
        // 이동
        Vector3 moveVelocity = moveDirection * data.walkSpeed;
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
        
        // 회전 (이동 방향으로)
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            player.transform.rotation = Quaternion.Slerp(
                player.transform.rotation,
                targetRotation,
                data.rotationSpeed * Time.fixedDeltaTime
            );
        }
    }
    
    public override void Exit()
    {
        Debug.Log("PlayerMoveState 종료");
    }
}