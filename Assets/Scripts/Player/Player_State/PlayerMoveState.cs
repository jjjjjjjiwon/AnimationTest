using UnityEngine;

/// <summary>
/// Player 이동 상태
/// WASD로 카메라 기준 이동
/// Bool Parameter + Transition 사용
/// </summary>
public class PlayerMoveState : PlayerState
{
    private Animator animator;
    private Rigidbody rb;
    private Transform cameraTransform;
    private PlayerData data;
    
    // 애니메이션 파라미터 해시 (최적화)
    private readonly int isMovingHash = Animator.StringToHash("IsMoving");
    
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
        
        // IsMoving = true → Animator가 Run으로 전환
        animator.SetBool(isMovingHash, true);
    }
    
    public override void Execute()
{
    
    // 입력 받기
    float horizontal = Input.GetAxisRaw("Horizontal");
    float vertical = Input.GetAxisRaw("Vertical");
    
    // ========== UI 열려있으면 이동 안 함 ==========
    if (SocketManagerUI.IsUIOpen)
    {
        rb.velocity = Vector3.zero;
        animator.SetBool(isMovingHash, false);
        return;
    }
    
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
    
    // 이동 (Y축 0으로 고정!)
    Vector3 moveVelocity = moveDirection * data.moveSpeed;
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
        
        // IsMoving = false는 IdleState에서 설정
        // 여기서는 안 해도 됨 (다음 State가 설정할 것)
    }
}