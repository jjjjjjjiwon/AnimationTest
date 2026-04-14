using UnityEngine;

public class StunState : State
{
    private float remainingTime; // 남은 시간 (타이머)
    private bool hasStarted;     // 애니메이션 재생 확인용
    private string animName;     // 재생할 애니메이션 이름

    // 외부에서 남은 시간을 조회하기 위한 속성 (Property)
    public float CurrentRemainingTime => Mathf.Max(0, remainingTime);

    public StunState(IEnemy enemy, float duration) : base(enemy)
    {
        this.remainingTime = duration;
        this.animName = enemy.DataPackage.stunData.animation_Name;
    }

    public override void Enter()
    {
        hasStarted = false;

        // 리액션 재생 및 물리 정지
        PlayStunAnimation();
        enemy.EnemyRigidbody.velocity = Vector3.zero;

        Debug.Log($"<color=purple>[Stun 진입]</color> 설정 시간: {remainingTime}초");
    }

    public override void Execute()
    {
        // 1. 타이머 감소
        remainingTime -= Time.deltaTime;

        /* 잠시 주석 처리하여 애니메이션 로직 배제
        if (!WaitForAnimationStart(enemy.EnemyAnimator, ref hasStarted, out AnimatorStateInfo stateInfo))
        {
            return;
        }
        */

        // 2. 오직 시간만 체크
        if (remainingTime <= 0)
        {
            Debug.Log("<color=red>[Stun]</color> 시간 종료!");
            enemy.SelectNextState();
        }
    }

    public void AddStunTime(float extraTime)
    {
        // [핵심] 기존 남은 시간에 새로운 시간을 더함 (누적)
        remainingTime += extraTime;

        // 리액션 갱신을 위해 애니메이션 다시 재생
        hasStarted = false;
        PlayStunAnimation();

        Debug.Log($"<color=purple>[Stun 연장]</color> +{extraTime}s | 현재 총 남은 시간: {remainingTime:F1}s");
    }

    private void PlayStunAnimation()
    {
        enemy.EnemyAnimator.Play(animName, 0, 0f);
    }

    public override void Exit()
    {
        Debug.Log("<color=purple>[Stun 해제]</color>");
    }
}