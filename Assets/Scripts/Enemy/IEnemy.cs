using UnityEngine;

public interface IEnemy
{
    // 1. 컴포넌트 접근 (기존 유지)
    Animator EnemyAnimator { get; }
    Rigidbody EnemyRigidbody { get; }
    Transform EnemyTransform { get; }

    // 2. 외부 참조 (State가 추격을 위해 알아야 함)
    Transform Player { get; } // 플레이어를 향해 가야 하니까요!

    // 3. 데이터 접근 (JSON에서 읽어온 능력치들)
    // 모든 상태(State)는 이 데이터를 보고 속도나 사거리를 결정합니다.
    EnemyDataPackage DataPackage { get; }

    // 4. 상태 제어
    void SelectNextState();
    
    // 5. 유틸리티 (선택 사항이지만 있으면 매우 편함)
    // 적이 죽었는지, 혹은 현재 특정 상태인지 체크할 때 씁니다.
    bool IsDead { get; }
}