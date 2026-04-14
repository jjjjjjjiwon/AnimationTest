using UnityEngine;

public class BossController : TestEnemyController
{
    // [1. 데이터 및 스탯 저장소]
    private EnemyDataPackage currentPackage;

    // 팩토리가 준 '기준점' (변하지 않음)
    protected float maxHP;
    protected float moveSpeed;
    protected float defense;
    protected float baseDamage;

    private float teleportTimer; // 텔레포트 쿨타임 관리

    /// <summary>
    /// 팩토리에서 소환 직후 호출하여 보스의 모든 설정을 마칩니다.
    /// </summary>
    public override void Setup(EnemyDataPackage package)
    {
        base.Setup(package);
        // 부모 setup에서 이미 많은 것을 처리하므로, 보스 전용 스탯만 추가 확인
        this.maxHP = package.finalMaxHP;
        this.currentHP = this.maxHP;
    }

    
    
    /// <summary>
    /// 유저님이 구상하신 "데이터가 있으면 해당 상태로 전환"하는 로직
    /// </summary>
    public void ExecuteMovement()
    {
        // 팩토리가 업그레이드 ID까지 다 계산해서 넣어준 실물 데이터를 확인
        if (currentPackage.teleportData != null)
        {
            stateMachine.ChangeState(new TeleportState(this, currentPackage.teleportData));
        }
        else if (currentPackage.dashData != null)
        {
            stateMachine.ChangeState(new DashState(this, currentPackage.dashData));
        }
        else
        {
            // 둘 다 없으면 기본 추격
            stateMachine.ChangeState(chaseState);
        }
    }

    /// <summary>
    /// 실시간 데미지 처리 (컨트롤러의 핵심 역할)
    /// </summary>
    public virtual void TakeDamage(float damage)
    {
        // 방어력 적용 (최소 1 데미지 보장)
        float finalDamage = Mathf.Max(1, damage - defense);
        currentHP -= finalDamage;

        Debug.Log($"{gameObject.name} 피격! 남은 HP: {currentHP}/{maxHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        if (deathState != null)
            stateMachine.ChangeState(deathState);
        else
            Destroy(gameObject);
    }
}