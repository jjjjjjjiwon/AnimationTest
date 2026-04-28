using UnityEngine;

/// <summary>
/// 추격 상태
/// Player를 향해 이동하며 추적
/// EnemyController가 매 프레임 탈출 조건 체크 (거리)
/// 조건 맞으면 Controller가 IdleState로 전환
/// </summary>
public class ChaseState : State
{
    private Rigidbody rb;
    private EnemyChaseStateJsonData chaseData;
    private TestEnemyController enemyController; // 구체적인 기능을 쓰기 위해 타입 캐스팅

    public ChaseState(IEnemy enemy, EnemyChaseStateJsonData data) : base(enemy)
    {
        // IEnemy 인터페이스를 실제 TestEnemyController로 형변환해서 참조를 보관합니다.
        this.enemyController = enemy as TestEnemyController;
        this.rb = enemy.EnemyRigidbody;
        this.chaseData = data;
    }

    public override void Enter()
    {
        if (chaseData != null && !string.IsNullOrEmpty(chaseData.animation_Name))
        {
            enemy.EnemyAnimator.Play(chaseData.animation_Name);
        }
    }

    public override void Execute()
    {
        if (enemyController == null) return;

        Vector3 moveDir = Vector3.zero;
        float speedMultiplier = 1.0f; // 기본 속도 배율
        string enemyName = enemyController.DataPackage.baseData.enemy_Name;

        switch (enemyName)
        {
            case "Archer":
                moveDir = EnemyAILibrary.GetZombieMove(enemy.EnemyTransform, enemy.Player);
                break;
            case "Zombie": // Archer도 Assassin의 회피 로직을 공유하도록 조립
                float visionAngle = enemyController.DataPackage.baseData.vision_Angle; // JSON에서 가져옴
                float detectRange = enemyController.DataPackage.baseData.detect_Range;

                // 라이브러리 호출 시 속도 배율을 받아옵니다 (out 키워드)
                moveDir = EnemyAILibrary.GetAssassinMove(
                    enemy.EnemyTransform,
                    enemy.Player,
                    visionAngle,
                    detectRange,
                    out speedMultiplier
                );
                break;
                // ... 생략
        }

        // 3. 최종 속도 계산 (JSON 속도 * 회피 배율)
        float baseSpeed = enemyController.DataPackage.baseData.base_Speed;
        float finalSpeed = baseSpeed * speedMultiplier;

        // 4. 물리 적용
        rb.velocity = new Vector3(moveDir.x * finalSpeed, rb.velocity.y, moveDir.z * finalSpeed);

        if (moveDir != Vector3.zero)
            enemy.EnemyTransform.forward = moveDir;
    }

    public override void Exit()
    {
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
    }
}