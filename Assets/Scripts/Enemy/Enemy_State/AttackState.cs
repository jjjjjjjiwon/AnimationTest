using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    private List<EnemyAttackMotionJsonData> currentComboSteps; // 현재 실행 중인 콤보의 모션 리스트
    private int currentStepIndex = 0;
    private bool hasStarted = false; // 현재 단계 애니메이션 시작 여부
    public bool IsExiting => isExiting;

    public AttackState(IEnemy enemy, List<EnemyAttackMotionJsonData> steps) : base(enemy)
    {
        this.currentComboSteps = steps;
    }

    public override void Enter()
    {
        currentStepIndex = 0;
        hasStarted = false;
        isExiting = false; // State 클래스의 변수 초기화

        Debug.Log($"[AttackState] 진입 - 총 {currentComboSteps.Count}단계 콤보 시작");
        ExecuteStep();
    }

    public override void Execute()
    {
        if (isExiting) return;

        // 플레이어를 계속 바라보게 함 (이전 단계에서 만든 로직)
        RotateTowardsPlayer();

        AnimatorStateInfo stateInfo;
        if (!WaitForAnimationStart(enemy.EnemyAnimator, ref hasStarted, out stateInfo)) return;

        // 애니메이션이 거의 끝났을 때 (95% 이상)
        if (stateInfo.normalizedTime >= 0.95f && !enemy.EnemyAnimator.IsInTransition(0))
        {
            // 1. 다음 공격 단계가 남아있는지 확인
            if (currentStepIndex < currentComboSteps.Count - 1)
            {
                // [핵심 수정] 다음 모션의 데이터를 미리 가져옵니다.
                var nextMotionData = currentComboSteps[currentStepIndex + 1];

                // [판단] 플레이어가 다음 모션의 '해제 범위' 안에 있는가?
                if (IsPlayerInRange(nextMotionData.combo_Release_Range))
                {
                    currentStepIndex++;
                    hasStarted = false; // 다음 애니메이션 대기를 위해 리셋
                    ExecuteStep();      // 다음 타격 실행
                }
                else
                {
                    // 플레이어가 너무 멀어지면 콤보 중단
                    Debug.Log($"[AttackState] 다음 모션({nextMotionData.animation_Name})의 해제 범위를 벗어남. 콤보 종료.");
                    FinishCombo();
                }
            }
            else
            {
                // 모든 콤보 시퀀스 완료
                FinishCombo();
            }
        }
    }

    private void ExecuteStep()
    {
        var currentData = currentComboSteps[currentStepIndex];

        Debug.Log($"[AttackState] 실행 중: {currentStepIndex + 1}타 ({currentData.animation_Name})");

        // 애니메이션 재생 (CrossFade로 부드럽게 연결)
        enemy.EnemyAnimator.CrossFade(currentData.animation_Name, 0.1f);

        // 필요 시 여기서 데미지 설정이나 회전 제어 로직 추가 가능
    }

    private void FinishCombo()
    {
        if (isExiting) return;

        isExiting = true;
        Debug.Log("[AttackState] 모든 콤보 시퀀스 종료 -> 상태 전환 요청");

        // 컨트롤러에게 다음 행동 판단을 맡김
        // 이때 SelectNextState()가 내부적으로 ChangeState(idle) 등을 호출하며 Exit()을 실행함
        enemy.SelectNextState();
    }

    public override void Exit()
    {
        Debug.Log("[AttackState] Exit 호출 - 리소스 정리");
        hasStarted = false;
        // 필요 시 공격 콜라이더 강제 종료 등 처리
    }

    private bool IsPlayerInRange(float range)
    {
        if (enemy.Player == null) return false;
        float dist = Vector3.Distance(enemy.EnemyTransform.position, enemy.Player.position);
        return dist <= range + 0.5f; // 0.5f 여유 오차
    }

    private void RotateTowardsPlayer()
    {
        if (enemy.Player == null) return;

        // 플레이어를 향한 방향 계산
        Vector3 direction = (enemy.Player.position - enemy.EnemyTransform.position).normalized;
        direction.y = 0; // 위아래로 꺾이는 것 방지

        if (direction != Vector3.zero)
        {
            // JSON에서 가져온 rotation_Speed 사용 (없다면 기본값 5.0f)
            float rotSpeed = currentComboSteps[currentStepIndex].rotation_Speed;
            if (rotSpeed <= 0) rotSpeed = 5.0f;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemy.EnemyTransform.rotation = Quaternion.Slerp(
                enemy.EnemyTransform.rotation,
                targetRotation,
                Time.deltaTime * rotSpeed
            );
        }
    }
}