using UnityEngine;
public class DeathState : State
{
    private bool hasStarted;
    private bool deathComplete;
    private string animName;

    public DeathState(IEnemy enemy) : base(enemy)
    {
        // JSON에서 받아온 애니메이션 이름 설정
        this.animName = enemy.DataPackage.deathData.animation_Name;
    }

    public override void Enter()
    {
        deathComplete = false;
        hasStarted = false;

        // 1. 애니메이션 이름으로 직접 재생
        enemy.EnemyAnimator.Play(animName, 0, 0f);

        // 2. 물리 및 이동 완전 정지
        enemy.EnemyRigidbody.velocity = Vector3.zero;
        enemy.EnemyRigidbody.isKinematic = true;

        Debug.Log($"<color=lime>[Death]</color> '{animName}' 애니메이션 시작");
    }

    public override void Execute()
    {
        if (deathComplete) return;
        
        // 애니메이션 시작 대기
        if (!WaitForAnimationStart(enemy.EnemyAnimator, ref hasStarted, out AnimatorStateInfo stateInfo))
        {
            return;
        }

        // 애니메이션 완료 체크 (1.0f는 100% 완료를 의미)
        if (stateInfo.normalizedTime >= 1.0f)
        {
            deathComplete = true;
            OnDeathComplete();
        }
    }

    private void OnDeathComplete()
    {
        // 3. 이펙트 (JSON에 이펙트 이름이 있다면 여기서 Factory 등에 요청)
        // 지금은 단순 로그와 파괴만 처리
        Debug.Log("<color=lime>[사망 완료]</color> 오브젝트를 삭제합니다.");
        
        Object.Destroy((enemy as MonoBehaviour).gameObject);
    }

    public override void Exit() { }
}