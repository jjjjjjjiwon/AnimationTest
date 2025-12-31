using UnityEngine;

/// <summary>
/// Player의 인터페이스
/// State들이 Player의 컴포넌트와 데이터에 접근하기 위한 계약
/// PlayerController가 이 인터페이스를 구현
/// </summary>
public interface IPlayer
{
    // ========== 컴포넌트 접근 ==========
    
    /// <summary>Player의 Transform</summary>
    Transform Transform { get; }
    
    /// <summary>카메라 Transform (이동 방향 계산용)</summary>
    Transform CameraTransform { get; }
    
    /// <summary>물리 처리용 Rigidbody</summary>
    Rigidbody Rigidbody { get; }
    
    /// <summary>애니메이션 제어용 Animator</summary>
    Animator Animator { get; }
    
    /// <summary>Player 설정 데이터</summary>
    PlayerData Data { get; }

    // ========== 리소스 ==========
    
    /// <summary>현재 HP</summary>
    float CurrentHP { get; }
    
    /// <summary>최대 HP</summary>
    float MaxHP { get; }
    
    /// <summary>현재 스턴 게이지</summary>
    float CurrentStunGauge { get; }
    
    /// <summary>최대 스턴 게이지</summary>
    float MaxStunGauge { get; }

    // ========== 회피 ==========
    
    /// <summary>회피 쿨타임이 끝났는지</summary>
    bool CanDodge { get; }

    // ========== State 전환 메서드 ==========
    
    /// <summary>
    /// IdleState로 전환
    /// </summary>
    void ChangeToIdle();
    
    /// <summary>
    /// MoveState로 전환
    /// </summary>
    void ChangeToMove();
    
    /// <summary>
    /// AttackState로 전환
    /// </summary>
    void ChangeToAttack();
    
    /// <summary>
    /// FinisherState로 전환
    /// </summary>
    void ChangeToFinisher();
    
    /// <summary>
    /// DodgeState로 전환
    /// </summary>
    void ChangeToDodge();
    
    /// <summary>
    /// HitState로 전환 (스턴)
    /// </summary>
    void ChangeToHit();
    
    /// <summary>
    /// DeadState로 전환 (사망)
    /// </summary>
    void ChangeToDead();

    // ========== 리소스 관리 ==========
    
    /// <summary>
    /// HP 감소
    /// </summary>
    void TakeDamage(float damage);
    
    /// <summary>
    /// 스턴 게이지 감소
    /// </summary>
    void TakeStunDamage(float damage);
    
    /// <summary>
    /// HP 회복
    /// </summary>
    void Heal(float amount);
}