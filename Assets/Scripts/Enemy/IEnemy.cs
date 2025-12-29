using UnityEngine;

/// <summary>
/// Enemy의 인터페이스
/// State들이 Enemy의 컴포넌트와 데이터에 접근하기 위한 계약
/// EnemyController가 이 인터페이스를 구현
/// </summary>
public interface IEnemy
{
    // ========== 컴포넌트 접근 ==========
    
    /// <summary>Enemy의 Transform</summary>
    Transform Transform { get; }
    
    /// <summary>추적 대상 (Player)</summary>
    Transform Player { get; }
    
    /// <summary>물리 처리용 Rigidbody</summary>
    Rigidbody Rigidbody { get; }
    
    /// <summary>애니메이션 제어용 Animator</summary>
    Animator Animator { get; }
    
    /// <summary>Enemy 설정 데이터</summary>
    EnemyData Data { get; }

    // ========== State 전환 메서드 ==========
    
    /// <summary>
    /// IdleState로 전환 (Hub로 복귀)
    /// 모든 State가 완료 후 호출
    /// </summary>
    void ChangeToIdle();
}