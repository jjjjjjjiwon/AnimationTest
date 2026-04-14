using UnityEngine;

public class TeleportState : State
{
    private EnemyTeleportStateJsonData data;
    private bool isMovementDone = false;

    private bool isExiting = false; // 종료 확인용 변수
    public bool IsExiting => isExiting;

    public TeleportState(IEnemy enemy, EnemyTeleportStateJsonData data) : base(enemy)
    {
        this.data = data;
    }

    public override void Enter()
    {
        isMovementDone = false;
        // 1. 사라지는 애니메이션 시작
        if (!string.IsNullOrEmpty(data.start_Animation_Name))
        {
            enemy.EnemyAnimator?.Play(data.start_Animation_Name);
        }

        // 텔레포트 중에는 물리 간섭을 최소화하기 위해 속도 제로
        if (enemy.EnemyRigidbody != null)
            enemy.EnemyRigidbody.velocity = Vector3.zero;

        Debug.Log($"[Teleport] 시작: {data.enemy_Teleport_State_ID}");
    }

   public override void Execute()
{
    Animator anim = enemy.EnemyAnimator;
    if (anim == null) return;

    AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

    // 1. 사라지는 단계 (이동 전)
    if (!isMovementDone)
    {
        if (stateInfo.IsName(data.start_Animation_Name) && stateInfo.normalizedTime >= 0.95f)
        {
            PerformTeleport();
            anim.Play(data.end_Animation_Name);
            isMovementDone = true;
            Debug.Log("[Teleport] 이동 실행 -> 엔드 애니메이션 재생 시작");
        }
        return; // 이동한 프레임에는 일단 종료
    }

    // 2. 나타나는 단계 (이동 후)
    if (isMovementDone)
    {
        if (stateInfo.IsName(data.end_Animation_Name))
        {
            if (stateInfo.normalizedTime >= 0.95f)
            {
                isExiting = true; // [드디어 방어막 해제]
                Debug.Log("[Teleport] 완료 -> 다음 상태로 전환 시도");
                enemy.SelectNextState();
            }
        }
        else if (!anim.IsInTransition(0))
        {
            // 혹시라도 애니메이션 이름이 안 맞아서 멈추는 것 방지 (보험)
            // Debug.Log("현재 애니메이션 이름: " + stateInfo.fullPathHash);
        }
    }
}

    private void PerformTeleport()
    {
        if (enemy.Player == null) return;

        Vector3 relativeDir = data.arrival_Direction.normalized;
        Vector3 targetPos = enemy.Player.position + (relativeDir * data.arrival_Distance);

        enemy.EnemyTransform.position = targetPos;

        // --- [수정 포인트: 누워있는 현상 방지] ---
        // 플레이어를 바라보되, 몸이 기울지 않게 y축만 계산해서 봅니다.
        Vector3 lookTarget = new Vector3(enemy.Player.position.x, enemy.EnemyTransform.position.y, enemy.Player.position.z);
        enemy.EnemyTransform.LookAt(lookTarget);

        // 만약 리지드바디가 있다면 속도를 0으로 초기화해서 미끄러짐 방지
        if (enemy.EnemyRigidbody != null) enemy.EnemyRigidbody.velocity = Vector3.zero;
    }

    public override void Exit() { }
}