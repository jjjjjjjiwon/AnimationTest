// using UnityEngine;

// /// <summary>
// /// 사망 상태
// /// 사망 애니메이션 완료 후 이펙트 재생하고 오브젝트 삭제
// /// 이 State에서는 IdleState로 복귀하지 않음 (최종 상태)
// /// </summary>
// public class DeathState : State
// {
//     private Animator animator;
//     private bool hasStarted;
//     private bool deathComplete; // 사망 처리 완료 여부 (중복 호출 방지)

//     public DeathState(IEnemy enemy) : base(enemy)
//     {
//         animator = enemy.Animator;
//     }

//     public override void Enter()
//     {
//         deathComplete = false;

//         // 사망 애니메이션 시작
//         animator.SetTrigger(AnimationConstants.DEATH_TRIGGER);

//         // 움직임 완전 정지
//         enemy.Rigidbody.velocity = Vector3.zero;
        
//         // 물리 비활성화 (충돌 무시)
//         enemy.Rigidbody.isKinematic = true;
//     }

//     public override void Execute()
//     {
//         // 이미 사망 처리 완료되었으면 실행 안 함
//         if (deathComplete)
//             return;

//         // ========== 1. 애니메이션 시작 대기 ==========
//         if (!WaitForAnimationStart(animator, ref hasStarted, out AnimatorStateInfo stateInfo))
//         {
//             // 아직 사망 애니메이션 시작 안 됨 (Move 중)
//             return;
//         }

//         // ========== 2. 애니메이션 완료 체크 ==========
//         // normalizedTime >= 1.0: 애니메이션 100% 완료
//         // !IsTag(MOVEMENT_TAG): 여전히 Death 애니메이션 중 (Move 복귀 안 함)
//         if (stateInfo.normalizedTime >= 1.0f && !stateInfo.IsTag(AnimationConstants.MOVEMENT_TAG))
//         {
//             // 사망 애니메이션 완료!
//             OnDeathComplete();
//             deathComplete = true; // 중복 호출 방지
//         }

//         // 사망 진행 중 (DEATH_TAG)
//     }

//     public override void Exit()
//     {
//         // DeathState는 종료 없음 (최종 상태)
//         // 하지만 만약을 위한 정리
//         animator.ResetTrigger(AnimationConstants.DEATH_TRIGGER);
//     }

//     /// <summary>
//     /// 사망 처리 완료 시 호출
//     /// 사망 이펙트 재생 후 오브젝트 즉시 삭제
//     /// </summary>
//     private void OnDeathComplete()
//     {
//         // 사망 이펙트 재생 (옵션)
//         if (enemy.Data.deathEffectPrefab != null)
//         {
//             Object.Instantiate(enemy.Data.deathEffectPrefab, enemy.Transform.position, Quaternion.identity);
//         }

//         // 애니메이션 완료 후 즉시 삭제
//         Object.Destroy(enemy.Transform.gameObject);
//     }
// }