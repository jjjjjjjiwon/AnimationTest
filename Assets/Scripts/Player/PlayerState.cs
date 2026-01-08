using UnityEngine;

/// <summary>
/// Player 상태 베이스 클래스
/// </summary>
public abstract class PlayerState
{
    protected PlayerController player;

    public PlayerState(PlayerController player)
    {
        this.player = player;
    }

    // ========================================
    // 상태 속성
    // ========================================

    /// <summary>
    /// 이 상태가 콤보를 중단시키는가?
    /// - false: Idle, Move, Attack (콤보 유지)
    /// - true: Dodge, Stun, Hit, Death (콤보 중단)
    /// </summary>
    public virtual bool InterruptsCombo => false;

    // ========================================
    // 상태 생명주기
    // ========================================

    public virtual void Enter()
    {
        // ========== 자동 콤보 리셋 ==========
        if (InterruptsCombo)
        {
            player.ComboSocket.ResetCombo();
            Debug.Log($"[{GetType().Name}] 콤보 리셋!");
        }
    }

    public abstract void Execute();

    public virtual void Exit() { }

    // ========================================
    // 유틸리티
    // ========================================

    /// <summary>
    /// 애니메이션 시작 대기
    /// - 첫 프레임에는 stateInfo가 이전 상태를 가리킴
    /// - normalizedTime이 0보다 클 때까지 대기
    /// </summary>
    protected bool WaitForAnimationStart(Animator animator, ref bool started, out AnimatorStateInfo stateInfo)
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!started)
        {
            if (stateInfo.normalizedTime > 0f)
            {
                started = true;
                return true;
            }
            return false;
        }

        return true;
    }
}