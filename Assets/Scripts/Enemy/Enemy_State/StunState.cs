// using UnityEngine;

// /// <summary>
// /// 기절 상태
// /// 일정 시간 동안 기절 애니메이션 재생
// /// 시간 경과 + 애니메이션 완료 시 IdleState로 복귀
// /// </summary>
// public class StunState : State
// {
//     private Animator animator;
//     private float stunTimer; // 기절 경과 시간
//     private bool hasStarted; // 애니메이션 시작 여부

//     public StunState(IEnemy enemy) : base(enemy)
//     {
//         animator = enemy.Animator;
//     }

//     public override void Enter()
//     {
//         hasStarted = false; // 애니메이션 시작 플래그 리셋
//         stunTimer = 0f; // 타이머 초기화

//         // 기절 애니메이션 시작
//         animator.SetTrigger(AnimationConstants.STUN_TRIGGER);

//         // 움직임 정지
//         enemy.Rigidbody.velocity = new Vector3(0, enemy.Rigidbody.velocity.y, 0);
//     }

//     public override void Execute()
//     {
//         // 기절 중 움직임 방지 (매 프레임)
//         enemy.Rigidbody.velocity = new Vector3(0, enemy.Rigidbody.velocity.y, 0);

//         // ========== 1. 애니메이션 시작 대기 ==========
//         if (!WaitForAnimationStart(animator, ref hasStarted, out AnimatorStateInfo stateInfo))
//         {
//             // 아직 기절 애니메이션 시작 안 됨 (Move 중)
//             return;
//         }

//         // ========== 2. 기절 시간 측정 ==========
//         stunTimer += Time.deltaTime;

//         // ========== 3. 완료 조건 체크 ==========
//         bool timeFinished = stunTimer >= enemy.Data.stunDuration;
//         bool animationFinished = stateInfo.IsTag(AnimationConstants.MOVEMENT_TAG);

//         // 시간 경과 AND 애니메이션 완료 = 기절 완료
//         if (timeFinished && animationFinished)
//         {
//             enemy.ChangeToIdle();
//         }

//         // 기절 진행 중 (STUN_TAG)
//     }

//     public override void Exit()
//     {
//         // Trigger 리셋
//         animator.ResetTrigger(AnimationConstants.STUN_TRIGGER);
//     }
// }