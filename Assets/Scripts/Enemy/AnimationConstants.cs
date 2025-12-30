/// <summary>
/// 애니메이션 관련 상수 모음
/// Animator의 Tag, Trigger, State 이름을 중앙에서 관리
/// 오타 방지 및 유지보수 용이
/// </summary>
public static class AnimationConstants
{
    // ========== Animator Tags ==========
    
    /// <summary>기본 이동 애니메이션 Tag (Move, Idle 등)</summary>
    public const string MOVEMENT_TAG = "Movement";
    
    /// <summary>공격 애니메이션 Tag</summary>
    public const string ATTACK_TAG = "Attack";
    
    /// <summary>돌진 애니메이션 Tag</summary>
    public const string DASH_TAG = "Dash";
    
    /// <summary>기절 애니메이션 Tag</summary>
    public const string STUN_TAG = "Stun";
    
    /// <summary>사망 애니메이션 Tag</summary>
    public const string DEATH_TAG = "Death";

    // ========== Animator Triggers ==========
    
    /// <summary>돌진 Trigger</summary>
    public const string DASH_TRIGGER = "DASH";
    
    /// <summary>기절 Trigger</summary>
    public const string STUN_TRIGGER = "STUN";
    
    /// <summary>사망 Trigger</summary>
    public const string DEATH_TRIGGER = "DEATH";

    // ========== Animator States ==========
    
    /// <summary>기본 Move State 이름</summary>
    public const string MOVE_STATE = "Move";
}